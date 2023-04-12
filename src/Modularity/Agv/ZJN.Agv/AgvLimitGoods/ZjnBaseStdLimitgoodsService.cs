using HSZ.Common.Const;
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
using HSZ.wms.ZjnWmsTask;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S7.Net;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using ZJN.Agv.Entitys.Dto.AgvLimitGoods;
using ZJN.Agv.Entitys.Entity;
using ZJN.Agv.Interfaces;
using ZJN.Plc.PlcHelper;
//using ZJN.Wcs.Entitys.Entity.ZjnPlcDto;
//using ZJN.Wcs.Interfaces.PlcCommunication;
using ErrorCode = HSZ.Common.Enum.ErrorCode;

namespace ZJN.Agv.AgvLimitGoods
{
    /// <summary>
    /// Agv请求取放服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "Agv", Name = "ZjnBaseStdLimitgoods", Order = 200)]
    [Route("api/agv/[controller]")]
    public class ZjnBaseStdLimitgoodsService : IZjnBaseStdLimitgoodsService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsPlcObjectEntity> _ZjnWcsPlcObjectEntity;
        private readonly ISqlSugarRepository<ZjnBaseStdLimitgoodsEntity> _zjnBaseStdLimitgoodsRepository;
        private readonly IUserManager _userManager;
        private readonly ZjnTaskService _ZjnTaskService;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseStdLimitgoodsService"/>类型的新实例
        /// </summary>
        public ZjnBaseStdLimitgoodsService(ISqlSugarRepository<ZjnBaseStdLimitgoodsEntity> zjnBaseStdLimitgoodsRepository,
            IUserManager userManager, ZjnTaskService zjnTaskService
            , ISqlSugarRepository<ZjnWcsPlcObjectEntity> zjnWcsPlcObjectEntity)
        {
            _zjnBaseStdLimitgoodsRepository = zjnBaseStdLimitgoodsRepository;
            _ZjnWcsPlcObjectEntity = zjnWcsPlcObjectEntity;
            _ZjnTaskService = zjnTaskService;
            _userManager = userManager;

            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取Agv请求取放
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseStdLimitgoodsRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseStdLimitgoodsInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取Agv请求取放列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseStdLimitgoodsListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            List<object> queryTaskType = input.F_TaskType != null ? input.F_TaskType.Split(',').ToObject<List<object>>() : null;
            var startTaskType = input.F_TaskType != null && !string.IsNullOrEmpty(queryTaskType.First().ToString()) ? queryTaskType.First() : decimal.MinValue;
            var endTaskType = input.F_TaskType != null && !string.IsNullOrEmpty(queryTaskType.Last().ToString()) ? queryTaskType.Last() : decimal.MaxValue;
            List<string> queryRequestTime = input.F_RequestTime != null ? input.F_RequestTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startRequestTime = queryRequestTime != null ? Ext.GetDateTime(queryRequestTime.First()) : null;
            DateTime? endRequestTime = queryRequestTime != null ? Ext.GetDateTime(queryRequestTime.Last()) : null;
            var data = await _zjnBaseStdLimitgoodsRepository.AsSugarClient().Queryable<ZjnBaseStdLimitgoodsEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_RequestId), a => a.RequestId.Contains(input.F_RequestId))
                .WhereIF(queryTaskType != null, a => SqlFunc.Between(a.TaskType, startTaskType, endTaskType))
                .WhereIF(!string.IsNullOrEmpty(input.F_LocationCode), a => a.LocationCode.Contains(input.F_LocationCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ContainerCode), a => a.ContainerCode.Contains(input.F_ContainerCode))
                .WhereIF(queryRequestTime != null, a => a.RequestTime >= new DateTime(startRequestTime.ToDate().Year, startRequestTime.ToDate().Month, startRequestTime.ToDate().Day, startRequestTime.ToDate().Hour, startRequestTime.ToDate().Minute, startRequestTime.ToDate().Second))
                .WhereIF(queryRequestTime != null, a => a.RequestTime <= new DateTime(endRequestTime.ToDate().Year, endRequestTime.ToDate().Month, endRequestTime.ToDate().Day, endRequestTime.ToDate().Hour, endRequestTime.ToDate().Minute, endRequestTime.ToDate().Second))
                .Select((a
                ) => new ZjnBaseStdLimitgoodsListOutput
                {
                    F_Id = a.Id,
                    F_RequestId = a.RequestId,
                    F_ClientCode = a.ClientCode,
                    F_TaskType = a.TaskType,
                    F_LocationCode = a.LocationCode,
                    F_ContainerCode = a.ContainerCode,
                    F_ChannelId = a.ChannelId,
                    F_RequestTime = a.RequestTime,
                }).OrderBy(sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnBaseStdLimitgoodsListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建Agv请求取放(此处用事务处理)
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        [AllowAnonymous]
        public async Task<dynamic> Create([FromBody] ZjnBaseStdLimitgoodsCrInput input)
        {
            //通过AGV传的信息，获取下一个任务目标，给PLC传目标位置
            RESTfulResult_AgvLimitGoods result_AgvLimitGoods = new RESTfulResult_AgvLimitGoods();
            try
            {

                //var userInfo = await _userManager.GetUserInfo();
                var entity = input.Adapt<ZjnBaseStdLimitgoodsEntity>();
                entity.Id = YitIdHelper.NextId().ToString();

                _db.BeginTran();
                var isOk = await _zjnBaseStdLimitgoodsRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
                if (!(isOk > 0))
                {
                    _db.RollbackTran();
                    //throw HSZException.Oh(ErrorCode.COM1000);
                    result_AgvLimitGoods.Code = "404";
                    result_AgvLimitGoods.Msg = ErrorCode.COM1000.ToString();
                }
                else
                {
                    result_AgvLimitGoods.Code = "404";
                    result_AgvLimitGoods.Msg = "Success";
                    //处理PLC交互
                    result_AgvLimitGoods = await HandlePlc(input);
                    if (result_AgvLimitGoods.Code == "200")
                    {
                        _db.CommitTran();
                    }
                }

            }
            catch (Exception ex)
            {
                _db.RollbackTran();
                result_AgvLimitGoods.Code = "404";
                result_AgvLimitGoods.Msg = ErrorCode.COM1000.ToString();
            }
            return result_AgvLimitGoods;
        }


        /// <summary>
        /// 新建Agv请求取放
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> HandlePlc([FromBody] ZjnBaseStdLimitgoodsCrInput input)
        {
            RESTfulResult_AgvLimitGoods result_AgvLimitGoods = new RESTfulResult_AgvLimitGoods();
            PackRead packRead = new();
            PackWrite packWrite = new();
            try
            {

                //获取PLC连接通过设备号，查找PLC对象配置表
                var plcobjects = await _ZjnWcsPlcObjectEntity.GetListAsync(l => l.DeviceId == input.locationCode);
                if (plcobjects == null || plcobjects.Count == 0)
                {
                    result_AgvLimitGoods.Code = "404";
                    result_AgvLimitGoods.Msg = "未配置PLC对象信息";
                    return result_AgvLimitGoods;
                }

                if (packRead == null || packWrite == null)
                {
                    result_AgvLimitGoods.Code = "404";
                    result_AgvLimitGoods.Msg = "读取不到PLC信息";
                    return result_AgvLimitGoods;
                }
                ////取料判断
                //if (input.taskType == 1)
                //{
                //    if (packRead.RequestPlc == 0)//&& packRead.AgvPlc == 0
                //    {
                //        packWrite.AgvWcs = 1;
                //    }
                //    else
                //    {
                //        result_AgvLimitGoods.Code = "404";
                //        result_AgvLimitGoods.Msg = "PLC不在初始状态";
                //        zjnTaskMonitorInfoOutput.IsError = true;
                //        zjnTaskMonitorInfoOutput.ErrorMsg = "PLC不在初始状态";
                //        return result_AgvLimitGoods;
                //    }
                //}
                ////放料判断
                //if (input.taskType == 2)
                //{
                //    if (packRead.RequestPlc == 0 && packWrite.AgvWcs == 0)
                //    {
                //        var task=await _ZjnWcsPlcObjectEntity.AsSugarClient().Queryable<ZjnWmsTaskDetailsEntity>()
                //            .Where(l => l.TaskDetailsStart == input.locationCode && l.TaskDetailsStates < 3).FirstAsync();
                //        if (task != null)
                //            packWrite.TrayCode = packWrite.GetS7StringToBytes(task.TaskDetailsEnd, 8);
                //        packWrite.AgvWcs = 3;
                //        packWrite.TrayCode = packWrite.GetS7StringToBytes(input.containerCode, 34);
                //        packWrite.ProductWcs = input.containerCode.Substring(2, 1) switch
                //        {
                //            "A" => 1,
                //            "B" => 2
                //        };
                //    }
                //    else
                //    {
                //        result_AgvLimitGoods.Code = "404";
                //        result_AgvLimitGoods.Msg = "PLC不在初始状态";
                //        zjnTaskMonitorInfoOutput.IsError = true;
                //        zjnTaskMonitorInfoOutput.ErrorMsg = "PLC不在初始状态";
                //        return result_AgvLimitGoods;
                //    }
                //}
                
                var reault = await packWrite.WriteAsync();
                await Task.Delay(1000);
                //等待PLC回复
                if (reault)
                {
                    PackRead againRead = (PackRead)packRead.Read();
                    //if (againRead.AgvPlc == packWrite.AgvWcs)
                    //{
                    //    ////AGV反馈
                    //    //if (input.taskType == 1)
                    //    //{
                    //    //    var result = await _ZjnTaskService.WCSReceivesTheSubtaskCallback(packRead.TaskCode.ToString(), 2);
                    //    //    if (result.code != 200)
                    //    //    {
                    //    //        result_AgvLimitGoods.Code = "404";
                    //    //        result_AgvLimitGoods.Msg = $"WMS更新任务失败{packRead.TaskCode}";
                    //    //        zjnTaskMonitorInfoOutput.IsError = true;
                    //    //        zjnTaskMonitorInfoOutput.ErrorMsg = $"WMS更新任务失败{packRead.TaskCode}";
                    //    //    }

                    //    //}
                    //    //result_AgvLimitGoods.Code = "200";
                    //    //result_AgvLimitGoods.Msg = "Success";
                    //    //zjnTaskMonitorInfoOutput.IsError = false;
                    //    //zjnTaskMonitorInfoOutput.ErrorMsg = "无";
                    //    packWrite.ResponseWcs = 0;
                    //    packWrite.AgvWcs = 0;
                    //    reault = await packWrite.WriteAsync();
                    //    if (reault)
                    //    {
                    //        //写入成功
                    //        result_AgvLimitGoods.Code = "200";
                    //        result_AgvLimitGoods.Msg = "Success";
                    //        zjnTaskMonitorInfoOutput.IsError = false;
                    //        zjnTaskMonitorInfoOutput.ErrorMsg = "无";
                    //    }
                    //}
                    //else
                    //{
                    //    result_AgvLimitGoods.Code = "404";
                    //    result_AgvLimitGoods.Msg = "PLC响应失败";
                    //    zjnTaskMonitorInfoOutput.IsError = true;
                    //    zjnTaskMonitorInfoOutput.ErrorMsg = "PLC响应失败";
                    //}

                }
                else
                {
                    //写入PLC失败
                    result_AgvLimitGoods.Code = "404";
                    result_AgvLimitGoods.Msg = "写入PLC失败";
                    return result_AgvLimitGoods;
                }
                

            }
            catch (Exception)
            {
                result_AgvLimitGoods.Code = "404";
                result_AgvLimitGoods.Msg = "PLC处理失败";
            }
            finally
            {                //await RedisHelper.HSetAsync(CommonConst.CACHE_KEY_Monitor_Data, zjnTaskMonitorInfoOutput.DeviceCode, zjnTaskMonitorInfoOutput);

            }
            return result_AgvLimitGoods;
        }


        /// <summary>
        /// 更新Agv请求取放
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseStdLimitgoodsUpInput input)
        {
            var entity = input.Adapt<ZjnBaseStdLimitgoodsEntity>();
            var isOk = await _zjnBaseStdLimitgoodsRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除Agv请求取放
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseStdLimitgoodsRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnBaseStdLimitgoodsRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


