using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S7.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZJN.Plc;
using HSZ.Common.Extension;
using HSZ.wms.Interfaces.ZjnRecordMaterialInventory;
using SqlSugar;
using HSZ.Common.Core.Manager;
using SqlSugar.IOC;
using Yitter.IdGenerator;
using HSZ.FriendlyException;
using HSZ.Logging.Attributes;
using ZJN.Plc.PlcHelper;
using static ZJN.Plc.PlcHelper.PackWriteD;

namespace HSZ.Wms.PLC
{
    /// <summary>
    /// PLC服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "PLC", Order = 200)]
    [Route("api/wms/[controller]")]
    public class PLCService : IZjnRecordMaterialInventoryService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsWorkDeviceEntity> _zjnWcsWorkDeviceInventoryRepository;
        private readonly ISqlSugarRepository<ZjnWcsPlcObjectEntity> _zjnWcsPlcObjectRepository;
        private readonly ISqlSugarRepository<ZjnWcsPlcEntity> _zjnWcsPlcRepository;
        private readonly ISqlSugarRepository<ZjnWcsPlcPointEntity> _zjnWcsPlcPoint;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

    /// <summary>
    /// 初始化一个<see cref="ZjnRecordMaterialInventoryService"/>类型的新实例
    /// </summary>
    public PLCService(ISqlSugarRepository<ZjnWcsWorkDeviceEntity> zjnWcsWorkDeviceInventoryRepository, ISqlSugarRepository<ZjnWcsPlcObjectEntity> zjnWcsPlcObjectRepository,
      ISqlSugarRepository<ZjnWcsPlcEntity> zjnWcsPlcRepository, IUserManager userManager, ISqlSugarRepository<ZjnWcsPlcPointEntity> zjnWcsPlcPoint)
    {
            _zjnWcsWorkDeviceInventoryRepository = zjnWcsWorkDeviceInventoryRepository;
            _zjnWcsPlcObjectRepository = zjnWcsPlcObjectRepository;
            _zjnWcsPlcRepository = zjnWcsPlcRepository;
            _userManager = userManager;
            _zjnWcsPlcPoint = zjnWcsPlcPoint;
        //只能作为事务处理
        _db = DbScoped.SugarScope;
    }
   
