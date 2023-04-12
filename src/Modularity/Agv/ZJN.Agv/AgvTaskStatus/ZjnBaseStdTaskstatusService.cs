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
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using HSZ.wms.ZjnWmsTask;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using ZJN.Agv.Entitys.Dto.AgvLimitGoods;
using ZJN.Agv.Entitys.Dto.AgvTaskStatus;
using ZJN.Agv.Entitys.Entity;
using ZJN.Agv.Interfaces;
using ZJN.Plc.PlcHelper;
//using ZJN.Wcs.Entitys.Entity.ZjnPlcDto;
//using ZJN.Wcs.Interfaces.PlcCommunication;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ZJN.Agv.AgvTaskStatus
{
    /// <summary>
    /// Agv上传任务状态服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "Agv", Name = "ZjnBaseStdTaskstatus", Order = 200)]
    [Route("api/agv/[controller]")]
    public class ZjnBaseStdTaskstatusService : IZjnBaseStdTaskstatusService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseStdTaskstatusEntity> _zjnBaseStdTaskstatusRepository;
        private readonly ISqlSugarRepository<ZjnBaseStdPdataskEntity> _pda;
        private readonly ISqlSugarRepository<ZjnWcsPlcObjectEntity> _ZjnWcsPlcObjectEntity;
        private readonly ZjnTaskService _ZjnTaskService;

        private readonly IConfiguration _configuration;

        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseStdTaskstatusService"/>类型的新实例
        /// </summary>
        public ZjnBaseStdTaskstatusService(ISqlSugarRepository<ZjnBaseStdTaskstatusEntity> zjnBaseStdTaskstatusRepository,
            ISqlSugarRepository<ZjnBaseStdPdataskEntity> pda,
            IConfiguration configuration
            , ZjnTaskService zjnTaskService
            , IUserManager userManager
            , ISqlSugarRepository<ZjnWcsPlcObjectEntity> zjnWcsPlcObjectEntity)
        {
            _zjnBaseStdTaskstatusRepository = zjnBaseStdTaskstatusRepository;
            _userManager = userManager;
            _ZjnWcsPlcObjectEntity = zjnWcsPlcObjectEntity;
            _userManager = userManager;
            _ZjnTaskService = zjnTaskService;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _pda = pda;
            _configuration = configuration;
        }

        /// <summary>
        /// 获取Agv上传任务状态
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseStdTaskstatusRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseStdTaskstatusInfoOutput>();
            //string conStr = _configuration["ConnectionStrings:DefaultConnection"];
            return output;
        }

        /// <summary>
		/// 获取Agv上传任务状态列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseStdTaskstatusListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            List<string> queryRequestTime = input.F_RequestTime != null ? input.F_RequestTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startRequestTime = queryRequestTime != null ? Ext.GetDateTime(queryRequestTime.First()) : null;
            DateTime? endRequestTime = queryRequestTime != null ? Ext.GetDateTime(queryRequestTime.Last()) : null;
            List<object> queryTaskStatus = input.F_TaskStatus != null ? input.F_TaskStatus.Split(',').ToObject<List<object>>() : null;
            var startTaskStatus = input.F_TaskStatus != null && !string.IsNullOrEmpty(queryTaskStatus.First().ToString()) ? queryTaskStatus.First() : decimal.MinValue;
            var endTaskStatus = input.F_TaskStatus != null && !string.IsNullOrEmpty(queryTaskStatus.Last().ToString()) ? queryTaskStatus.Last() : decimal.MaxValue;
            var data = await _zjnBaseStdTaskstatusRepository.AsSugarClient().Queryable<ZjnBaseStdTaskstatusEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_RequestId), a => a.RequestId.Contains(input.F_RequestId))
                .WhereIF(!string.IsNullOrEmpty(input.F_ClientCode), a => a.ClientCode.Contains(input.F_ClientCode))
                .WhereIF(queryRequestTime != null, a => a.RequestTime >= new DateTime(startRequestTime.ToDate().Year, startRequestTime.ToDate().Month, startRequestTime.ToDate().Day, startRequestTime.ToDate().Hour, startRequestTime.ToDate().Minute, startRequestTime.ToDate().Second))
                .WhereIF(queryRequestTime != null, a => a.RequestTime <= new DateTime(endRequestTime.ToDate().Year, endRequestTime.ToDate().Month, endRequestTime.ToDate().Day, endRequestTime.ToDate().Hour, endRequestTime.ToDate().Minute, endRequestTime.ToDate().Second))
                .WhereIF(!string.IsNullOrEmpty(input.F_InstanceId), a => a.InstanceId.Contains(input.F_InstanceId))
                .WhereIF(queryTaskStatus != null, a => SqlFunc.Between(a.TaskStatus, startTaskStatus, endTaskStatus))
                .WhereIF(!string.IsNullOrEmpty(input.F_AgvNum), a => a.AgvNum.Contains(input.F_AgvNum))
                .Select((a
) => new ZjnBaseStdTaskstatusListOutput
{
    F_Id = a.Id,
    F_RequestId = a.RequestId,
    F_ClientCode = a.ClientCode,
    F_ChannelId = a.ChannelId,
    F_RequestTime = a.RequestTime,
    F_InstanceId = a.InstanceId,
    F_TaskIndex = a.TaskIndex,
    F_TaskStatus = a.TaskStatus,
    F_AgvNum = a.AgvNum,
}).OrderBy(sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnBaseStdTaskstatusListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// AGV上传取料完成或放料完成等
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        [AllowAnonymous]
        public async Task<dynamic> Create([FromBody] ZjnBaseStdTaskstatusCrInput input)
        {
            RESTfulResult_AgvTaskStatus result_AgvTaskStatus = new RESTfulResult_AgvTaskStatus();
            try
            {
                //放料完成WMS需更新任务状态
                //var userInfo = await _userManager.GetUserInfo();
                var entity = input.Adapt<ZjnBaseStdTaskstatusEntity>();
                entity.Id = YitIdHelper.NextId().ToString();

                _db.BeginTran();
                var isOk = await _zjnBaseStdTaskstatusRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();


                if (!(isOk > 0))
                {
                    _db.RollbackTran();
                    //throw HSZException.Oh(ErrorCode.COM1000);
                    result_AgvTaskStatus.Code = "404";
                    result_AgvTaskStatus.Msg = ErrorCode.COM1000.ToString();
                    return result_AgvTaskStatus;
                }
                else
                {

                    result_AgvTaskStatus.Code = "200";
                    result_AgvTaskStatus.Msg = "Success";
                    //处理PLC交互
                    if (input.taskStatus == 7 || input.taskStatus == 6)
                    {
                        result_AgvTaskStatus = await HandlePlc(input);
                    }

                    if (result_AgvTaskStatus.Code == "200")
                    {
                        _db.CommitTran();
                    }

                }
            }
            catch (Exception ex)
            {
                _db.RollbackTran();
                result_AgvTaskStatus.Code = "404";
                result_AgvTaskStatus.Msg = ErrorCode.COM1000.ToString();
            }
            return result_AgvTaskStatus;
        }


        /// <summary>
        /// 新建Agv取放完成
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> HandlePlc([FromBody] ZjnBaseStdTaskstatusCrInput input)
        {
            RESTfulResult_AgvTaskStatus result_AgvLimitGoods = new RESTfulResult_AgvTaskStatus();
            List<ZjnWcsPlcObjectEntity> plcobjects = new();
            PackRead packRead = null;
            PackWrite packWrite = null;
            try
            {
                var tasklist = await RedisHelper.GetAsync<List<ZjnWmsTaskDetailsInfoOutput>>(CommonConst.CACHE_KEY_TASK_LIST);
                if (tasklist.Count == 0)
                {
                    result_AgvLimitGoods.Code = "404";
                    result_AgvLimitGoods.Msg = $"在缓存中查找不到此任务{input.instanceId}";
                    return tasklist;
                }
                var task = tasklist.Where(l => l.taskId == input.instanceId && l.taskDetailsStates == 2).First();

                //上料台放料完成获取PLC对象
                if (input.taskStatus == 6)
                {
                    plcobjects = await _ZjnWcsPlcObjectEntity.GetListAsync(l => l.DeviceId == task.taskDetailsEnd);
                }
                //下料台取料完成获取PLC对象
                if (input.taskStatus == 7)
                {
                    plcobjects = await _ZjnWcsPlcObjectEntity.GetListAsync(l => l.DeviceId == task.taskDetailsStart);
                }

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
                //取料完成判断
                if (input.taskStatus == 7)
                {
                    if (packRead.RequestPlc == 0)//&& packRead.AgvPlc == 0
                    {
                        var result = await _ZjnTaskService.TaskProcesByWcs(input.instanceId.ToInt32(), 3, null);

                        if (result.code != 200)
                        {
                            result_AgvLimitGoods.Code = "404";
                            result_AgvLimitGoods.Msg = "WMS结束任务异常";
                            return result_AgvLimitGoods;
                        }
                        if (result.data != null)
                        {
                            var resultIng = await _ZjnTaskService.WCSReceivesTheSubtaskCallback(result.data.taskDetailsId, 2);
                            if (resultIng.code != 200)
                            {
                               //
                            }
                        }
                    }

                    //packWrite.AgvWcs = 2;
                }
                else
                {
                    result_AgvLimitGoods.Code = "404";
                    result_AgvLimitGoods.Msg = "PLC不在初始状态";
                    return result_AgvLimitGoods;
                }

                //放料完成判断
                if (input.taskStatus == 6)
                {
                    //if (packWrite.AgvWcs == 0)
                    //{
                    //    packWrite.AgvWcs = 4;
                    //    packWrite.ProductWcs = task.trayNo.Substring(2, 1) switch
                    //    {
                    //        "A" => 1,
                    //        "B" => 2,
                    //        _ => 0
                    //    };
                    //    //等待WMS处理再获取新任务
                    //    var result = await _ZjnTaskService.TaskProcesByWcs(input.instanceId.ToInt32(), 3, null);
                    //    if (result.code != 200)
                    //    {
                    //        result_AgvLimitGoods.Code = "404";
                    //        result_AgvLimitGoods.Msg = "WMS结束任务异常";
                    //        zjnTaskMonitorInfoOutput.IsError = true;
                    //        zjnTaskMonitorInfoOutput.ErrorMsg = $"WMS结束任务异常{input.instanceId}";
                    //        return result_AgvLimitGoods;
                    //    }
                    //    //新任务
                    //    var taskresult = result.data;// await GetTasksAsync(result.data.taskDetailsId);
                    //    if (taskresult != null)
                    //    {
                    //        packWrite.TargetDevice = packWrite.GetS7StringToBytes(taskresult.taskDetailsEnd, 8);
                    //    }
                    //    else
                    //    {
                    //        zjnTaskMonitorInfoOutput.IsError = true;
                    //        zjnTaskMonitorInfoOutput.ErrorMsg = $"WMS结束任务成功，获取新任务失败{input.instanceId}";
                    //    }

                    //}
                    //else
                    //{
                    //    result_AgvLimitGoods.Code = "404";
                    //    result_AgvLimitGoods.Msg = "PLC不在初始状态";
                    //    zjnTaskMonitorInfoOutput.IsError = true;
                    //    zjnTaskMonitorInfoOutput.ErrorMsg = $"PLC不在初始状态{input.instanceId}";
                    //    return result_AgvLimitGoods;
                    //}
                }

                var reault = await packWrite.WriteAsync();
                await Task.Delay(1000);
                //等待PLC回复
                if (reault)
                {


                    PackRead againRead = (PackRead)packRead.Read();
                    //if (againRead.AgvPlc == packWrite.AgvWcs)
                    //{
                    //    result_AgvLimitGoods.Code = "200";
                    //    result_AgvLimitGoods.Msg = "Success";
                    //    zjnTaskMonitorInfoOutput.IsError = false;
                    //    zjnTaskMonitorInfoOutput.ErrorMsg = $"无";

                    //    packWrite.TargetDevice = new byte[8];
                    //    packWrite.ResponseWcs = 0;
                    //    packWrite.AgvWcs = 0;
                    //    reault = await packWrite.WriteAsync();
                    //    if (reault)
                    //    {
                    //        //AGV调
                    //        if (input.taskStatus == 6)
                    //        {
                    //            var result = await _ZjnTaskService.WCSReceivesTheSubtaskCallback(packRead.TaskCode.ToString(), 2);
                    //            if (result.code != 200)
                    //            {
                    //                result_AgvLimitGoods.Code = "200";
                    //                result_AgvLimitGoods.Msg = $"WMS更新下一任务失败{packRead.TaskCode}";
                    //                zjnTaskMonitorInfoOutput.IsError = true;
                    //                zjnTaskMonitorInfoOutput.ErrorMsg = $"WMS更新下一任务失败{packRead.TaskCode}";
                    //            }
                    //        }
                    //        //写入成功
                    //        result_AgvLimitGoods.Code = "200";
                    //        result_AgvLimitGoods.Msg = "Success";
                    //        zjnTaskMonitorInfoOutput.IsError = false;
                    //        zjnTaskMonitorInfoOutput.ErrorMsg = $"无";
                    //    }

                    //}
                    //else
                    //{
                    //    result_AgvLimitGoods.Code = "404";
                    //    result_AgvLimitGoods.Msg = "PLC响应失败";
                    //    zjnTaskMonitorInfoOutput.IsError = true;
                    //    zjnTaskMonitorInfoOutput.ErrorMsg = $"PLC响应失败{input.instanceId}";
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
            {
                //await RedisHelper.HSetAsync(CommonConst.CACHE_KEY_Monitor_Data, zjnTaskMonitorInfoOutput.DeviceCode, zjnTaskMonitorInfoOutput);
            }
            return result_AgvLimitGoods;
        }

        /// <summary>
        /// 更新Agv上传任务状态
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseStdTaskstatusUpInput input)
        {
            var entity = input.Adapt<ZjnBaseStdTaskstatusEntity>();
            var isOk = await _zjnBaseStdTaskstatusRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除Agv上传任务状态
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseStdTaskstatusRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnBaseStdTaskstatusRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 获取任务
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [NonAction]
        public async Task<ZjnWmsTaskDetailsInfoOutput> GetTasksAsync(string taskid)
        {
            //从redis中获取任务
            try
            {
                var taskHs = await RedisHelper.HGetAllAsync<ZjnWmsTaskDetailsInfoOutput>(CommonConst.CACHE_KEY_TASK_LIST);
                var tasklist = new List<ZjnWmsTaskDetailsInfoOutput>(taskHs.Values);

                if (tasklist.Count > 0)
                {
                    var task = tasklist.Where(l => l.taskDetailsId == taskid).FirstOrDefault();
                    if (task != null)
                    {
                        return task;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}


