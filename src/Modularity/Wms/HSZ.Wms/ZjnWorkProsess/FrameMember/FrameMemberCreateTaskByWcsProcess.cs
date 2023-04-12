using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.UnifyResult;
using HSZ.wms.Entitys.Dto.zjnWcsProcessConfig;
using HSZ.wms.Entitys.Dto.ZjnWmsTask;
using HSZ.wms.ZjnWmsTask;
using Microsoft.AspNetCore.Mvc;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSZ.wms.Interfaces.ZjnServicePathConfig;
using Mapster;
using HSZ.Dependency;
using HSZ.wms.Interfaces.ZjnWmsTask;
using HSZ.Wms.Interfaces.ZjnWorkProsess;
using HSZ.Common.DI;
using HSZ.Common.Extension;
using Microsoft.AspNetCore.Authorization;
using HSZ.wms.Interfaces.ZjnWmsLocation;

namespace HSZ.Wms.ZjnWorkProsess
{
    /// <summary>
    /// WCS主动发起新任务
    /// </summary>
    [WareDI(WareType.FrameMember)]
    public class FrameMemberCreateTaskByWcsProcess: ICreateTaskByWcsProcess, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsTaskDetailsEntity> _zjnTaskListDetailsRepository;
        private readonly IZjnWcsProcessConfigService _zjnServicePathConfigService;
        private readonly ISqlSugarRepository<ZjnWmsTaskEntity> _zjnTaskListRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        private readonly IZjnWmsLocationAutoService _zjnWmsLocationAutoService;


        private readonly IZjnWmsTaskService _ZjnTaskService;
        private readonly IFindLocationProcess _findLocationProcess;
        private readonly ISqlSugarRepository<ZjnWmsMaterialInventoryEntity> _zjnWmsMaterialInventoryRepository;


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="zjnTaskListDetailsRepository"></param>
        public FrameMemberCreateTaskByWcsProcess(
            IUserManager userManager,
            IZjnWmsTaskService zjnTaskService,
            ISqlSugarRepository<ZjnWmsTaskEntity> zjnTaskListRepository,
            IZjnWcsProcessConfigService zjnServicePathConfigService,
            ISqlSugarRepository<ZjnWmsTaskDetailsEntity> zjnTaskListDetailsRepository,
            IFindLocationProcess findLocationProcess,
            IZjnWmsLocationAutoService zjnWmsLocationAutoService,
            ISqlSugarRepository<ZjnWmsMaterialInventoryEntity> zjnWmsMaterialInventoryRepository)
        {
            _zjnTaskListDetailsRepository = zjnTaskListDetailsRepository;
            _zjnServicePathConfigService = zjnServicePathConfigService;
            _zjnTaskListRepository = zjnTaskListRepository;
            _userManager = userManager;
            _ZjnTaskService = zjnTaskService;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _findLocationProcess = findLocationProcess;
            _zjnWmsLocationAutoService = zjnWmsLocationAutoService;
            _zjnWmsMaterialInventoryRepository = zjnWmsMaterialInventoryRepository;
        }


