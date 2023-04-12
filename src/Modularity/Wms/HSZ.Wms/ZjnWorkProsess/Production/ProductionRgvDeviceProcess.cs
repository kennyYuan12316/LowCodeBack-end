using HSZ.Common.Core.Manager;
using HSZ.Entitys.wms;
using HSZ.wms.ZjnWmsTask;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSZ.Common.Const;
using HSZ.Common.Enum;
using HSZ.UnifyResult;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using Mapster;
using HSZ.FriendlyException;
using HSZ.Dependency;
using HSZ.wms.Interfaces.ZjnWmsTask;
using HSZ.Wms.Interfaces.ZjnWorkProsess;
using HSZ.Common.DI;

namespace HSZ.Wms.ZjnWorkProsess
{
    /// <summary>
    /// 子任务业务处理 --- RGV业务
    /// </summary>
    [WareDI(WareType.Production)]
    public class ProductionRgvDeviceProcess : IRgvDeviceProcess, ITransient
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
        public ProductionRgvDeviceProcess(
            IUserManager userManager,
            IZjnWmsTaskService zjnTaskService,
            ISqlSugarRepository<ZjnWmsTaskDetailsEntity> zjnTaskListDetailsRepository,
            ISqlSugarRepository<ZjnWmsTrayEntity> zjnWmsTrayRepository,
            ISqlSugarRepository<ZjnWmsTrayGoodsEntity> zjnWmsTrayGoodsRepository,
            ISqlSugarRepository<ZjnWmsGoodsWeightEntity> zjnWmsGoodsWeightRepository)
        {
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _ZjnTaskService = zjnTaskService;
            _zjnTaskListDetailsRepository = zjnTaskListDetailsRepository;
            _zjnWmsGoodsWeightRepository = zjnWmsGoodsWeightRepository;
            _zjnWmsTrayGoodsRepository = zjnWmsTrayGoodsRepository;
            _zjnWmsTrayRepository = zjnWmsTrayRepository;
        }

        /// <summary>
        /// RGV任务开始，RGV终点接驳台如果没有扫描枪，应该从这里开始分配货位
        /// </summary>
        /// <param name="taskDetail">任务编号</param>
        /// <param name="TaskState">任务状态</param>
        /// <returns></returns>
        public async Task<ZjnWmsTaskDetailsInfoOutput> RGVDetailStart(ZjnWmsTaskDetailsEntity taskDetail, int TaskState)
        {
            if (taskDetail.TaskDetailsStates == 2)
            {
                if (TaskState == 3)
                {
                    return await _ZjnTaskService.FinishTask(taskDetail, TaskState);
                }
                else
                {
                    throw HSZException.Oh("状态提交有误");
                }
            }
            else
            {
                throw HSZException.Oh("此任务不是在执行状态,请确认任务");
            }
        }
    }
}
