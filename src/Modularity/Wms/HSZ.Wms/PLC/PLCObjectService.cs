using Aspose.Cells;
using HSZ.Common.Core.Manager;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.Logging.Attributes;
using HSZ.Wms.Interfaces.PLC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using S7.Net;
using S7.Net.Types;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using ZJN.Entitys.wcs;
using ZJN.Plc.PlcHelper;

namespace HSZ.Wms.PLC
{
    /// <summary>
    /// PLC服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "PLCObject", Order = 200)]
    [Route("api/wms/[controller]")]
    public class PLCObjectService : IPLCObjectService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsPlcEntity> _zjnWcsPlcRepository;
        private readonly ISqlSugarRepository<ZjnWcsPlcPointEntity> _zjnWcsPlcPointRepository;
        private readonly ISqlSugarRepository<ZjnWcsPlcObjectEntity> _zjnWcsPlcObjectRepository;
        private readonly ISqlSugarRepository<ZjnWcsWorkDeviceEntity> _zjnWcsWorkDeviceRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private List<ZjnWcsWorkDeviceEntity> deviceList = new List<ZjnWcsWorkDeviceEntity>();

        public PLCObjectService(ISqlSugarRepository<ZjnWcsPlcEntity> zjnWcsPlcRepository, ISqlSugarRepository<ZjnWcsPlcObjectEntity> zjnWcsPlcObjectRepository, ISqlSugarRepository<ZjnWcsWorkDeviceEntity> zjnWcsWorkDeviceRepository, IUserManager userManager, ISqlSugarRepository<ZjnWcsPlcPointEntity> zjnWcsPlcPointRepository)
        {
            _zjnWcsPlcRepository = zjnWcsPlcRepository;
            _zjnWcsPlcObjectRepository = zjnWcsPlcObjectRepository;
            _zjnWcsWorkDeviceRepository = zjnWcsWorkDeviceRepository;
            _userManager = userManager;
            _db = DbScoped.SugarScope;
            _zjnWcsPlcPointRepository = zjnWcsPlcPointRepository;
        }