        /// <summary>
        /// wcs创建任务
        /// </summary>
        /// <param name="input">子任务id</param>
        /// <returns></returns>
        //[HttpPut("{id}/CancelTask")]
        [NonAction]
        public async Task<bool> WcsCreateTask(WcsCreateTaskInput input)
        {
            string processId = "";//业务流程id
            switch (input.taskProcessType)
            {
                case TaskProcessType.EmptyTrayInto:
                    #region 空托入库
                    try
                    {
                        var processEntity = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWcsProcessConfigEntity>()
                            .Where(x => x.WorkType == (int)TaskProcessType.EmptyTrayInto && x.WorkStart.Contains(input.deviceId)).FirstAsync();
                        if (processEntity == null) throw HSZException.Oh("找不到业务流程");
                        processId = processEntity.Id;
                        //解析数据
                        ZjnWcsProcessConfigJsonOutput data = await _zjnServicePathConfigService.PathData(processId);
                        ZjnWmsTaskCrInput taskInput = data.zjnTaskInfoOutput.Adapt<ZjnWmsTaskCrInput>();
                        taskInput.taskList = data.ZjnTaskListDetailsList.Adapt<List<ZjnWmsTaskDetailsEntity>>();

                        taskInput.taskFrom = "WCS";
                        taskInput.trayNo = input.trayNo;
                        taskInput.quantity = input.trayNum;
                        taskInput.positionFrom = input.deviceId;
                        taskInput.operationDirection = "Into";
                        await _ZjnTaskService.Create(taskInput);

                        return true;
                    }
                    catch (Exception)
                    {

                        return false;
                    }
                #endregion 
                case TaskProcessType.EmptyTrayOut:
                    #region 空托出库
                    try
                    {
                        var processEntity = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWcsProcessConfigEntity>().Where(x => x.WorkType == (int)TaskProcessType.EmptyTrayOut && x.WorkEnd.Contains(input.deviceId)).FirstAsync();
                        if (processEntity == null) throw HSZException.Oh("找不到业务流程");
                        processId = processEntity.Id;

                        //查找空托
                        var location = await _findLocationProcess.FindEmptyTrayLocation(input.trayType, input.deviceId);

                        _db.BeginTran();

                        await _zjnWmsLocationAutoService.UpdateLocationStatus(location.LocationNo, LocationStatus.Order);

                        //创建任务
                        ZjnWmsTaskCrInput taskInput = new ZjnWmsTaskCrInput
                        {
                            trayNo = location.TrayNo,//托盘
                            positionFrom = location.LocationNo,//起点货位
                            positionTo = input.deviceId,//终点缓存位
                            operationDirection = "Out",//出库
                            taskFrom = "WCS",
                            taskName = "空托出库",
                        };
                        //传入添加任务必须参数
                        var list = await _ZjnTaskService.CreateByConfigId(processId, taskInput);
                        _db.CommitTran();

                        return true;
                    }
                    catch (Exception)
                    {
                        _db.RollbackTran();

                        return false;
                    }
                #endregion
                default:
                    return false;
            }


        }

        /// <summary>
        /// 叠盘机产生任务函数
        /// </summary>
        /// <param name="Reserve1">数量</param>
        /// <param name="TrayCode">托盘码</param>
        /// <param name="DeviceCode">设备号</param>
        /// <returns></returns>
        [HttpPost("FoldingPlateMachine")]
        [AllowAnonymous]
        public async Task<RESTfulResult<bool>> FoldingPlateMachine(float Reserve1, string TrayCode, string DeviceCode)//,int PlcTaskNo
        {
            RESTfulResult<bool> rESTfulResult = new RESTfulResult<bool>();
            try
            {
                WcsCreateTaskInput input = new WcsCreateTaskInput();
                input.deviceId = DeviceCode;
                input.taskProcessType = TaskProcessType.EmptyTrayInto;
                input.trayNo = TrayCode;
                //input.plcTaskNo = PlcTaskNo;
                input.trayNum = Reserve1.ToInt32();
                //创建空托入库
                rESTfulResult.data = await this.WcsCreateTask(input);
                rESTfulResult.code = 200;
                return rESTfulResult;
            }
            catch (Exception ex)
            {

                rESTfulResult.code = 500;
                rESTfulResult.msg = ex.Message;
                return rESTfulResult;
            }

        }

        /// <summary>
        /// 托盘缓存区
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="trayType"></param>
        /// <returns></returns>
        [HttpPost("TrayCache")]
        [AllowAnonymous]
        public async Task<RESTfulResult<bool>> TrayCache(string deviceCode, int trayType)
        {
            RESTfulResult<bool> rESTfulResult = new RESTfulResult<bool>();
            try
            {
                WcsCreateTaskInput input = new WcsCreateTaskInput();
                input.deviceId = deviceCode;
                input.taskProcessType = TaskProcessType.EmptyTrayOut;
                input.trayType = trayType;
                //创建空托入库
                rESTfulResult.data = await this.WcsCreateTask(input);
                rESTfulResult.code = 200;
                return rESTfulResult;
            }
            catch (Exception ex)
            {

                rESTfulResult.code = 500;
                rESTfulResult.msg = ex.Message;
                return rESTfulResult;
            }
        }


    }
}
