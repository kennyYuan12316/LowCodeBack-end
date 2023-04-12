using HSZ.Common.Const;
using HSZ.Common.Core.Manager;
using HSZ.Entitys.wms;
using HSZ.UnifyResult;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using HSZ.wms.ZjnWmsTask;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSZ.Common.TaskResultPubilcParms;
using HSZ.FriendlyException;
using HSZ.Dependency;
using HSZ.wms.Interfaces.ZjnWmsTask;
using HSZ.Wms.Interfaces.ZjnWorkProsess;
using HSZ.Common.DI;

namespace HSZ.Wms.ZjnWorkProsess
{
    /// <summary>
    /// 子任务业务处理 --- 出库台业务
    /// </summary>
    [WareDI(WareType.FrameMember)]
    public class FrameMemberWarehousOutProcess : IWarehousOutProcess, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsTaskDetailsEntity> _zjnTaskListDetailsRepository;
        private readonly ISqlSugarRepository<ZjnWmsTaskEntity> _zjnTaskListRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        private readonly IZjnWmsTaskService _ZjnTaskService;


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="zjnTaskListDetailsRepository"></param>
        public FrameMemberWarehousOutProcess(
            IUserManager userManager,
            IZjnWmsTaskService zjnTaskService,
            ISqlSugarRepository<ZjnWmsTaskEntity> zjnTaskListRepository,
            ISqlSugarRepository<ZjnWmsTaskDetailsEntity> zjnTaskListDetailsRepository)
        {
            _zjnTaskListDetailsRepository = zjnTaskListDetailsRepository;
            _zjnTaskListRepository = zjnTaskListRepository;
            _ZjnTaskService = zjnTaskService;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }


        /// <summary>
        /// 出库台任务
        /// </summary>
        /// <param name="WmsTaskData"></param>
        /// <param name="TaskState"></param>
        /// <returns></returns>
        public async Task<ZjnWmsTaskDetailsInfoOutput> OutboundDeskTask(ZjnWmsTaskDetailsEntity WmsTaskData, int TaskState)
        {
            if (WmsTaskData.TaskDetailsStates == 2)//处于执行中
            {
                //await _ZjnTaskService.ResetTask(WmsTaskData.TaskDetailsId.ToString(), TaskState);
                //return await _ZjnTaskService.GetNextTaskDetails(WmsTaskData.TaskDetailsId);
                return await _ZjnTaskService.FinishTask(WmsTaskData, TaskState);

            }
            else
            {
                throw HSZException.Oh("此任务不是在执行状态,请确认任务");
            }

        }


    }
}
