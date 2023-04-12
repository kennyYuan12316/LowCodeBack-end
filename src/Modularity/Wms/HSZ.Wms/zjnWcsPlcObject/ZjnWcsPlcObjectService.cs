using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.Logging.Attributes;
using HSZ.UnifyResult;
using HSZ.wms.Entitys.Dto.ZjnWcsPlcObject;
using HSZ.wms.Interfaces.ZjnWcsPlcObject;
using HSZ.WorkFlow.Entitys.Model.Properties;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using S7.Net;
using SqlSugar;
using SqlSugar.IOC;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using ZJN.Plc.PlcHelper;
//using ZJN.Wcs.Interfaces.PlcCommunication;
using ErrorCode = HSZ.Common.Enum.ErrorCode;

namespace HSZ.wms.ZjnWcsPlcObject
{
    /// <summary>
    /// PLC对象表服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnWcsPlcObject", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWcsPlcObjectService : IZjnWcsPlcObjectService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsPlcObjectEntity> _zjnWcsPlcObjectRepository;
        private readonly ISqlSugarRepository<ZjnWcsPlcEntity> _zjnWcsPlcRepository;
        private readonly ISqlSugarRepository<ZjnWcsPlcPointEntity> _zjnWcsPlcPointRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        //private readonly IPlcCommunicationService _IPlcCommunicationService;

        /// <summary>
        /// 初始化一个<see cref="ZjnWcsPlcObjectService"/>类型的新实例
        /// </summary>
        public ZjnWcsPlcObjectService(ISqlSugarRepository<ZjnWcsPlcObjectEntity> zjnWcsPlcObjectRepository,
            IUserManager userManager,
            ISqlSugarRepository<ZjnWcsPlcEntity> zjnWcsPlcRepository,
            ISqlSugarRepository<ZjnWcsPlcPointEntity> zjnWcsPlcPointRepository)
        {
            _zjnWcsPlcObjectRepository = zjnWcsPlcObjectRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _zjnWcsPlcRepository = zjnWcsPlcRepository;
            _zjnWcsPlcPointRepository = zjnWcsPlcPointRepository;
        }

