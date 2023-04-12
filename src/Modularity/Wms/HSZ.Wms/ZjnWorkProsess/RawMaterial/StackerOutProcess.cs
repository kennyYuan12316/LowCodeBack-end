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
using HSZ.Common.Enum;
using Microsoft.CodeAnalysis;
using HSZ.Localization;

namespace HSZ.Wms.ZjnWorkProsess
{
    /// <summary>
    /// 子任务业务处理 --- 堆垛机出库业务
    /// </summary>
    [WareDI(WareType.RawMaterial)]
    public class StackerOutProcess : IStackerOutProcess, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsMaterialInventoryEntity> _zjnWmsMaterialInventoryRepository;
        private readonly ISqlSugarRepository<ZjnWmsLocationEntity> _zjnWmsLocationRepository;
        private readonly ISqlSugarRepository<ZjnWmsTaskDetailsEntity> _zjnTaskListDetailsRepository;
        private readonly CacheManager _cacheManager;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        private readonly IZjnWmsTaskService _ZjnTaskService;
        private readonly IZjnWmsLocationAutoService _zjnWmsLocationAutoService;


        /// <summary>
        /// 初始化
        /// </summary>
        public StackerOutProcess(
            IUserManager userManager,
            IZjnWmsTaskService zjnTaskService,
            ISqlSugarRepository<ZjnWmsMaterialInventoryEntity> zjnWmsMaterialInventoryRepository,
            ISqlSugarRepository<ZjnWmsLocationEntity> zjnWmsLocationRepository,
            CacheManager cacheManager,
            IZjnWmsLocationAutoService zjnWmsLocationAutoService,
            ISqlSugarRepository<ZjnWmsTaskDetailsEntity> zjnTaskListDetailsRepository)
        {
            _userManager = userManager;
            _ZjnTaskService = zjnTaskService;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _zjnWmsMaterialInventoryRepository = zjnWmsMaterialInventoryRepository;
            _zjnWmsLocationRepository = zjnWmsLocationRepository;
            _cacheManager = cacheManager;
            _zjnWmsLocationAutoService = zjnWmsLocationAutoService;
            _zjnTaskListDetailsRepository = zjnTaskListDetailsRepository;
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
                    var location = await _zjnWmsLocationRepository.GetFirstAsync(x => x.LocationNo == WmsTaskData.TaskDetailsStart);
                    if (location.Row == 1)//出深层货位后解除外层锁定
                    {
                        var outerLocationNo = $"{location.ByWarehouse}-{location.AisleNo}-002-{location.Cell.ToString("d3")}-{location.Layer.ToString("d3")}";
                        await _zjnWmsLocationAutoService.UpdateLocationStatus(outerLocationNo, Common.Enum.LocationStatus.Empty);
                    }
                    if (location.Row == 4)
                    {
                        var outerLocationNo = $"{location.ByWarehouse}-{location.AisleNo}-003-{location.Cell.ToString("d3")}-{location.Layer.ToString("d3")}";
                        await _zjnWmsLocationAutoService.UpdateLocationStatus(outerLocationNo, Common.Enum.LocationStatus.Empty);
                    }
                    await _zjnWmsLocationRepository.UpdateAsync(a => new ZjnWmsLocationEntity() { TrayNo = string.Empty }, b => b.LocationNo == WmsTaskData.TaskDetailsStart);

                    //抽检节点
                    if (WmsTaskData.TaskDetailsEnd == "A01081" && WmsTaskData.TaskType == 4)
                    {
                        var nexTask = await _zjnTaskListDetailsRepository.AsQueryable()
                            .Where(l => l.TaskDetailsStart == WmsTaskData.TaskDetailsEnd&&l.TaskDetailsStates==1&&l.TrayNo== WmsTaskData.TrayNo)
                            .FirstAsync();

                        if (nexTask != null)
                        {
                            nexTask.NodeNext = string.Empty;
                            var result = await _zjnTaskListDetailsRepository.AsUpdateable()
                                .SetColumns(l => l.NodeNext == "")
                                .Where(l => l.TaskDetailsId == nexTask.TaskDetailsId)
                                .ExecuteCommandAsync();
                            if (!(result > 0))
                                throw HSZException.Oh("数据库更新结束节点任务失败");
                        }
                    }

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
