using HSZ.Common.Const;
using HSZ.Common.Core.Manager;
using HSZ.Common.TaskResultPubilcParms;
using HSZ.Entitys.wms;
using HSZ.UnifyResult;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using HSZ.wms.Interfaces.ZjnServicePathConfig;
using HSZ.wms.ZjnWmsTask;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSZ.FriendlyException;
using HSZ.Dependency;
using HSZ.wms.Interfaces.ZjnWmsTask;
using HSZ.Wms.Interfaces.ZjnWorkProsess;
using HSZ.Common.DI;

namespace HSZ.Wms.ZjnWorkProsess
{
    /// <summary>
    /// Agv调度业务
    /// </summary>
    [WareDI(WareType.RawMaterial)]
    public class AgvDetailProcess : IAgvDeviceProcess, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsTaskDetailsEntity> _zjnTaskListDetailsRepository;
        private readonly ISqlSugarRepository<ZjnWmsGoodsWeightEntity> _zjnWmsGoodsWeightRepository;
        private readonly ISqlSugarRepository<ZjnWmsTrayGoodsEntity> _zjnWmsTrayGoodsRepository;
        private readonly ISqlSugarRepository<ZjnWmsTrayEntity> _zjnWmsTrayRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        private readonly IZjnWmsTaskService _ZjnTaskService;

        /// <summary>
        /// 初始化
        /// </summary>
        public AgvDetailProcess(
            IUserManager userManager,
            IZjnWmsTaskService zjnTaskService,
            ISqlSugarRepository<ZjnWmsTaskDetailsEntity> zjnTaskListDetailsRepository,
            ISqlSugarRepository<ZjnWmsTrayEntity> zjnWmsTrayRepository,
            ISqlSugarRepository<ZjnWmsTrayGoodsEntity> zjnWmsTrayGoodsRepository,
            ISqlSugarRepository<ZjnWmsGoodsWeightEntity> zjnWmsGoodsWeightRepository)
        {
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _zjnTaskListDetailsRepository = zjnTaskListDetailsRepository;
            _zjnWmsGoodsWeightRepository = zjnWmsGoodsWeightRepository;
            _zjnWmsTrayGoodsRepository = zjnWmsTrayGoodsRepository;
            _zjnWmsTrayRepository = zjnWmsTrayRepository;
            _ZjnTaskService = zjnTaskService;
        }


        /// <summary>
        /// AGV业务处理
        /// </summary>
        /// <param name="WmsTaskData">子任务数据</param>
        /// <param name="TaskState">状态</param>
        /// <param name="parameter">重量</param>
        /// <returns></returns>
        public async Task<ZjnWmsTaskDetailsInfoOutput> AgvDetailStart(ZjnWmsTaskDetailsEntity WmsTaskData, int TaskState, TaskResultPubilcParameter parameter)
        {
            return await _ZjnTaskService.FinishTask(WmsTaskData, TaskState);
        }
    }

}
