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
using HSZ.Common.TaskResultPubilcParms;
using HSZ.UnifyResult;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using HSZ.FriendlyException;
using HSZ.Dependency;
using HSZ.wms.Interfaces.ZjnWmsTask;
using Yitter.IdGenerator;
using HSZ.wms.ZjnWmsLocationAuto;
using HSZ.wms.Interfaces.ZjnWmsLocation;
using HSZ.Wms.Interfaces.ZjnWorkProsess;
using HSZ.Common.DI;

namespace HSZ.Wms.ZjnWorkProsess
{
    /// <summary>
    /// 子任务业务处理 --- 堆垛机入库业务
    /// </summary>
    [WareDI(WareType.RawMaterial)]
    public class FrameMemberStackerInProcess : IStackerInProcess, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsTaskDetailsEntity> _zjnTaskListDetailsRepository;
        private readonly ISqlSugarRepository<ZjnWmsTaskEntity> _zjnTaskListRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        private readonly IZjnWmsTaskService _ZjnTaskService;
        private readonly CacheManager _cacheManager;
        private readonly ISqlSugarRepository<ZjnWmsMaterialInventoryEntity> _zjnWmsMaterialInventoryRepository;
        private readonly ISqlSugarRepository<ZjnWmsTrayGoodsEntity> _zjnWmsTrayGoodsRepository;
        private readonly ISqlSugarRepository<ZjnWmsGoodsEntity> _zjnWmsGoodsRepository;
        private readonly ISqlSugarRepository<ZjnWmsLocationEntity> _zjnWmsLocationRepository;
        private readonly ISqlSugarRepository<ZjnWmsTrayLocationLogEntity> _zjnWmsTrayLocationLogRepository;
        private readonly IZjnWmsLocationAutoService _zjnWmsLocationAutoService;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="zjnTaskListDetailsRepository"></param>
        public FrameMemberStackerInProcess(
            IUserManager userManager,
            IZjnWmsTaskService zjnTaskService,
            ISqlSugarRepository<ZjnWmsTaskEntity> zjnTaskListRepository,
            ISqlSugarRepository<ZjnWmsTaskDetailsEntity> zjnTaskListDetailsRepository,
            CacheManager cacheManager,
            ISqlSugarRepository<ZjnWmsMaterialInventoryEntity> zjnWmsMaterialInventoryRepository,
            ISqlSugarRepository<ZjnWmsTrayGoodsEntity> zjnWmsTrayGoodsRepository,
            ISqlSugarRepository<ZjnWmsGoodsEntity> zjnWmsGoodsRepository,
            ISqlSugarRepository<ZjnWmsLocationEntity> zjnWmsLocationRepository,
            ISqlSugarRepository<ZjnWmsTrayLocationLogEntity> zjnWmsTrayLocationLogRepository,
            IZjnWmsLocationAutoService zjnWmsLocationAutoService)
        {
            _zjnTaskListDetailsRepository = zjnTaskListDetailsRepository;
            _zjnTaskListRepository = zjnTaskListRepository;
            _userManager = userManager;
            _ZjnTaskService = zjnTaskService;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _cacheManager = cacheManager;
            _zjnWmsMaterialInventoryRepository = zjnWmsMaterialInventoryRepository;
            _zjnWmsTrayGoodsRepository = zjnWmsTrayGoodsRepository;
            _zjnWmsGoodsRepository = zjnWmsGoodsRepository;
            _zjnWmsLocationRepository = zjnWmsLocationRepository;
            _zjnWmsTrayLocationLogRepository = zjnWmsTrayLocationLogRepository;
            _zjnWmsLocationAutoService = zjnWmsLocationAutoService;
        }