        /// <summary>
        /// 获取PLC对象表
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnWcsPlcObjectRepository.GetFirstAsync(p => p.PlcObjId == id)).Adapt<ZjnWcsPlcObjectInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取PLC对象表列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWcsPlcObjectListQueryInput input)
        {
            var sidx = input.sidx == null ? "PlcObjID" : input.sidx;
            List<object> queryDb = input.Db != null ? input.Db.Split(',').ToObject<List<object>>() : null;
            var startDb = input.Db != null && !string.IsNullOrEmpty(queryDb.First().ToString()) ? queryDb.First() : decimal.MinValue;
            var endDb = input.Db != null && !string.IsNullOrEmpty(queryDb.Last().ToString()) ? queryDb.Last() : decimal.MaxValue;
            var data = await _zjnWcsPlcObjectRepository.AsSugarClient().Queryable<ZjnWcsPlcObjectEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.Region), a => a.Region.Equals(input.Region))
                .WhereIF(queryDb != null, a => SqlFunc.Between(a.Db, startDb, endDb))
                .WhereIF(!string.IsNullOrEmpty(input.DeviceID), a => a.DeviceId.Contains(input.DeviceID))
                .WhereIF(!string.IsNullOrEmpty(input.PlcID), a => a.PlcId.Contains(input.PlcID))
                .WhereIF(!string.IsNullOrEmpty(input.StackerGroup), a => a.StackerGroup.Contains(input.StackerGroup))

                .Select((a
) => new ZjnWcsPlcObjectListOutput
{
    Region = a.Region,
    PlcPointID = a.PlcPointId,
    PlcID = a.PlcId,
    Db = a.Db,
    Start = a.Start,
    Length = a.Length,
    ObjType = a.ObjType,
    ObjValue = a.ObjValue,
    PackType = a.PackType,
    PackSize = a.PackSize,
    DeviceID = a.DeviceId,
    Descrip = a.Descrip,
    PlcObjID = a.PlcObjId,
    StackerGroup = a.StackerGroup,
}).OrderBy(sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnWcsPlcObjectListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建PLC对象表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWcsPlcObjectCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWcsPlcObjectEntity>();
            entity.PlcObjId = YitIdHelper.NextId().ToString();

            var isOk = await _zjnWcsPlcObjectRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新PLC对象表
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWcsPlcObjectUpInput input)
        {
            var entity = input.Adapt<ZjnWcsPlcObjectEntity>();
            var isOk = await _zjnWcsPlcObjectRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除PLC对象表
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnWcsPlcObjectRepository.GetFirstAsync(p => p.PlcObjId.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnWcsPlcObjectRepository.AsDeleteable().Where(d => d.PlcObjId == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }


        /// <summary>
        /// 获取PLC对象json数据
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("GetPlcJosnData/{id}")]
        [NonUnify]
        public async Task<RESTfulResult<dynamic>> GetPlcJosnData(string id)
        {

            RESTfulResult<dynamic> tfulResult = new RESTfulResult<dynamic>();


            var output = (await _zjnWcsPlcObjectRepository.GetListAsync(p => p.DeviceId == id)).Adapt<List<ZjnWcsPlcObjectInfoOutput>>();
            if (output == null) throw HSZException.Oh(Common.Enum.ErrorCode.COM1005);

            foreach (var item in output)
            {
                var plc = await _zjnWcsPlcRepository.GetFirstAsync(p => p.PlcId == item.plcId);
                if (plc.IsActive == false) {
                    
                    item.json = "plc连接未启用ip:" + plc.Ip;
                    continue;
                }
                ////创建plc连接
                var device = new Device((CpuType)plc.CpuType, plc.Ip, plc.Port ?? 102, Convert.ToInt16(plc.Rack ?? 0), Convert.ToInt16(plc.Sock ?? 0), plc.PlcId, plc.StackerId);
                try
                {
                    await device.StartAsync();

                }
                catch (Exception ex)
                {
                    device.Close();
                    //tfulResult.code = 500;
                    //tfulResult.data = null;
                    //tfulResult.msg = "plc连接失败!";
                    //return tfulResult;
                    item.json = "plc连接失败!ip:"+plc.Ip;
                }
                if (device.Plc.IsConnected)
                {
                    //var plcPoint = await _zjnWcsPlcObjectRepository.GetFirstAsync(w => w.PlcPointId == item.plcPointId);
                    var list = device.ReadDbClass(item.objType, item.db.Value, item.start.Value, 0);
                    try
                    {


                        Type type = Type.GetType($"ZJN.Plc.PlcHelper.{item.objType},ZJN.Plc");
                        var name = $"ZJN.Plc.PlcHelper.{item.objType}Output,ZJN.Plc";//{ item.objType}
                        Type t = Type.GetType(name);
                        var ojb1 = Activator.CreateInstance(t);
                        ojb1 = list.Adapt(ojb1, type, t);
                        item.json = ojb1;
                    }
                    catch (Exception ex)
                    {
                        device.Close();

                        //tfulResult.code = 500;
                        //tfulResult.data = null;
                        //tfulResult.msg = "数据转化出错!";
                        //return tfulResult;
                        item.json = "数据转化出错!ip:" + plc.Ip;

                    }
                    device.Close();

                }
                else
                {
                    device.Close();

                    //tfulResult.code = 500;
                    //tfulResult.data = null;
                    //tfulResult.msg = "plc连接失败!";
                    //return tfulResult;
                    item.json = "plc连接失败!ip:" + plc.Ip;

                }
                //var s7plc = _IPlcCommunicationService.GetDevice(plc.PlcId);
                //if (s7plc == null || !s7plc.IsConnected)
                //{
                //}
                //if (s7plc != null)
                //{
                //    var list = s7plc.ReadDbClass(
                //  item.objType,
                //  Convert.ToInt32(item.db ?? 0),
                //  Convert.ToInt32(item.start ?? 0),
                //  0);
                //    try
                //    {


                //    Type type = Type.GetType($"ZJN.Plc.PlcHelper.{item.objType},ZJN.Plc");
                //    var name = $"ZJN.Plc.PlcHelper.{item.objType}Output,ZJN.Plc";//{ item.objType}
                //    Type t = Type.GetType(name);
                //    var ojb1 = Activator.CreateInstance(t);
                //    ojb1 = list.Adapt(ojb1, type,t);
                //    item.json = ojb1;
                //    }
                //    catch (Exception ex)
                //    {

                //        throw;
                //    }
                //}
                ////获取所有属性。
                //PropertyInfo[] properties = type.GetProperties();

                //foreach (PropertyInfo prop in properties)
                //{
                //    var a = prop.Name;
                //    var property = list.GetType().GetProperty(prop.Name);

                //    if (prop.PropertyType.Name == "Byte[]") {
                //        //property.SetValue(list,null, null);

                //    }
                //    if (property.HasAttribute<JsonIgnoreAttribute>()) {

                //    }

                //}



            }
            tfulResult.code = 200;
            tfulResult.data = output;
            return tfulResult;
        }


        /// <summary>
        /// 修改PLC对象
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpPut("UpdatePlcData")]
        [NonUnify]
        public async Task<RESTfulResult<dynamic>> UpdatePlcData([FromBody] ZjnWcsPlcObjectInfoOutput input)
        {

            RESTfulResult<dynamic> tfulResult = new RESTfulResult<dynamic>();
            var plc = await _zjnWcsPlcRepository.GetFirstAsync(p => p.PlcId == input.plcId);
            ////创建plc连接
            var device = new Device((CpuType)plc.CpuType, plc.Ip, plc.Port ?? 102, Convert.ToInt16(plc.Rack ?? 0), Convert.ToInt16(plc.Sock ?? 0), plc.PlcId, plc.StackerId);
            try
            {
                await device.StartAsync();
                string json = Convert.ToString(input.json);

                if (device.Plc.IsConnected)
                {
                    switch (input.objType)
                    {

                        case "PackRead":
                            #region 单工位读
                            PackRead packRead = device.ReadDbClass<PackRead>(input.objType, (int)input.db, (int)input.start, 0);
                            PackReadOutput packReadOutput = json.Deserialize<PackReadOutput>();
                            packRead = packReadOutput.Adapt(packRead);
                            var resultpackRead = await packRead.WriteAsync();
                            if (resultpackRead)
                            {
                                tfulResult.code = 200;
                                tfulResult.data = resultpackRead;
                                tfulResult.msg = "修改成功";
                                return tfulResult;
                            }
                            else
                            {
                                tfulResult.code = 500;
                                tfulResult.data = resultpackRead;
                                tfulResult.msg = "修改失败";
                                return tfulResult;
                            }
                            #endregion
                            break;
                        case "PackWrite":
                            #region 单工位写
                            PackWrite packWrite = device.ReadDbClass<PackWrite>(input.objType, (int)input.db, (int)input.start, 0);
                            PackWriteOutput packWriteOutput = json.Deserialize<PackWriteOutput>();
                            packWrite = packWriteOutput.Adapt(packWrite);
                            var resultpackWrite = await packWrite.WriteAsync();
                            if (resultpackWrite)
                            {
                                tfulResult.code = 200;
                                tfulResult.data = resultpackWrite;
                                tfulResult.msg = "修改成功";
                                return tfulResult;
                            }
                            else
                            {
                                tfulResult.code = 500;
                                tfulResult.data = resultpackWrite;
                                tfulResult.msg = "修改失败";
                                return tfulResult;
                            }
                            #endregion
                            break;
                        case "PackStatus":
                            #region 单工位状态
                            PackStatus packStatus = device.ReadDbClass<PackStatus>(input.objType, (int)input.db, (int)input.start, 0);
                            PackStatusOutput packStatusOutput = json.Deserialize<PackStatusOutput>();
                            packStatus = packStatusOutput.Adapt(packStatus);
                            var resultpackStatus = await packStatus.WriteAsync();
                            if (resultpackStatus)
                            {
                                tfulResult.code = 200;
                                tfulResult.data = resultpackStatus;
                                tfulResult.msg = "修改成功";
                                return tfulResult;
                            }
                            else
                            {
                                tfulResult.code = 500;
                                tfulResult.data = resultpackStatus;
                                tfulResult.msg = "修改失败";
                                return tfulResult;
                            }
                            #endregion
                            break;
                        case "PackReadD":
                            #region 双工位读

                            PackReadD packReadD = device.ReadDbClass<PackReadD>(input.objType, (int)input.db, (int)input.start, 0);
                            PackReadDOutput packReadDOutput = json.Deserialize<PackReadDOutput>();
                            packReadD = packReadDOutput.Adapt(packReadD);
                            var resultpackReadD = await packReadD.WriteAsync();
                            if (resultpackReadD)
                            {
                                tfulResult.code = 200;
                                tfulResult.data = resultpackReadD;
                                tfulResult.msg = "修改成功";
                                return tfulResult;
                            }
                            else
                            {
                                tfulResult.code = 500;
                                tfulResult.data = resultpackReadD;
                                tfulResult.msg = "修改失败";
                                return tfulResult;
                            }
                            #endregion
                            break;
                        case "PackWriteD":
                            #region 双工位写
                            PackWriteD packWriteD = device.ReadDbClass<PackWriteD>(input.objType, (int)input.db, (int)input.start, 0);
                            PackWriteDOutput packWriteDOutput = json.Deserialize<PackWriteDOutput>();
                            packWriteD = packWriteDOutput.Adapt(packWriteD);
                            var resultpackWriteD = await packWriteD.WriteAsync();
                            if (resultpackWriteD)
                            {
                                tfulResult.code = 200;
                                tfulResult.data = resultpackWriteD;
                                tfulResult.msg = "修改成功";
                                return tfulResult;
                            }
                            else
                            {
                                tfulResult.code = 500;
                                tfulResult.data = resultpackWriteD;
                                tfulResult.msg = "修改失败";
                                return tfulResult;
                            }
                            #endregion
                            break;
                        case "PackStatusD":
                            #region 双工位状态
                            PackStatusD packStatusD = device.ReadDbClass<PackStatusD>(input.objType, (int)input.db, (int)input.start, 0);
                            PackStatusDOutput packStatusDOutput = json.Deserialize<PackStatusDOutput>();
                            packStatusD = packStatusDOutput.Adapt(packStatusD);
                            var resultpackStatusD = await packStatusD.WriteAsync();
                            if (resultpackStatusD)
                            {
                                tfulResult.code = 200;
                                tfulResult.data = resultpackStatusD;
                                tfulResult.msg = "修改成功";
                                return tfulResult;
                            }
                            else
                            {
                                tfulResult.code = 500;
                                tfulResult.data = resultpackStatusD;
                                tfulResult.msg = "修改失败";
                                return tfulResult;
                            }
                            #endregion
                            break;
                        case "PackStackerRead":
                            #region 单叉堆垛机读
                            PackStackerRead packStackerRead = device.ReadDbClass<PackStackerRead>(input.objType, (int)input.db, (int)input.start, 0);
                            PackStackerReadOutput packStackerReadOutput = json.Deserialize<PackStackerReadOutput>();
                            packStackerRead = packStackerReadOutput.Adapt(packStackerRead);
                            var resultpackStackerRead = await packStackerRead.WriteAsync();
                            if (resultpackStackerRead)
                            {
                                tfulResult.code = 200;
                                tfulResult.data = resultpackStackerRead;
                                tfulResult.msg = "修改成功";
                                return tfulResult;
                            }
                            else
                            {
                                tfulResult.code = 500;
                                tfulResult.data = resultpackStackerRead;
                                tfulResult.msg = "修改失败";
                                return tfulResult;
                            }
                            #endregion
                            break;
                        case "PackStackerWrite":
                            #region 单叉堆垛机写
                            PackStackerWrite packStackerWrite = device.ReadDbClass<PackStackerWrite>(input.objType, (int)input.db, (int)input.start, 0);
                            PackStackerWriteOutput packStackerWriteOutput = json.Deserialize<PackStackerWriteOutput>();
                            packStackerWrite = packStackerWriteOutput.Adapt(packStackerWrite);
                            var resultpackStackerWrite = await packStackerWrite.WriteAsync();
                            if (resultpackStackerWrite)
                            {
                                tfulResult.code = 200;
                                tfulResult.data = resultpackStackerWrite;
                                tfulResult.msg = "修改成功";
                                return tfulResult;
                            }
                            else
                            {
                                tfulResult.code = 500;
                                tfulResult.data = resultpackStackerWrite;
                                tfulResult.msg = "修改失败";
                                return tfulResult;
                            }
                            #endregion
                            break;
                        case "PackStackerReadD":
                            #region 双叉堆垛机读
                            PackStackerReadD packStackerReadD = device.ReadDbClass<PackStackerReadD>(input.objType, (int)input.db, (int)input.start, 0);
                            PackStackerReadDOutput packStackerReadDOutput = json.Deserialize<PackStackerReadDOutput>();
                            packStackerReadD = packStackerReadDOutput.Adapt(packStackerReadD);
                            var resultpackStackerReadD = await packStackerReadD.WriteAsync();
                            if (resultpackStackerReadD)
                            {
                                tfulResult.code = 200;
                                tfulResult.data = resultpackStackerReadD;
                                tfulResult.msg = "修改成功";
                                return tfulResult;
                            }
                            else
                            {
                                tfulResult.code = 500;
                                tfulResult.data = resultpackStackerReadD;
                                tfulResult.msg = "修改失败";
                                return tfulResult;
                            }
                            #endregion
                            break;
                        case "PackStackerWriteD":
                            #region 双叉堆垛机写
                            PackStackerWriteD packStackerWriteD = device.ReadDbClass<PackStackerWriteD>(input.objType, (int)input.db, (int)input.start, 0);
                            PackStackerWriteDOutput packStackerWriteDOutput = json.Deserialize<PackStackerWriteDOutput>();
                            packStackerWriteD = packStackerWriteDOutput.Adapt(packStackerWriteD);
                            var resultpackStackerWriteD = await packStackerWriteD.WriteAsync();
                            if (resultpackStackerWriteD)
                            {
                                tfulResult.code = 200;
                                tfulResult.data = resultpackStackerWriteD;
                                tfulResult.msg = "修改成功";
                                return tfulResult;
                            }
                            else
                            {
                                tfulResult.code = 500;
                                tfulResult.data = resultpackStackerWriteD;
                                tfulResult.msg = "修改失败";
                                return tfulResult;
                            }
                            #endregion
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    device.Close();
                    tfulResult.code = 500;
                    tfulResult.data = null;
                    tfulResult.msg = "plc连接失败";
                    //throw HSZException.Oh("plc连接失败！");
                }



            }
            catch (Exception ex)
            {
                device.Close();
                tfulResult.code = 500;
                tfulResult.data = null;
                tfulResult.msg = ex.Message;
                throw;
            }




            //var output = (await _zjnWcsPlcObjectRepository.GetListAsync(p => p.DeviceId == id)).Adapt<List<ZjnWcsPlcObjectInfoOutput>>();
            //if (output == null) throw HSZException.Oh(Common.Enum.ErrorCode.COM1005);

            //foreach (var item in output)
            //{
            //    var plc = await _zjnWcsPlcRepository.GetFirstAsync(p => p.PlcId == item.plcId);
            //    ////创建plc连接
            //    var device = new Device((CpuType)plc.CpuType, plc.Ip, plc.Port ?? 102, Convert.ToInt16(plc.Rack ?? 0), Convert.ToInt16(plc.Sock ?? 0), plc.PlcId, plc.StackerId);
            //    await device.StartAsync();
            //    if (device.Plc.IsConnected)
            //    {
            //        var plcPoint = await _zjnWcsPlcPointRepository.GetFirstAsync(w => w.PlcPointId == item.plcPointId);
            //        var list = device.ReadDbClass(item.objType, plcPoint.Db, plcPoint.Start, 0);
            //        try
            //        {


            //            Type type = Type.GetType($"ZJN.Plc.PlcHelper.{item.objType},ZJN.Plc");
            //            var name = $"ZJN.Plc.PlcHelper.{item.objType}Output,ZJN.Plc";//{ item.objType}
            //            Type t = Type.GetType(name);
            //            var ojb1 = Activator.CreateInstance(t);
            //            ojb1 = list.Adapt(ojb1, type, t);
            //            item.json = ojb1;
            //        }
            //        catch (Exception ex)
            //        {
            //            device.Close();

            //            throw;
            //        }
            //        device.Close();

            //    }
            //    else
            //    {
            //        device.Close();

            //        throw HSZException.Oh("plc连接失败！");
            //    }
            //    //var s7plc = _IPlcCommunicationService.GetDevice(plc.PlcId);
            //    //if (s7plc == null || !s7plc.IsConnected)
            //    //{
            //    //}
            //    //if (s7plc != null)
            //    //{
            //    //    var list = s7plc.ReadDbClass(
            //    //  item.objType,
            //    //  Convert.ToInt32(item.db ?? 0),
            //    //  Convert.ToInt32(item.start ?? 0),
            //    //  0);
            //    //    try
            //    //    {


            //    //    Type type = Type.GetType($"ZJN.Plc.PlcHelper.{item.objType},ZJN.Plc");
            //    //    var name = $"ZJN.Plc.PlcHelper.{item.objType}Output,ZJN.Plc";//{ item.objType}
            //    //    Type t = Type.GetType(name);
            //    //    var ojb1 = Activator.CreateInstance(t);
            //    //    ojb1 = list.Adapt(ojb1, type,t);
            //    //    item.json = ojb1;
            //    //    }
            //    //    catch (Exception ex)
            //    //    {

            //    //        throw;
            //    //    }
            //    //}
            //    ////获取所有属性。
            //    //PropertyInfo[] properties = type.GetProperties();

            //    //foreach (PropertyInfo prop in properties)
            //    //{
            //    //    var a = prop.Name;
            //    //    var property = list.GetType().GetProperty(prop.Name);

            //    //    if (prop.PropertyType.Name == "Byte[]") {
            //    //        //property.SetValue(list,null, null);

            //    //    }
            //    //    if (property.HasAttribute<JsonIgnoreAttribute>()) {

            //    //    }

            //    //}



            //}
            tfulResult.code = 200;
            tfulResult.data = null;
            return tfulResult;
        }



        /// <summary>
        /// 查询设备状态DB true占位0 false无占位1
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <returns></returns>
        [HttpGet("GetPlcStatusDB/{id}")]
        [NonUnify]
        [AllowAnonymous]
        [IgnoreLog]
        public async Task<RESTfulResult<bool>> GetPlcStatusDB(string id)
        {
            RESTfulResult<bool> tfulResult = new RESTfulResult<bool>();

            var output = (await _zjnWcsPlcObjectRepository.GetFirstAsync(p => p.DeviceId == id && p.PackType == "STATUS")).Adapt<ZjnWcsPlcObjectInfoOutput>();
            if (output == null) throw HSZException.Oh(Common.Enum.ErrorCode.COM1005);

            var plc = await _zjnWcsPlcRepository.GetFirstAsync(p => p.PlcId == output.plcId);
            ////创建plc连接
            var device = new Device((CpuType)plc.CpuType, plc.Ip, plc.Port ?? 102, Convert.ToInt16(plc.Rack ?? 0), Convert.ToInt16(plc.Sock ?? 0), plc.PlcId, plc.StackerId);
            try
            {
                await device.StartAsync();

            }
            catch (Exception ex)
            {
                device.Close();
                tfulResult.code = 500;
                tfulResult.data = false;
                tfulResult.msg = "plc连接失败!";
                return tfulResult;
            }
            if (device.Plc.IsConnected)
            {
                var plcPoint = await _zjnWcsPlcPointRepository.GetFirstAsync(w => w.PlcPointId == output.plcPointId);
                var list = device.ReadDbClass(output.objType, plcPoint.Db, plcPoint.Start, 0);
                try
                {


                    Type type = Type.GetType($"ZJN.Plc.PlcHelper.{output.objType},ZJN.Plc");
                    var name = $"ZJN.Plc.PlcHelper.{output.objType}Output,ZJN.Plc";//{ item.objType}
                    Type t = Type.GetType(name);
                    var ojb1 = Activator.CreateInstance(t);
                    ojb1 = list.Adapt(ojb1, type, t);
                    output.json = ojb1;
                    int ExistTray = ((dynamic)ojb1).ExistTray;
                    if (ExistTray == 0)
                    {
                        tfulResult.code = 200;
                        tfulResult.data = true;
                        return tfulResult;
                    }
                    else
                    {
                        tfulResult.code = 200;
                        tfulResult.data = false;
                        return tfulResult;
                    }
                }
                catch (Exception ex)
                {
                    device.Close();

                    tfulResult.code = 500;
                    tfulResult.data = false;
                    tfulResult.msg = "数据转化出错!";
                    return tfulResult;
                }
                device.Close();

            }
            else
            {
                device.Close();

                tfulResult.code = 500;
                tfulResult.data = false;
                tfulResult.msg = "plc连接失败!";
                return tfulResult;
            }





            tfulResult.code = 200;
            tfulResult.data = true;
            return tfulResult;
        }

    }
}