        /// <summary>
        /// 打开链接PLC
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        
        public static Device GetInfo(string ip)
        {

                //var ip = "192.168.1.35";
                //创建plc连接
                var plc = new Device(CpuType.S71500, ip,102, 0, 1, "","DDD");
                
           _= plc.StartAsync();
                //if (plc.Plc.IsConnected)
                //{
                //    var obj1 = plc.Plc.Read(DataType.DataBlock, 103, 0, VarType.Byte, 8);
                //}
                //var kk = plc.ReadDbBytes(0, 1101, 00);
                //var obj = plc.ReadDbClass("PackTestList", 101, 4, 4);
                return plc;
        }
        /// <summary>
        /// 堆垛机 获取点位
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetInfo")]
        [AllowAnonymous]
        [NonUnify]
        public async Task<dynamic> DdJiCesxt() {

            var inventoryEntityList = _zjnWcsPlcRepository.AsSugarClient().Queryable<ZjnWcsPlcEntity>().Where(p => p.Ip != null).ToList();
            List<ZjnWcsPlcPointEntity> list = new List<ZjnWcsPlcPointEntity>();
            var rgv = new PackStackerWriteD();
            foreach (var item in inventoryEntityList)
            {
                var DevicePLC = GetInfo("192.168.1.17");

                if (!DevicePLC.Plc.IsConnected)
                {
                    return null;
                }
                var List = DevicePLC.ReadDbClass("PackStackerWriteD", 51, 0, 0);
                DevicePLC.Close();
                if (List != null)
                {
                    rgv = List as PackStackerWriteD;
                }
                ZjnWcsPlcPointEntity pointEntity = new ZjnWcsPlcPointEntity();
                pointEntity.Db = rgv.Db;
                pointEntity.Caption = "堆垛机";
                pointEntity.IsActive = true;
                pointEntity.ObjValue = "0";
                pointEntity.Start = rgv.Start;
                pointEntity.Timestamp = DateTime.Now;
                pointEntity.Region = "0";
                pointEntity.PlcId = item.PlcId;
                pointEntity.PackType = rgv.PackType;
                pointEntity.PackSize = rgv.PackSize;
                pointEntity.IsList = true;
                list.Add(pointEntity);
            }
            //新增记录
            var isOk1 = await _zjnWcsPlcPoint.AsInsertable(list).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (false)
            {
                throw HSZException.Oh("点位插入失败");
            }
            //Create(list, inventoryEntityList,rgv);
            // pointEntity.PlcPointId = YitIdHelper.NextId().ToString();
            //ZjnWcsWorkDeviceEntity zjnWcsWorkDevice = new ZjnWcsWorkDeviceEntity();
            //zjnWcsWorkDevice.DeviceId = YitIdHelper.NextId().ToString();
            //zjnWcsWorkDevice.IsDelete = 0;
            //zjnWcsWorkDevice.PlcIP = rgv.PlcID;
            //zjnWcsWorkDevice.DeviceType = "1";
            //ZjnWcsPlcObjectEntity p = new ZjnWcsPlcObjectEntity();
            //p.PlcId = rgv.PlcID;
            //p.Db = rgv.Db;
            //p.Start = rgv.Start;
            //p.ObjType = rgv.ObjectType;
            //p.PackType = rgv.PackType;
            //p.PackSize = rgv.PackSize;


            //try
            //{
            //    // 开启事务
            //    _db.BeginTran();

            //    //新增记录
            //    await _zjnWcsPlcPoint.AsInsertable(pointEntity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            //    //await _zjnWcsPlcRepository.AsInsertable(zjnWcsPlc).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            //    //await _zjnWcsWorkDeviceInventoryRepository.AsInsertable(zjnWcsWorkDevice).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            //    //await _zjnWcsPlcObjectRepository.AsInsertable(p).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            //    //关闭事务
            //    _db.CommitTran();
            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}