        /// <summary>
        /// 堆垛机入库业务处理
        /// </summary>
        /// <param name="WmsTaskData">子任务号</param>
        /// <param name="TaskState">状态</param>
        /// <param name="parameter">重量</param>
        /// <returns></returns>
        public async Task singleScTask(ZjnWmsTaskDetailsEntity WmsTaskData, int TaskState, TaskResultPubilcParameter parameter)
        {

            try
            {
                //1.主任务、子任务改成完结状态  2.删除子任务缓存
                if (WmsTaskData.TaskDetailsStates == 2)
                {
                    if (TaskState == 3)
                    {
                        //开启事务
                        _db.BeginTran();

                        var WmsTaskData2 = (await _zjnTaskListRepository.GetFirstAsync(p => p.TaskNo == WmsTaskData.TaskId));
                        WmsTaskData2.TaskState = TaskState;
                        //子任务完成
                        await _ZjnTaskService.ResetTask(WmsTaskData.TaskDetailsId.ToString(), TaskState);
                        //WmsTaskData.TaskDetailsStates = TaskState;
                        //await _zjnTaskListDetailsRepository.AsUpdateable(WmsTaskData).ExecuteCommandAsync();
                        //主任务完成
                        await _zjnTaskListRepository.AsUpdateable(WmsTaskData2).ExecuteCommandAsync();
                        await _zjnWmsLocationAutoService.UpdateLocationStatus(WmsTaskData.TaskDetailsEnd, Common.Enum.LocationStatus.Full);

                        //update by yml 2022-11-12
                        //用途：绑定货位托盘
                        await _zjnWmsLocationRepository.AsUpdateable().SetColumns(l => l.TrayNo == WmsTaskData.TrayNo).Where(l => l.LocationNo == WmsTaskData.TaskDetailsEnd).ExecuteCommandAsync();

                        var trayGood = await _zjnWmsTrayGoodsRepository.GetFirstAsync(p => p.TrayNo == WmsTaskData.TrayNo && p.IsDeleted == 0);
                        var goods = await _zjnWmsGoodsRepository.GetFirstAsync(p => p.GoodsCode == trayGood.GoodsCode && p.IsDelete == 0);
                        await _zjnWmsMaterialInventoryRepository.InsertAsync(new ZjnWmsMaterialInventoryEntity()
                        {
                            Id = YitIdHelper.NextId().ToString(),
                            ProductsContainer = WmsTaskData.TrayNo,
                            ProductsCode = trayGood?.GoodsCode,
                            ProductsLocation = WmsTaskData.TaskDetailsEnd,
                            ProductsName = goods?.GoodsName,
                            ProductsCustomer = goods?.CustomerId,
                            ProductsCheckType = goods?.CheckType.ToString(),
                            ProductsIsLock = 0,
                            ProductsQuantity = trayGood != null ? Convert.ToDecimal(trayGood?.Quantity) : 1,
                            ProductsTakeCount = 0,
                            ProductsState = goods?.GoodsState.ToString(),
                            CreateTime = DateTime.Now,
                            //CreateUser = _userManager.UserId
                        });
                        await _zjnWmsTrayLocationLogRepository.InsertAsync(new ZjnWmsTrayLocationLogEntity()
                        {
                            Id = YitIdHelper.NextId().ToString(),
                            LocationNo = WmsTaskData.TaskDetailsEnd,
                            GoodsCode = trayGood?.GoodsCode,
                            TrayNo = WmsTaskData.TrayNo,
                            Quantity = trayGood?.Quantity,
                            Unit = goods?.Unit.ToString(),
                            GoodsId = goods?.Id,
                            CreateTime = DateTime.Now,
                            //CreateUser = _userManager.UserId
                        });
                        _db.CommitTran();
                        await RedisHelper.HDelAsync(CommonConst.CACHE_KEY_TASK_LIST, WmsTaskData.TaskDetailsId.ToString());
                    }
                    else
                    {
                        //待定
                    }
                }
                else
                {
                    throw HSZException.Oh("此任务不是在执行状态,请确认任务");
                }
            }
            catch (Exception ex)
            {
                _db.RollbackTran();
                throw HSZException.Oh(ex.Message);
            }
        }







    }
}
