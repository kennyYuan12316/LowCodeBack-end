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
using Microsoft.CodeAnalysis;
using HSZ.wms.Entitys.Dto.ZjnWmsTask;
using Newtonsoft.Json;
using HSZ.Common.Extension;
using System.Security.Policy;

namespace HSZ.Wms.ZjnWorkProsess
{
    /// <summary>
    /// 子任务业务处理 --- 堆垛机入库业务
    /// </summary>
    [WareDI(WareType.RawMaterial)]
    public class StackerInProcess : IStackerInProcess, ITransient
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
        public StackerInProcess(
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

                        //抽检入库后继续出库
                        if (WmsTaskData.NodeNext != string.Empty && WmsTaskData.TaskType == 5)
                        {
                            //开启事务
                            _db.BeginTran();
                            var taskOut = await _zjnTaskListDetailsRepository.AsQueryable().Where(l => l.NodeCode == WmsTaskData.NodeNext && l.TaskType == 4).FirstAsync();
                            //判断下一任务是否出库
                            if (taskOut != null)
                            {
                                var siteentity = await _zjnTaskListDetailsRepository.AsSugarClient().Queryable<ZjnWcsWorkSiteEntity>().Where(x => x.StationId == taskOut.TaskDetailsEnd).FirstAsync();
                                taskOut.TaskDetailsStart = WmsTaskData.TaskDetailsEnd;
                                taskOut.RowStart = WmsTaskData.RowEnd;
                                taskOut.CellStart = WmsTaskData.CellEnd;
                                taskOut.LayerStart = WmsTaskData.LayerEnd;
                                taskOut.RowEnd = siteentity.Row;
                                taskOut.CellEnd = siteentity.Cell;
                                taskOut.LayerEnd = siteentity.Layer;
                                var result = await _zjnTaskListDetailsRepository.UpdateAsync(taskOut);
                                if (!result)
                                    throw HSZException.Oh("数据库更新出库任务失败");
                                var redisResult = RedisHelper.HSet(CommonConst.CACHE_KEY_TASK_LIST, taskOut.TaskDetailsId, taskOut);
                                if (!redisResult)
                                    throw HSZException.Oh("Redis更新出库任务失败");
                            }
                            //直接从货位里取出产生出库任务
                            _db.CommitTran();

                        }



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
                        //实托{空托不处理}
                        //update by yml 2022-11-14
                        //用途：判断是否为空托，空托的库存物料信息为null
                        var taskDto = await _ZjnTaskService.GetZjnWmsTaskDetailsInfoOutput(WmsTaskData.TaskDetailsId);
                        var goods = new ZjnWmsGoodsEntity();

                        if ((int)taskDto.taskProcessType == 2)
                        {
                            goods = await _zjnWmsGoodsRepository.GetFirstAsync(p => p.GoodsCode == trayGood.GoodsCode && p.IsDelete == 0);
                        }
                        if (taskDto.taskProcessType == Common.Enum.TaskProcessType.Into 
                            || taskDto.taskProcessType == Common.Enum.TaskProcessType.EmptyTrayInto)
                        {
                            _db.Updateable<ZjnWcsWorkDeviceEntity>().SetColumns(x => x.JobGroup == string.Empty)
                                .Where(w => w.DeviceId == WmsTaskData2.PositionFrom);
                        }
                        else
                        {
                            _db.Updateable<ZjnWcsWorkDeviceEntity>().SetColumns(x => x.JobGroup == string.Empty)
                                .Where(w => w.DeviceId == WmsTaskData2.PositionTo);
                        }

                        string CreateUser = "";
                        try
                        {
                            CreateUser = _userManager?.UserId ?? "";
                        }
                        catch
                        {

                            CreateUser = "WCS";
                        }

                        await _zjnWmsMaterialInventoryRepository.InsertAsync(new ZjnWmsMaterialInventoryEntity()
                        {
                            Id = YitIdHelper.NextId().ToString(),
                            ProductsContainer = WmsTaskData.TrayNo,
                            ProductsCode = trayGood?.GoodsCode ?? String.Empty,
                            ProductsLocation = WmsTaskData.TaskDetailsEnd,
                            ProductsName = goods?.GoodsName ?? String.Empty,
                            ProductsCustomer = goods?.CustomerId ?? String.Empty,
                            ProductsCheckType = goods?.CheckType.ToString() ?? String.Empty,
                            ProductsIsLock = 0,
                            ProductsQuantity = trayGood != null ? Convert.ToDecimal(trayGood?.Quantity) : 0,
                            ProductsTakeCount = 0,
                            ProductsState = goods?.GoodsState.ToString() ?? String.Empty,
                            ProductsBatch = WmsTaskData2.BatchNo,
                            ProductsBill = WmsTaskData2.BillNo,
                            Expand = WmsTaskData2.OrderNo,
                            CreateTime = DateTime.Now,
                            CreateUser = CreateUser,
                            LastModifyUserId = CreateUser,
                            LastModifyTime = DateTime.Now,
                            IsDeleted = 0
                        });
                        await _zjnWmsTrayLocationLogRepository.InsertAsync(new ZjnWmsTrayLocationLogEntity()
                        {

                            Id = YitIdHelper.NextId().ToString(),
                            LocationNo = WmsTaskData.TaskDetailsEnd,
                            GoodsCode = trayGood?.GoodsCode ?? string.Empty,
                            TrayNo = WmsTaskData.TrayNo,
                            Quantity = trayGood?.Quantity ?? 0,
                            Unit = goods?.Unit.ToString() ?? string.Empty,
                            GoodsId = goods?.Id ?? string.Empty,
                            CreateTime = DateTime.Now,
                            CreateUser = CreateUser,
                            EnabledMark = 1
                        });

                        //await _zjnWmsLocationRepository.AsUpdateable()
                        //        .SetColumns(x => new ZjnWmsLocationEntity() { TrayNo = WmsTaskData.TrayNo })
                        //        .Where(x => x.LocationNo == WmsTaskData.TaskDetailsEnd).ExecuteCommandAsync();
                        //当深度货位为锁定，需要生成移库任务
                        var location = await _zjnWmsLocationRepository.GetFirstAsync(x => x.LocationNo == WmsTaskData.TaskDetailsEnd);
                        if (location.Row == 2 || location.Row == 3)
                        {
                            var row = location.Row == 2 ? "001" : "004";
                            var innerLocationNo = $"{location.ByWarehouse}-{location.AisleNo}-{row}-{location.Cell.ToString("d3")}-{location.Layer.ToString("d3")}";
                            var innerLocation = await _zjnWmsLocationRepository.GetFirstAsync(x => x.LocationNo == innerLocationNo);
                            if (innerLocation != null && innerLocation.LocationStatus == "8")
                            {
                                var moveConfigId = await _db.Queryable<ZjnWcsProcessConfigEntity>().FirstAsync(x => x.WorkType == 3);//移库
                                if (moveConfigId == null)
                                {
                                    throw HSZException.Oh("未配置移库业务流程配置");
                                }
                                ZjnWmsTaskCrInput taskInput = new ZjnWmsTaskCrInput
                                {
                                    taskName = "分配货位移库任务",
                                    positionFrom = innerLocationNo,
                                    positionCurrent = location.LocationNo,
                                    priority = 3
                                };
                                await _ZjnTaskService.CreateByConfigId(moveConfigId.Id, taskInput);
                            }
                        }
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