            return null;
        }
        /// <summary>
        /// 根据ip获取PLC信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Create")]
        [AllowAnonymous]
        [IgnoreLog]
        
        public async Task<dynamic> Create(string ip=null) {

            try
            {
                List<ZjnWcsWorkDeviceEntity> lstDev = new List<ZjnWcsWorkDeviceEntity>();
                List<ZjnWcsPlcObjectEntity> lstObj = new List<ZjnWcsPlcObjectEntity>();
                var inventoryEntityList =  _zjnWcsPlcRepository.AsSugarClient().Queryable<ZjnWcsPlcEntity>().WhereIF(!string.IsNullOrEmpty(ip), a => a.Ip.Contains(ip)).ToList();
                foreach (var item in inventoryEntityList)
                {
                  var list=_zjnWcsPlcPoint.AsSugarClient().Queryable<ZjnWcsPlcPointEntity>().Where(p => p.PlcId==item.PlcId).ToList();
                    foreach (var point in list)
                    {
                        var plc = new Device(CpuType.S71500, item.Ip, 102, 0, 1, item.PlcId, item.StackerId);

                        _ = plc.StartAsync();

                        if (!plc.IsConnected)
                        {
                            return null;
                        }
                        var List =  plc.ReadDbClass(point.ObjType, point.Db, point.Start, point.ListCount);
                        plc.Close();
                        if (List != null)
                        {
                            switch (point.ObjType)
                            {

                                case "PackReadList":
                                    var obj2 = List as PackReadList;
                                    obj2.List.ToList().ForEach(x =>
                                    {
                                        this.AddListItem(x, ref lstObj, ref lstDev,point.Descrip);

                                    });
                                    break;
                                case "PackReadDList":
                                    var obj3 = List as PackReadDList;
                                    obj3.List.ToList().ForEach(x =>
                                    {
                                        this.AddListItem(x, ref lstObj, ref lstDev, point.PlcPointId, point.Descrip);
                                    });
                                    break;
                                case "PackWriteList":
                                    var obj4 = List as PackWriteList;
                                    obj4.List.ToList().ForEach(x =>
                                    {
                                        this.AddListItem(x, ref lstObj, ref lstDev, point.PlcPointId, point.Descrip);
                                    });
                                    break;
                                case "PackWriteDList":
                                    var obj5 = List as PackWriteDList;
                                    obj5.List.ToList().ForEach(x =>
                                    {
                                        this.AddListItem(x, ref lstObj, ref lstDev, point.PlcPointId, point.Descrip);
                                    });
                                    break;
                                case "PackStatusList":
                                    var obj6 = List as PackStatusList;
                                    obj6.List.ToList().ForEach(x =>
                                    {
                                        this.AddListItem(x, ref lstObj, ref lstDev, point.PlcPointId, point.Descrip);
                                    });
                                    break;
                                case "PackStatusDList":
                                    var obj7 = List as PackStatusDList;
                                    obj7.List.ToList().ForEach(x =>
                                    {
                                        this.AddListItem(x, ref lstObj, ref lstDev, point.PlcPointId, point.Descrip);
                                    });
                                    break;
                                default:
                                    var obj1 = List as PackBase;
                                    this.AddListItem(obj1, ref lstObj, ref lstDev, point.PlcPointId, point.Descrip);
                                    break;
                            }

                        }

                    }
                }

                if (lstDev.Count>0)
                {
                    _db.BeginTran();
                    //新增记录
                    await _zjnWcsWorkDeviceInventoryRepository.AsInsertable(lstDev).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
                    await _zjnWcsPlcObjectRepository.AsInsertable(lstObj).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
                    //关闭事务
                    _db.CommitTran();

                }
                return null;


            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        private void AddListItem(PackBase pack, ref List<ZjnWcsPlcObjectEntity> lstObj, ref List<ZjnWcsWorkDeviceEntity> lstDevice,string PlcPointId,string ctlType="")
        {
           
          ZjnWcsWorkDeviceEntity d = new ZjnWcsWorkDeviceEntity();
            d.DeviceId = pack.DeviceNick;
            d.IsDelete = 0;
            d.PlcID = pack.PlcID;
            d.ControllerType = ctlType;
            d.IsController = "1";
            d.Caption = "";
            d.CreateUser = "admin";//userInfo.userId;
            d.CreateTime = DateTime.Now;
            d.Descrip = "";           
            d.DeviceType = "";
            d.ModifiedTime = DateTime.Now;
            d.ModifiedUser = "admin";//userInfo.userId;
            lstDevice.Add(d);           

            ZjnWcsPlcObjectEntity p = new ZjnWcsPlcObjectEntity();
            p.PlcId = pack.PlcID;
            p.DeviceId = pack.DeviceNick;
            p.Db = pack.Db;
            p.Start = pack.Start;
            p.ObjType = pack.GetType().Name;
            p.PackType = pack.PackType;
            p.PackSize = pack.PackSize;
            p.Region = "";
            p.Timestamp = DateTime.Now;
            p.Descrip = "";
            p.PlcObjId = YitIdHelper.NextId().ToString();
            p.PlcPointId = PlcPointId;
            lstObj.Add(p);
            
        }

        /// <summary>
        /// 将对象转换为byte数组
        /// </summary>
        /// <param name="obj">被转换对象</param>
        /// <returns>转换后byte数组</returns>
        public static byte[] Object2Bytes(object obj)
        {
            byte[] buff;
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter iFormatter = new BinaryFormatter();
                iFormatter.Serialize(ms, obj);
                buff = ms.GetBuffer();
            }
            return buff;
        }

        /// <summary>
        /// 将byte数组转换成对象
        /// </summary>
        /// <param name="buff">被转换byte数组</param>
        /// <returns>转换完成后的对象</returns>
        public static object Bytes2Object(byte[] buff)
        {
            object obj;
            using (MemoryStream ms = new MemoryStream(buff))
            {
                IFormatter iFormatter = new BinaryFormatter();
                obj = iFormatter.Deserialize(ms);
            }
            return obj;
        }

    }
}
