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
using Microsoft.CodeAnalysis;
using HSZ.wms.Interfaces.ZjnWmsLocation;
using HSZ.Common.Extension;
using HSZ.DynamicApiController;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Microsoft.AspNetCore.Authorization;

namespace HSZ.Wms.ZjnWorkProsess
{
    /// <summary>
    /// WCS主动发起新任务
    /// </summary>
    [WareDI(WareType.RawMaterial)]
    [ApiDescriptionSettings(Tag = "wms", Name = "CreateTaskByWcsProcess", Order = 200)]
    [Route("api/wms/[controller]")]
    public class CreateTaskByWcsProcess : ICreateTaskByWcsProcess, IDynamicApiController, ITransient
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
        public CreateTaskByWcsProcess(
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
                        var tray = await _db.Queryable<ZjnWmsTrayEntity>().FirstAsync(f => f.TrayNo == input.trayNo);
                        if (tray == null || tray.TrayStates != 1)
                        {
                            throw HSZException.Oh("此托盘不存在");
                        }
                        taskInput.quantity = input.trayNum;
                        taskInput.positionFrom = input.deviceId;
                        taskInput.operationDirection = "Into";
                        await _ZjnTaskService.Create(taskInput);

                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw HSZException.Oh(ex.Message);
                    }
                #endregion
                //case TaskProcessType.EmptyTrayOut:
                //    #region 空托出库
                //    try
                //    {
                //        var processEntity = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWcsProcessConfigEntity>()
                //            .Where(x => x.WorkType == (int)TaskProcessType.EmptyTrayOut && x.WorkEnd.Contains(input.deviceId)).FirstAsync();
                //        if (processEntity == null) throw HSZException.Oh("找不到业务流程");
                //        processId = processEntity.Id;
                //        //解析数据
                //        ZjnWcsProcessConfigJsonOutput data = await _zjnServicePathConfigService.PathData(processId);
                //        ZjnWmsTaskCrInput taskInput = data.zjnTaskInfoOutput.Adapt<ZjnWmsTaskCrInput>();
                //        taskInput.taskList = data.ZjnTaskListDetailsList.Adapt<List<ZjnWmsTaskDetailsEntity>>();

                //        taskInput.taskFrom = "WCS";
                //        taskInput.TrayType = input.trayType.ToString();
                //        taskInput.quantity = input.trayNum;
                //        taskInput.positionTo = input.deviceId;
                //        taskInput.operationDirection = "Out";
                //        var locations = await _zjnWmsMaterialInventoryRepository.AsQueryable()
                //    .InnerJoin<ZjnWmsLocationEntity>((a, b) => a.ProductsLocation == b.LocationNo && a.IsDeleted == 0
                //    && b.IsDelete == 0 && b.EnabledMark == 1)
                //    .InnerJoin<ZjnWmsAisleEntity>((a, b, c) => b.AisleNo == c.AisleNo && c.EnabledMark == 1)
                //    .InnerJoin<ZjnWmsTrayEntity>((a, b, c, d) => a.ProductsContainer == d.TrayNo && d.EnabledMark == 1)
                //    .Where((a, b, c, d) => b.LocationStatus == "1" && a.ProductsCode == string.Empty
                //    && d.Type == input.trayType).OrderBy(a => a.CreateTime, OrderByType.Asc).OrderBy((a, b) => b.Depth)
                //    .Select((a, b) => b).ToListAsync();
                //        if (locations.Count == 0)
                //        {
                //            throw HSZException.Oh("暂无库存，无法出库");
                //        }
                //        //深度问题先不管
                //        taskInput.trayNo = locations[0].TrayNo;
                //        taskInput.positionFrom = locations[0].LocationNo;
                //        await _ZjnTaskService.Create(taskInput);

                //        return true;
                //    }
                //    catch (Exception)
                //    {

                //        return false;
                //    }
                //#endregion
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
        public async Task<RESTfulResult<bool>> FoldingPlateMachine(float Reserve1, string TrayCode, string DeviceCode)
        {
            RESTfulResult<bool> rESTfulResult = new RESTfulResult<bool>();
            try
            {
                WcsCreateTaskInput input = new WcsCreateTaskInput();
                input.deviceId = DeviceCode;
                input.taskProcessType = TaskProcessType.EmptyTrayInto;
                input.trayNo = TrayCode;
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
