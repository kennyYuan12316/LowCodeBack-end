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
using Yitter.IdGenerator;
using HSZ.Common.Extension;

namespace HSZ.Wms.ZjnWorkProsess
{
    /// <summary>
    /// WCS主动发起新任务
    /// </summary>
    [WareDI(WareType.Production)]
    public class ProductionCreateTaskByWcsProcess: ICreateTaskByWcsProcess, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsTaskDetailsEntity> _zjnTaskListDetailsRepository;
        private readonly IZjnWcsProcessConfigService _zjnServicePathConfigService;
        private readonly ISqlSugarRepository<ZjnWmsTaskEntity> _zjnTaskListRepository;
        private readonly ISqlSugarRepository<ZjnWmsTrayEntity> _zjnWmsTrayRepository;
        private readonly ISqlSugarRepository<ZjnWcsProcessConfigEntity> _zjnWcsProcessConfigEntity;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        private readonly IZjnWmsTaskService _ZjnTaskService;


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="zjnTaskListDetailsRepository"></param>
        public ProductionCreateTaskByWcsProcess(
            IUserManager userManager,
            IZjnWmsTaskService zjnTaskService,
            ISqlSugarRepository<ZjnWmsTaskEntity> zjnTaskListRepository,
            IZjnWcsProcessConfigService zjnServicePathConfigService,
            ISqlSugarRepository<ZjnWmsTaskDetailsEntity> zjnTaskListDetailsRepository,
            ISqlSugarRepository<ZjnWmsTrayEntity> zjnWmsTrayRepository,
            ISqlSugarRepository<ZjnWcsProcessConfigEntity> zjnWcsProcessConfigEntity)
        {
            _zjnTaskListDetailsRepository = zjnTaskListDetailsRepository;
            _zjnServicePathConfigService = zjnServicePathConfigService;
            _zjnTaskListRepository = zjnTaskListRepository;
            _zjnWmsTrayRepository = zjnWmsTrayRepository;
            _userManager = userManager;
            _ZjnTaskService = zjnTaskService;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _zjnWcsProcessConfigEntity = zjnWcsProcessConfigEntity;
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
                        var processEntity = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWcsProcessConfigEntity>().Where(x => x.WorkType == (int)TaskProcessType.EmptyTrayInto && x.WorkStart == input.deviceId).FirstAsync();
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
                case TaskProcessType.Into:
                    #region 入库
                    try
                    {
                        var processEntity = await _zjnWcsProcessConfigEntity.GetFirstAsync(x => x.WorkType == (int)TaskProcessType.Into && x.WorkStart.Contains(input.deviceId));
                        if (processEntity == null) throw HSZException.Oh("找不到业务流程");
                        processId = processEntity.Id;
                        //解析数据
                        ZjnWcsProcessConfigJsonOutput data = await _zjnServicePathConfigService.PathData(processId);
                        ZjnWmsTaskCrInput taskInput = data.zjnTaskInfoOutput.Adapt<ZjnWmsTaskCrInput>();
                        taskInput.taskList = data.ZjnTaskListDetailsList.Adapt<List<ZjnWmsTaskDetailsEntity>>();

                        taskInput.taskFrom = "WCS";
                        taskInput.trayNo = input.trayNo;
                        taskInput.quantity = 1;
                        taskInput.positionFrom = input.deviceId;
                        taskInput.operationDirection = "Into";
                        taskInput.productLevel = "1";
                        taskInput.materialCode = "WL-001";
                        await _ZjnTaskService.Create(taskInput);

                        return true;
                    }
                    catch (Exception)
                    {

                        return false;
                    }
                #endregion
                default:
                    return false;
            }


        }


        /// <summary>
        /// 投料口自动产生任务
        /// </summary>
        /// <param name="Reserve1">数量</param>
        /// <param name="TrayCode">托盘码</param>
        /// <param name="DeviceCode">设备号</param>
        /// <returns></returns>
        public async Task<RESTfulResult<bool>> FoldingPlateMachine(float Reserve1, string TrayCode, string DeviceCode)
        {
            RESTfulResult<bool> rESTfulResult = new RESTfulResult<bool>();
            try
            {

                WcsCreateTaskInput input = new WcsCreateTaskInput();
                input.deviceId = DeviceCode;
                input.taskProcessType = TaskProcessType.Into;
                input.trayNo = TrayCode;
                input.trayNum = Reserve1.ToInt32();

                //1.调用LES接口，确定业务流程 （实盘/ocv/空托）

                //测试阶段托盘过来，自动添加托盘信息
                var tray = await _zjnWmsTrayRepository.AsSugarClient().Queryable<ZjnWmsTrayEntity>().FirstAsync(x => x.TrayNo == TrayCode);
                if (tray == null)
                {
                    ZjnWmsTrayEntity trayEntity = new ZjnWmsTrayEntity();
                    trayEntity.Id = YitIdHelper.NextId().ToString();
                    trayEntity.TrayNo = TrayCode;
                    trayEntity.TrayName = "测试添加-" + TrayCode;
                    trayEntity.Type = 1;
                    trayEntity.CreateUser = "WCS";
                    trayEntity.CreateTime = DateTime.Now;
                    trayEntity.EnabledMark = 1;
                    trayEntity.IsDelete = 0;
                    trayEntity.TrayStates = 1;

                    await _zjnWmsTrayRepository.AsSugarClient().Insertable(trayEntity).ExecuteCommandAsync();
                }


                if (true)
                {
                    input.taskProcessType = TaskProcessType.Into;
                }
                else if (1 == 1)
                {
                    input.taskProcessType = TaskProcessType.Into;
                }
                else
                {
                    input.taskProcessType = TaskProcessType.EmptyTrayInto;
                }

                //创建入库任务
                rESTfulResult.data = await this.WcsCreateTask(input);
                if (rESTfulResult.data)
                {
                    rESTfulResult.code = 200;

                }
                else
                {
                    rESTfulResult.code = 500;

                }
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
