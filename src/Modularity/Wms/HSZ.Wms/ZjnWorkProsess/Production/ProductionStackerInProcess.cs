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
    [WareDI(WareType.Production)]
    public class ProductionStackerInProcess : IStackerInProcess, ITransient
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
        public ProductionStackerInProcess(
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

                        //update by yml 2022-11-14
                        //用途：判断是否为空托，空托的库存物料信息为null
                        var taskDto =await _ZjnTaskService.GetZjnWmsTaskDetailsInfoOutput(WmsTaskData.TaskDetailsId);
                        var goods = new ZjnWmsGoodsEntity();

                        if ((int)taskDto.taskProcessType == 2) {
                            goods = await _zjnWmsGoodsRepository.GetFirstAsync(p => p.GoodsCode == trayGood.GoodsCode && p.IsDelete == 0);
                        }
                       



                        string CreateUser = "";
                        try
                        {
                            CreateUser = _userManager?.UserId??"";
                        }
                        catch
                        {

                            CreateUser = "WCS";
                        }

                        await _zjnWmsMaterialInventoryRepository.InsertAsync(new ZjnWmsMaterialInventoryEntity()
                        {
                            Id = YitIdHelper.NextId().ToString(),
                            ProductsContainer = WmsTaskData.TrayNo,
                            ProductsCode = trayGood?.GoodsCode??String.Empty,
                            ProductsLocation = WmsTaskData.TaskDetailsEnd,
                            ProductsName = goods?.GoodsName ?? String.Empty,
                            ProductsCustomer = goods?.CustomerId ?? String.Empty,
                            ProductsCheckType = goods?.CheckType.ToString() ?? String.Empty,
                            ProductsIsLock = 0,
                            ProductsQuantity = trayGood != null ? Convert.ToDecimal(trayGood?.Quantity) : 0,
                            ProductsTakeCount = 0,
                            ProductsState = goods?.GoodsState.ToString() ?? String.Empty,
                            CreateTime = DateTime.Now,
                            CreateUser = CreateUser,
                            LastModifyUserId=CreateUser,
                            LastModifyTime=DateTime.Now
                        });
                        await _zjnWmsTrayLocationLogRepository.InsertAsync(new ZjnWmsTrayLocationLogEntity()
                        {
                            Id = YitIdHelper.NextId().ToString(),
                            LocationNo = WmsTaskData.TaskDetailsEnd,
                            GoodsCode = trayGood?.GoodsCode ?? String.Empty,
                            TrayNo = WmsTaskData.TrayNo,
                            Quantity = trayGood?.Quantity ?? 0,
                            Unit = goods?.Unit.ToString() ?? String.Empty,
                            GoodsId = goods?.Id ?? String.Empty,
                            CreateTime = DateTime.Now,
                            CreateUser = CreateUser,
                            EnabledMark=1
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
