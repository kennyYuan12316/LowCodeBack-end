using HSZ.Common.Const;
using HSZ.Common.Core.Manager;
using HSZ.Entitys.wms;
using HSZ.UnifyResult;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using HSZ.wms.Interfaces.ZjnServicePathConfig;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSZ.wms.ZjnWmsTask;
using HSZ.FriendlyException;
using HSZ.Dependency;
using HSZ.wms.Interfaces.ZjnWmsTask;
using System.Runtime.InteropServices;
using HSZ.Common.TaskResultPubilcParms;
using HSZ.Wms.Interfaces.ZjnWorkProsess;
using HSZ.Common.DI;

namespace HSZ.Wms.ZjnWorkProsess
{
    /// <summary>
    /// 子任务业务处理 --- 平面设备业务
    /// </summary>
    [WareDI(WareType.Production)]
    public class ProductionPlaneDeviceProcess : IPlaneDeviceProcess, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsTaskDetailsEntity> _zjnTaskListDetailsRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        private readonly IZjnWmsTaskService _ZjnTaskService;


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="zjnTaskListDetailsRepository"></param>
        public ProductionPlaneDeviceProcess(
            IUserManager userManager,
            IZjnWmsTaskService zjnTaskService,
            ISqlSugarRepository<ZjnWmsTaskDetailsEntity> zjnTaskListDetailsRepository)
        {
            _zjnTaskListDetailsRepository = zjnTaskListDetailsRepository;
            _userManager = userManager;
            _ZjnTaskService = zjnTaskService;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }


        /// <summary>
        /// 平面设备任务
        /// </summary>
        /// <param name="WmsTaskData"></param>
        /// <param name="TaskState"></param>
        /// <returns></returns>
        public async Task<ZjnWmsTaskDetailsInfoOutput> PlaneDeviceTask(ZjnWmsTaskDetailsEntity WmsTaskData, int TaskState, TaskResultPubilcParameter parameter)
        {
            if (WmsTaskData.TaskDetailsStates == 2)//处于执行中
            {
                return await _ZjnTaskService.FinishTask(WmsTaskData, TaskState, parameter);
                //await _ZjnTaskService.ResetTask(WmsTaskData.TaskDetailsId.ToString(), TaskState);
                //return await _ZjnTaskService.GetNextTaskDetails(WmsTaskData.TaskDetailsId);
            }
            else
            {
                throw HSZException.Oh("此任务不是在执行状态,请确认任务");
            }
        }



    }
}