        /// <summary>
        /// PLC调用最新
        /// 用途：连接成功的添加，连接失败的显示
        /// 创建：于明亮
        /// </summary>
        /// <returns></returns>
        [HttpGet("InitNew")]
        [AllowAnonymous]
        [IgnoreLog]
        public async Task<dynamic> InitNew(string plcid = "")
        {
            string error = "";
            try
            {
                var plcList = await _zjnWcsPlcRepository.AsQueryable().WhereIF(!string.IsNullOrEmpty(plcid), s => s.PlcId == plcid).Where(s => s.IsActive).ToArrayAsync();
                if (plcList.Length == 0) return "未找到plc连接表！";
                //var plcIds = await _zjnWcsPlcObjectRepository.AsQueryable().GroupBy(g => g.PlcId)
                //        .Select(s => s.PlcId).ToArrayAsync();
                foreach (var plc in plcList)
                {
                    //if (plcIds.Contains(plc.PlcId)) continue;
                    //创建plc连接
                    var device = new Device((CpuType)plc.CpuType, plc.Ip, plc.Port ?? 102, Convert.ToInt16(plc.Rack ?? 0), Convert.ToInt16(plc.Sock ?? 0), plc.PlcId, plc.StackerId);
                    try
                    {
                        await device.StartAsync();
                        if (device.Plc.IsConnected)
                        {
                            var plcPointList = await _zjnWcsPlcPointRepository.AsQueryable().Where(w => w.IsActive && w.PlcId == plc.PlcId).ToArrayAsync();
                            //_db.BeginTran();
                            foreach (var plcPoint in plcPointList)
                            {
                                var obj = device.ReadDbClass(plcPoint.ObjType, plcPoint.Db, plcPoint.Start, plcPoint.ListCount);

                                List<ZjnWcsPlcObjectEntity> plcObjectList = new List<ZjnWcsPlcObjectEntity>();
                                object[] objs = obj.GetType().GetProperty("List").GetValue(obj) as object[];
                                for (int i = 0; i < plcPoint.ListCount; i++)
                                {
                                    var deviceId = objs[i].GetType().GetProperty("DeviceCode_S").GetValue(objs[i]).ToString();
                                    if (!string.IsNullOrEmpty(deviceId) && !deviceList.Any(a => a.DeviceId == deviceId))
                                    {
                                        var deviceObject = new ZjnWcsWorkDeviceEntity()
                                        {
                                            DeviceId = deviceId,
                                            Caption = deviceId,
                                            DeviceType = "2",
                                            IsActive = 1,
                                            PlcID = plc.PlcId,
                                            PlcIP = plc.Ip,
                                            IsDelete = 0,
                                            IsController = "1",
                                        };
                                        deviceList.Add(deviceObject);
                                    }
                                    var plcObject = new ZjnWcsPlcObjectEntity()
                                    {
                                        Length = plcPoint.Start + plcPoint.PackSize,
                                        PlcPointId = plcPoint.PlcPointId.ToString(),
                                        Region = plcPoint.Region,
                                        Timestamp = plcPoint.Timestamp,
                                        Db = plcPoint.Db,
                                        Descrip = plcPoint.Descrip,
                                        DeviceId = deviceId,
                                        PackSize = plcPoint.PackSize,
                                        ObjType = plcPoint.ObjType.Remove(plcPoint.ObjType.Length - 4),
                                        ObjValue = JsonConvert.SerializeObject(objs[i]),
                                        PackType = plcPoint.PackType,
                                        PlcId = plc.PlcId,
                                        Start = plcPoint.Start + plcPoint.PackSize * i,
                                        PlcObjId = YitIdHelper.NextId().ToString()
                                    };
                                    plcObjectList.Add(plcObject);

                                    var plcobjentity = await _zjnWcsPlcObjectRepository.GetFirstAsync(x => x.DeviceId == plcObject.DeviceId && x.ObjType == plcObject.ObjType);
                                    if (plcobjentity != null)
                                    {
                                        plcObject.PlcObjId = plcobjentity.PlcObjId;
                                        var resupdate = await _zjnWcsPlcObjectRepository.AsUpdateable(plcObject).ExecuteCommandAsync();
                                        if (resupdate <= 0)
                                        {
                                            error += plcObject.DeviceId + " 修改失败！";

                                        }
                                    }
                                    else
                                    {
                                        var resInt = await _zjnWcsPlcObjectRepository.AsInsertable(plcObject).ExecuteCommandAsync();
                                        if (resInt > 0)
                                        {

                                        }
                                        else
                                        {
                                            error += plcObject.DeviceId + " 添加失败！";
                                        }
                                    }



                                }
                                //await _zjnWcsPlcObjectRepository.AsInsertable(plcObjectList).ExecuteCommandAsync();


                            }
                            //_db.CommitTran();
                        }
                    }
                    catch (Exception)
                    {
                        device.Close();

                        error += "plc连接失败!ip:" + plc.Ip + "\n";
                    }

                }
                foreach (var item in deviceList)
                {
                    try
                    {
                        await _zjnWcsWorkDeviceRepository.AsInsertable(item).ExecuteCommandAsync();

                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                //_db.RollbackTran();
                throw HSZException.Oh(ex.Message);
            }

            return error;
        }


        /// <summary>
        /// PLC调用
        /// </summary>
        /// <returns></returns>
        [HttpGet("Init")]
        [AllowAnonymous]
        [IgnoreLog]
        public async Task Init(string plcid = "")
        {
            try
            {
                var plcList = await _zjnWcsPlcRepository.AsQueryable().WhereIF(!string.IsNullOrEmpty(plcid), s => s.PlcId == plcid).Where(s => s.IsActive).ToArrayAsync();
                if (plcList.Length == 0) return;
                var plcIds = await _zjnWcsPlcObjectRepository.AsQueryable().GroupBy(g => g.PlcId)
                        .Select(s => s.PlcId).ToArrayAsync();
                foreach (var plc in plcList)
                {
                    if (plcIds.Contains(plc.PlcId)) continue;
                    //创建plc连接
                    var device = new Device((CpuType)plc.CpuType, plc.Ip, plc.Port ?? 102, Convert.ToInt16(plc.Rack ?? 0), Convert.ToInt16(plc.Sock ?? 0), plc.PlcId, plc.StackerId);
                    await device.StartAsync();
                    if (device.Plc.IsConnected)
                    {
                        var plcPointList = await _zjnWcsPlcPointRepository.AsQueryable().Where(w => w.IsActive && w.PlcId == plc.PlcId).ToArrayAsync();
                        _db.BeginTran();
                        foreach (var plcPoint in plcPointList)
                        {
                            if (plcPoint.IsList)
                            {
                                var list = device.ReadDbClass(plcPoint.ObjType, plcPoint.Db, plcPoint.Start, plcPoint.ListCount);
                                await AddPlcList(plc, plcPoint, list);
                            }
                            else
                            {
                                var obj = device.ReadDbClass(plcPoint.ObjType, plcPoint.Db, plcPoint.Start);
                                await AddPlcObject(plc, plcPoint, obj);
                            }
                        }
                        _db.CommitTran();
                    }
                    device.Close();
                }
                foreach (var item in deviceList)
                {
                    try
                    {
                        await _zjnWcsWorkDeviceRepository.AsInsertable(item).ExecuteCommandAsync();

                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                _db.RollbackTran();
                throw HSZException.Oh(HSZ.Common.Enum.ErrorCode.COM1000);
            }
        }

        private async Task AddPlcList(ZjnWcsPlcEntity plc, ZjnWcsPlcPointEntity plcPoint, object obj)
        {
            List<ZjnWcsPlcObjectEntity> plcObjectList = new List<ZjnWcsPlcObjectEntity>();
            object[] objs = obj.GetType().GetProperty("List").GetValue(obj) as object[];
            for (int i = 0; i < plcPoint.ListCount; i++)
            {
                var deviceId = objs[i].GetType().GetProperty("DeviceCode_S").GetValue(objs[i]).ToString();
                if (!string.IsNullOrEmpty(deviceId) && !deviceList.Any(a => a.DeviceId == deviceId))
                {
                    var deviceObject = new ZjnWcsWorkDeviceEntity()
                    {
                        DeviceId = deviceId,
                        Caption = deviceId,
                        DeviceType = "2",
                        IsActive = 1,
                        PlcID = plc.PlcId,
                        PlcIP = plc.Ip,
                        IsDelete = 0,
                        IsController = "1",
                    };
                    deviceList.Add(deviceObject);
                }
                var plcObject = new ZjnWcsPlcObjectEntity()
                {
                    Length = plcPoint.Start + plcPoint.PackSize,
                    PlcPointId = plcPoint.PlcPointId.ToString(),
                    Region = plcPoint.Region,
                    Timestamp = plcPoint.Timestamp,
                    Db = plcPoint.Db,
                    Descrip = plcPoint.Descrip,
                    DeviceId = deviceId,
                    PackSize = plcPoint.PackSize,
                    ObjType = plcPoint.ObjType.Remove(plcPoint.ObjType.Length - 4),
                    ObjValue = JsonConvert.SerializeObject(objs[i]),
                    PackType = plcPoint.PackType,
                    PlcId = plc.PlcId,
                    Start = plcPoint.Start + plcPoint.PackSize * i,
                    PlcObjId = YitIdHelper.NextId().ToString()
                };
                plcObjectList.Add(plcObject);
            }
            await _zjnWcsPlcObjectRepository.AsInsertable(plcObjectList).ExecuteCommandAsync();
        }

        private async Task AddPlcObject(ZjnWcsPlcEntity plc, ZjnWcsPlcPointEntity plcPoint, object obj)
        {
            List<ZjnWcsPlcObjectEntity> plcObjectList = new List<ZjnWcsPlcObjectEntity>();
            var deviceId = obj.GetType().GetProperty("DeviceCode").GetValue(obj).ToString();
            if (!deviceList.Any(a => a.DeviceId == deviceId))
            {
                var deviceObject = new ZjnWcsWorkDeviceEntity()
                {
                    DeviceId = deviceId,
                    DeviceType = "1",
                    IsActive = 1,
                    PlcID = plc.PlcId,
                    PlcIP = plc.Ip,
                    IsDelete = 0,
                    IsController = "1",
                };
                deviceList.Add(deviceObject);
            }
            var plcObject = new ZjnWcsPlcObjectEntity()
            {
                Length = plcPoint.Start + plcPoint.PackSize,
                PlcPointId = plcPoint.PlcPointId.ToString(),
                Region = plcPoint.Region,
                Timestamp = plcPoint.Timestamp,
                Db = plcPoint.Db,
                Descrip = plcPoint.Descrip,
                DeviceId = deviceId,
                PackSize = plcPoint.PackSize,
                ObjType = plcPoint.ObjType,
                ObjValue = JsonConvert.SerializeObject(obj),
                PackType = plcPoint.PackType,
                PlcId = plc.PlcId,
                Start = plcPoint.Start,
                PlcObjId = YitIdHelper.NextId().ToString()
            };
            await _zjnWcsPlcObjectRepository.AsInsertable(plcObject).ExecuteCommandAsync();
        }

        /// <summary>
        /// 初始化plc点位
        /// </summary>
        /// <returns></returns>
        [HttpGet("InitPoint")]
        [AllowAnonymous]
        [IgnoreLog]
        public async Task InitPlcPoints(string regionNo)
        {
            try
            {
                //List<ZjnWcsPlcPointEntity> plcPointList = new List<ZjnWcsPlcPointEntity>();
                var dataTypes = InitPlcDataType();
                var plcList = await _zjnWcsPlcRepository.AsQueryable().Where(s => s.IsActive)
                    .WhereIF(!string.IsNullOrEmpty(regionNo), s => s.Region == regionNo).ToArrayAsync();
                if (plcList.Length == 0) return;
                int type = 0;
                _db.BeginTran();
                foreach (var plc in plcList)
                {
                    if (plc.Caption == "1" && !plc.IsStacker)
                    {
                        type = 1;
                    }
                    else if (plc.Caption == "7" && !plc.IsStacker)
                    {
                        type = 2;
                    }
                    else if (plc.Caption == "1" && plc.IsStacker)
                    {
                        type = 3;
                    }
                    else if (plc.Caption == "7" && plc.IsStacker)
                    {
                        type = 4;
                    }
                    foreach (var dataType in dataTypes[type])
                    {
                        dataType.PlcPointId = YitIdHelper.NextId().ToString();
                        dataType.PlcId = plc.PlcId;
                        dataType.Region = plc.Region;
                        //plcPointList.Add(dataType);
                        _zjnWcsPlcPointRepository.AsInsertable(dataType).ExecuteCommand();
                    }
                }
                _db.CommitTran();
            }
            catch (Exception ex)
            {
                _db.RollbackTran();
                throw HSZException.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 点位配置生成
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, ZjnWcsPlcPointEntity[]> InitPlcDataType()
        {
            Dictionary<int, ZjnWcsPlcPointEntity[]> dic = new Dictionary<int, ZjnWcsPlcPointEntity[]>();
            dic.Add(1, new ZjnWcsPlcPointEntity[]
            {
                new ZjnWcsPlcPointEntity()
                {
                    Db=101,
                    Start=2,
                    PackSize=64,
                    ObjType="PackReadList",
                    IsList=true,
                    PackType="READ"
                },
                new ZjnWcsPlcPointEntity()
                {
                    Db=102,
                    Start=2,
                    PackSize=64,
                    ObjType="PackWriteList",
                    IsList=true,
                    PackType="WRITE"
                },
                new ZjnWcsPlcPointEntity()
                {
                    Db=100,
                    Start=0,
                    PackSize=64,
                    ObjType="PackStatusList",
                    IsList=true,
                    PackType="STATUS"
                }
            });
            dic.Add(2, new ZjnWcsPlcPointEntity[]
            {
                new ZjnWcsPlcPointEntity()
                {
                    Db=101,
                    Start=2,
                    PackSize=112,
                    ObjType="PackReadDList",
                    IsList=true,
                    PackType="READ"
                },
                new ZjnWcsPlcPointEntity()
                {
                    Db=102,
                    Start=2,
                    PackSize=112,
                    ObjType="PackWriteDList",
                    IsList=true,
                    PackType="WRITE"
                },
                new ZjnWcsPlcPointEntity()
                {
                    Db=100,
                    Start=0,
                    PackSize=112,
                    ObjType="PackStatusDList",
                    IsList=true,
                    PackType="STATUS"
                }
            });
            dic.Add(3, new ZjnWcsPlcPointEntity[]
            {
                new ZjnWcsPlcPointEntity()
                {
                    Db=3,
                    Start=0,
                    PackSize=48,
                    ObjType="PackStackerRead",
                    IsList=false,
                    Length=110,
                    PackType="READ"
                },
                new ZjnWcsPlcPointEntity()
                {
                    Db=3,
                    Start=48,
                    PackSize=62,
                    ObjType="PackStackerWrite",
                    IsList=false,
                    Length=110,
                    PackType="WRITE"
                }
            });
            dic.Add(4, new ZjnWcsPlcPointEntity[]
            {
                new ZjnWcsPlcPointEntity()
                {
                    Db=3,
                    Start=0,
                    PackSize=48,
                    ObjType="PackStackerReadD",
                    IsList=false,
                    Length=134,
                    PackType="READ"
                },
                new ZjnWcsPlcPointEntity()
                {
                    Db=3,
                    Start=48,
                    PackSize=86,
                    ObjType="PackStackerWriteD",
                    IsList=false,
                    Length=134,
                    PackType="WRITE"
                }
            });
            return dic;
        }
    }
}
