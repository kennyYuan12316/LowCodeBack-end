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
using HSZ.FriendlyException;
using HSZ.Dependency;
using HSZ.wms.Interfaces.ZjnWmsTask;
using HSZ.wms.Interfaces.ZjnWmsLocation;
using HSZ.Wms.Interfaces.ZjnWorkProsess;
using HSZ.Common.DI;
using HSZ.Common.TaskResultPubilcParms;

namespace HSZ.Wms.ZjnWorkProsess
{
    /// <summary>
    /// 子任务业务处理 --- 堆垛机出库业务
    /// </summary>
    [WareDI(WareType.RawMaterial)]
    public class FrameMemberStackerOutProcess : IStackerOutProcess, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsMaterialInventoryEntity> _zjnWmsMaterialInventoryRepository;
        private readonly ISqlSugarRepository<ZjnWmsLocationEntity> _zjnWmsLocationRepository;
        private readonly CacheManager _cacheManager;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        private readonly IZjnWmsTaskService _ZjnTaskService;
        private readonly IZjnWmsLocationAutoService _zjnWmsLocationAutoService;


        /// <summary>
        /// 初始化
        /// </summary>
        public FrameMemberStackerOutProcess(
            IUserManager userManager,
            IZjnWmsTaskService zjnTaskService,
            ISqlSugarRepository<ZjnWmsMaterialInventoryEntity> zjnWmsMaterialInventoryRepository,
            ISqlSugarRepository<ZjnWmsLocationEntity> zjnWmsLocationRepository,
            CacheManager cacheManager,
            IZjnWmsLocationAutoService zjnWmsLocationAutoService)
        {
            _userManager = userManager;
            _ZjnTaskService = zjnTaskService;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _zjnWmsMaterialInventoryRepository = zjnWmsMaterialInventoryRepository;
            _zjnWmsLocationRepository = zjnWmsLocationRepository;
            _cacheManager = cacheManager;
            _zjnWmsLocationAutoService = zjnWmsLocationAutoService;
        }



        /// <summary>
        /// 堆垛机出库任务
        /// </summary>
        /// <param name="WmsTaskData"></param>
        /// <param name="TaskState"></param>
        /// <returns></returns>
        public async Task<ZjnWmsTaskDetailsInfoOutput> StackerOutOfStorageTask(ZjnWmsTaskDetailsEntity WmsTaskData, int TaskState, TaskResultPubilcParameter parameter)
        {

            if (WmsTaskData.TaskDetailsStates == 2)//处于执行中
            {
                //await _ZjnTaskService.ResetTask(WmsTaskData.TaskDetailsId.ToString(), TaskState);
                //return await _ZjnTaskService.GetNextTaskDetails(WmsTaskData.TaskDetailsId);
                try
                {
                    //清除库存，货位释放
                    _db.BeginTran();
                    await _zjnWmsMaterialInventoryRepository.UpdateAsync(a => new ZjnWmsMaterialInventoryEntity() { IsDeleted = 1 }, a => a.ProductsLocation == WmsTaskData.TaskDetailsStart);

                    //update by yml 2022-11-12
                    //用途：解绑货位托盘
                    await _zjnWmsLocationRepository.AsUpdateable().SetColumns(l => l.TrayNo == "").Where(l => l.LocationNo == WmsTaskData.TaskDetailsStart).ExecuteCommandAsync();

                    await _zjnWmsLocationAutoService.UpdateLocationStatus(WmsTaskData.TaskDetailsStart, Common.Enum.LocationStatus.Empty);
                    _db.CommitTran();
                }
                catch (Exception ex)
                {
                    _db.RollbackTran();
                    throw HSZException.Oh(ex.Message);
                }
                return await _ZjnTaskService.FinishTask(WmsTaskData, TaskState, parameter);
            }
            else
            {
                throw HSZException.Oh("此任务不是在执行状态,请确认任务");
            }


        }

    }
}
