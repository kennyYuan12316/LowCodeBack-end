using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.UnifyResult;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using HSZ.wms.Interfaces.ZjnWmsLocation;
using HSZ.wms.Interfaces.ZjnWmsTask;
using HSZ.wms.ZjnWmsLocationAuto;
using HSZ.wms.ZjnWmsTask;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSZ.Wms.Interfaces.ZjnWorkProsess;
using HSZ.Common.DI;
using HSZ.Wms.Entitys.Dto.ZjnWmsLocation;
using HSZ.Wms.Interfaces.zjnLocationGenerator;
using Microsoft.CodeAnalysis;
using NPOI.SS.Formula.Functions;
using HSZ.Common.Extension;
using Microsoft.AspNetCore.Server.IIS.Core;
using NPOI.Util;
using HSZ.wms.Entitys.Dto.ZjnWmsTask;

namespace HSZ.Wms.ZjnWorkProsess
{
    /// <summary>
    /// 入库寻找货位类
    /// </summary>
    [ApiDescriptionSettings(Tag = "FindLocation", Name = "FindLocation", Order = 220)]
    [Route("api/[controller]")]
    [WareDI(WareType.Production)]
    public class ProductionFindLocationProcess : IFindLocationProcess, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsTaskDetailsEntity> _zjnTaskListDetailsRepository;
        private readonly ISqlSugarRepository<ZjnWmsTrayEntity> _zjnWmsTrayRepository;
        private readonly ISqlSugarRepository<ZjnWmsRoadwayInboundPlanEntity> _zjnWmsRoadwayInboundPlanRepository;
        private readonly ISqlSugarRepository<ZjnWmsAisleEntity> _zjnWmsAisleRepository;
        private readonly ISqlSugarRepository<ZjnWmsLocationGroupEntity> _zjnWmsLocationGroupRepository;
        private readonly ISqlSugarRepository<ZjnWmsLocationGroupDetailsEntity> _zjnWmsLocationGroupDetailsRepository;
        private readonly ISqlSugarRepository<ZjnWcsWorkSiteEntity> _zjnWcsWorkSiteRepository;
        private readonly ISqlSugarRepository<ZjnWmsEquipmentListEntity> _zjnWmsEquipmentListRepository;
        private readonly ISqlSugarRepository<ZjnWmsLocationEntity> _zjnWmsLocationRepository;
        private readonly ISqlSugarRepository<ZjnWmsTaskEntity> _zjnTaskListRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        private readonly IZjnWmsTaskService _ZjnTaskService;
        private readonly IZjnWmsLocationAutoService _zjnWmsLocationAutoService;
        private readonly ILocationGenerator _locationGenerator;
        private readonly ISqlSugarRepository<ZjnWmsMaterialInventoryEntity> _zjnWmsMaterialInventoryRepository;
        private readonly ISqlSugarRepository<ZjnWmsTrayGoodsEntity> _zjnWmsTrayGoodsRepository;

        private readonly ISqlSugarRepository<ZjnWcsProcessConfigEntity> _zjnWcsProcessConfigRepository;
        private readonly ISqlSugarRepository<ZjnWmsGoodsEntity> _zjnWmsGoodsRepository;

        /// <summary>
        /// 初始化
        /// </summary>
        public ProductionFindLocationProcess(
            IUserManager userManager,
            ISqlSugarRepository<ZjnWmsTaskEntity> zjnTaskListRepository,
            ISqlSugarRepository<ZjnWmsTaskDetailsEntity> zjnTaskListDetailsRepository,
            ISqlSugarRepository<ZjnWmsTrayEntity> zjnWmsTrayRepository,
            ICacheManager cacheManager,
            IZjnWmsTaskService zjnTaskService,
            ISqlSugarRepository<ZjnWmsLocationEntity> zjnWmsLocationRepository,
            ISqlSugarRepository<ZjnWmsTrayGoodsEntity> zjnWmsTrayGoodsRepository,
            ISqlSugarRepository<ZjnWmsEquipmentListEntity> zjnWmsEquipmentListRepository,
            ISqlSugarRepository<ZjnWcsWorkSiteEntity> zjnWcsWorkSiteRepository,
            ISqlSugarRepository<ZjnWmsLocationGroupEntity> zjnWmsLocationGroupRepository,
            ISqlSugarRepository<ZjnWmsLocationGroupDetailsEntity> zjnWmsLocationGroupDetailsRepository,
            ISqlSugarRepository<ZjnWmsRoadwayInboundPlanEntity> zjnWmsRoadwayInboundPlanRepository,
            ISqlSugarRepository<ZjnWmsGoodsWeightEntity> zjnWmsGoodsWeightRepository,
            ISqlSugarRepository<ZjnWmsAisleEntity> zjnWmsAisleRepository,
            IZjnWmsLocationAutoService zjnWmsLocationAutoService,
            ILocationGenerator locationGenerator,
            ISqlSugarRepository<ZjnWmsMaterialInventoryEntity> zjnWmsMaterialInventoryRepository,
            ISqlSugarRepository<ZjnWcsProcessConfigEntity> zjnWcsProcessConfigRepository,
            ISqlSugarRepository<ZjnWmsGoodsEntity> zjnWmsGoodsRepository)
        {
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _ZjnTaskService = zjnTaskService;
            _zjnTaskListRepository = zjnTaskListRepository;
            _zjnTaskListDetailsRepository = zjnTaskListDetailsRepository;
            _zjnWcsWorkSiteRepository = zjnWcsWorkSiteRepository;
            _zjnWmsRoadwayInboundPlanRepository = zjnWmsRoadwayInboundPlanRepository;
            _zjnWmsTrayRepository = zjnWmsTrayRepository;
            _zjnWmsEquipmentListRepository = zjnWmsEquipmentListRepository;
            _zjnWmsLocationRepository = zjnWmsLocationRepository;
            _cacheManager = cacheManager;
            _userManager = userManager;
            _zjnWmsAisleRepository = zjnWmsAisleRepository;
            _zjnWmsLocationGroupRepository = zjnWmsLocationGroupRepository;
            _zjnWmsLocationGroupDetailsRepository = zjnWmsLocationGroupDetailsRepository;
            _zjnWmsLocationAutoService = zjnWmsLocationAutoService;
            _locationGenerator = locationGenerator;
            _zjnWmsMaterialInventoryRepository = zjnWmsMaterialInventoryRepository;
            _zjnWmsTrayGoodsRepository = zjnWmsTrayGoodsRepository;
            _zjnWcsProcessConfigRepository = zjnWcsProcessConfigRepository;
            _zjnWmsGoodsRepository = zjnWmsGoodsRepository;
        }

        /// <summary>
        /// 分配货位测试接口(缓存，需要完成任务接口解除占用测试)
        /// </summary>
        /// <param name="productLevel">产品等级</param>
        /// <returns></returns>
        [HttpPost("WmsProductionAllot/{productLevel}")]
        public async Task WmsAllotLocationTask(string productLevel)
        {
            try
            {
                var aisleCount = _locationGenerator.ReferInfo.AisleNos.Length;
                var currentAisle = await _zjnWmsRoadwayInboundPlanRepository.AsQueryable().FirstAsync();
                if (currentAisle == null)
                {
                    throw HSZException.Oh("未配置巷道入库均衡策略");
                }
                var tmp = await _zjnWmsEquipmentListRepository.GetListAsync(x => x.Type == "2");

                var binds = await RedisHelper.GetAsync<ZjnWmsTaskDetailsInfoOutput[]>("ProductionTest");
                if (binds == null)
                {
                    var executingTasks = tmp.Select(s => new ZjnWmsTaskDetailsInfoOutput()
                    {
                        taskDetailsEnd = s.EquipmentSerialNumber,
                        taskDetailsMove = s.TheBinding
                    }).ToArray();
                    await RedisHelper.SetAsync("ProductionTest", executingTasks);
                    binds = executingTasks;
                }
                var locations = await _zjnWmsLocationRepository.AsQueryable()
                    .InnerJoin<ZjnWmsAisleEntity>((a, b) => a.AisleNo == b.AisleNo)
                        .Where(a => a.LocationStatus == "0")//空货位
                        .Where(a => a.Warning == 0)
                        .Where(a => a.EnabledMark == 1 && a.IsDelete == 0)
                        .Where((a, b) => b.EnabledMark == 1)
                        //.Where(w => w.AisleNo == currentAisle.NowroadwayGroup)
                        //.WhereIF(locationGroup != null && groupDetails.Count != 0, exp.ToExpression())
                        .PartitionBy(a => new { a.Layer }).Take(_locationGenerator.MaxPerLayer)//用PartitionBy必须Take，源码默认了Take(1)
                        .OrderBy(a => a.Cell, OrderByType.Desc)
                        .OrderBy(a => a.Priority, OrderByType.Desc)
                        .OrderBy(a => a.Layer)
                        .ToListAsync();

                var locationDic = locations.GroupBy(g => g.AisleNo).ToDictionary(k => k.Key, v => v.ToList());//直接找出所有巷道的数据
                if (locationDic.Count == 0)
                {
                    //改为缓存区任务
                    throw HSZException.Oh("缓存区任务暂未开始");
                }
                (string, string, string, string) locationRes =
                        await TestAllot(locationDic, binds, productLevel, currentAisle.NowroadwayGroup);
                _db.BeginTran();
                if (locationRes.Item1 == null)//全部巷道已有入库任务
                {
                    //改为缓存区任务
                    throw HSZException.Oh("缓存区任务暂未开始");
                }
                currentAisle.NowroadwayGroup = locationRes.Item3;
                await _zjnWmsLocationAutoService.UpdateLocationStatus(locationRes.Item1, LocationStatus.Order);
                ZjnWmsTaskDetailsInfoOutput puInfo;
                var puList = binds.Where(w => w.taskDetailsMove == currentAisle.NowroadwayGroup).OrderBy(o => o.taskDetailsEnd);
                if (locationRes.Item2 == null)
                {
                    puInfo = puList.ElementAt(0);
                    puInfo.productLevel = productLevel;
                    puInfo.taskDetailsDescribe = locationRes.Item1;
                }
                else
                {
                    puInfo = puList.ElementAt(1);
                    puInfo.productLevel = productLevel;
                    puInfo.taskDetailsDescribe = locationRes.Item2;
                    await _zjnWmsLocationAutoService.UpdateLocationStatus(locationRes.Item2, LocationStatus.Order);
                    if (locationRes.Item1 != locationRes.Item4)
                    {
                        await _zjnWmsLocationAutoService.UpdateLocationStatus(locationRes.Item4, LocationStatus.Empty);
                    }
                }
                await RedisHelper.SetAsync("ProductionTest", binds);
                await _zjnWmsRoadwayInboundPlanRepository.AsUpdateable(currentAisle).ExecuteCommandAsync();
                _db.CommitTran();
            }
            catch (Exception ex)
            {
                _db.RollbackTran();
                throw HSZException.Oh(ex.Message);
            }
        }

        private async Task<(string, string, string, string)> TestAllot(Dictionary<string, List<ZjnWmsLocationEntity>> locationDic, ZjnWmsTaskDetailsInfoOutput[] binds, string productLevel, string aisleNo)
        {
            var aisleCount = _locationGenerator.ReferInfo.AisleNos.Length;
            var aisleGroups = binds.Where(w => w.productLevel == productLevel)
                .GroupBy(g => g.taskDetailsMove).OrderBy(o => o.Key);
            foreach (var aisleGroup in aisleGroups)//找相同产品等级的入库口
            {
                if (aisleGroup.Count() == 2)//入库口已满
                {
                    continue;
                }
                else if (aisleGroup.Count() == 1)
                {
                    //优先级：相邻货位，一前一后，尽量靠近,另外货位需要重新分配
                    var firstTaskDetail = aisleGroup.First();
                    if (!locationDic.ContainsKey(firstTaskDetail.taskDetailsMove))
                    {
                        continue;
                    }
                    else
                    {
                        var firstLocation = await _zjnWmsLocationRepository.GetFirstAsync(x => x.LocationNo == firstTaskDetail.taskDetailsDescribe);
                        if (firstLocation == null) break;//有bug会进，就停止分配第2个工位
                        var locationts = locationDic[firstTaskDetail.taskDetailsMove];
                        locationts.Insert(0, firstLocation);//把单个货位分配的算上再重新分配
                        var locDic = locationts.ToDictionary(k => k.LocationNo, v => v);
                        string nearLocationNo = string.Empty;
                        foreach (var item in locationts)
                        {
                            //原有堆垛机任务也需变更，入库口任务不变
                            nearLocationNo = $"{item.ByWarehouse}-{item.AisleNo}-{item.Row.ToString("d3")}-{(item.Cell - 1).ToString("d3")}-{item.Layer.ToString("d3")}";
                            if (locDic.ContainsKey(nearLocationNo))
                            {
                                return (item.LocationNo, nearLocationNo, firstTaskDetail.taskDetailsMove, firstLocation.LocationNo);
                            }
                            else if (item.Row == 1)
                            {
                                nearLocationNo = $"{item.ByWarehouse}-{item.AisleNo}-002-{(item.Cell - 1).ToString("d3")}-{item.Layer.ToString("d3")}";
                                if (locDic.ContainsKey(nearLocationNo))
                                {
                                    return (item.LocationNo, nearLocationNo, firstTaskDetail.taskDetailsMove, firstLocation.LocationNo);
                                }
                            }
                            else if (item.Row == 2)
                            {
                                nearLocationNo = $"{item.ByWarehouse}-{item.AisleNo}-001-{(item.Cell - 1).ToString("d3")}-{item.Layer.ToString("d3")}";
                                if (locDic.ContainsKey(nearLocationNo))
                                {
                                    return (item.LocationNo, nearLocationNo, firstTaskDetail.taskDetailsMove, firstLocation.LocationNo);
                                }
                            }
                        }
                        return (locationts[0].LocationNo, locationts[1].LocationNo, firstTaskDetail.taskDetailsMove, firstLocation.LocationNo);//相邻的直接取第一个,原有任务也需变更
                    }
                }
            }
            int num = Convert.ToInt32(aisleNo);
            int cur = num + 1;
            while (cur != num)
            {
                aisleNo = cur.ToString("d3");
                if (!locationDic.ContainsKey(aisleNo) || (binds.Any(a => a.taskDetailsMove == aisleNo
                && !string.IsNullOrEmpty(a.productLevel))))//所有占位的入库口直接排除
                {
                    if (cur >= aisleCount)
                    {
                        cur = 1;
                    }
                    else
                    {
                        cur++;
                    }
                    continue;
                }
                var locations = locationDic[aisleNo];
                if (cur >= aisleCount)
                {
                    cur = 1;
                }
                else
                {
                    cur++;
                }
                return (locations[0].LocationNo, null, aisleNo, null);
            }
            return (null, null, null, null);//分配失败分配缓存位
        }

        /// <summary>
        /// 模拟堆垛机任务完成
        /// </summary>
        /// <param name="aisleNo">巷道号</param>
        /// <returns></returns>
        [HttpPost("TaskCompleted/{aisleNo}")]
        public async Task TaskCompleted(string aisleNo)
        {
            var binds = await RedisHelper.GetAsync<ZjnWmsTaskDetailsInfoOutput[]>("ProductionTest");
            var aisleBinds = binds.Where(w => w.taskDetailsMove == aisleNo);
            foreach (var aisleBind in aisleBinds)
            {
                aisleBind.productLevel = null;
                aisleBind.taskDetailsDescribe = null;
            }
            await RedisHelper.SetAsync("ProductionTest", binds);
        }

        /// <summary>
        /// 分配货位并生成子任务3.0
        /// </summary>
        /// <param name="dto">子任务信息</param>
        /// <returns></returns>
        [NonAction]
        public async Task<ZjnWmsTaskDetailsInfoOutput> WmsAllotLocationTask(ZjnWmsTaskDetailsInfoOutput dto)
        {
            try
            {
                bool istrue = true;//标识,同等级电芯货位不更新更新入库策略
                bool isConflict = false;//标识，记录策略巷道是否和禁用冲突（冲突巷道不按策略+1，按实际巷道+1）
                ZjnWmsTaskDetailsInfoOutput outs = new ZjnWmsTaskDetailsInfoOutput();
                ZjnWmsLocationEntity temps = new ZjnWmsLocationEntity();
                string locationNum = null;//货位
                string Aisle = null;//巷道
                string Rukutai = null;//入库台


                #region//前置必要条件判断
                //可用巷道
                var AisleList = await _zjnWmsAisleRepository.GetListAsync(x => x.EnabledMark == 1 && x.IsDelete == 0);
                if (AisleList == null || AisleList.Count == 0)
                {
                    throw HSZException.Oh("所有巷道被禁用，无法分配货位");
                }
                //托盘数据校验
                var tray = await _zjnWmsTrayRepository.GetFirstAsync(w => w.TrayNo == dto.trayNo && w.IsDelete == 0);
                if (tray == null)
                {
                    throw HSZException.Oh("未找到托盘数据，无法分配货位");
                }
                //均衡策略校验
                var currentAisle = await _zjnWmsRoadwayInboundPlanRepository.AsQueryable().FirstAsync();
                if (currentAisle == null)
                {
                    throw HSZException.Oh("未配置巷道入库均衡策略");
                }
                #endregion

                #region //同等级电芯优先考虑
                //根据产品等级筛相同电芯等级的任务
                List<ZjnWmsTaskDetailsEntity> detailList = await _zjnTaskListDetailsRepository.AsQueryable().Where(a => a.TaskDetailsStates < 3 && a.ProductLevel == dto.productLevel && a.TaskType == 7).OrderBy(a => a.TaskDetailsId, OrderByType.Desc).ToListAsync();
                if (detailList.Count > 1)
                {
                    var productList = detailList.Where(a => a.TaskDetailsId != detailList[0].TaskDetailsId);//需要剔除当前事务中的任务
                    foreach (var item in productList)
                    {
                        //当前只有一条入库台任务
                        var taskList = await _zjnTaskListDetailsRepository.GetListAsync(a => a.TaskDetailsStates < 3 && a.TaskDetailsEnd == item.TaskDetailsEnd && a.TaskType == 7);
                        if (taskList.Count == 1)
                        {
                            //同等级第一条任务的堆垛机任务
                            ZjnWmsTaskDetailsEntity taskEnd = await _zjnTaskListDetailsRepository.GetFirstAsync(a => a.NodeCode == taskList[0].NodeNext);
                            if (string.IsNullOrEmpty(taskEnd.TaskDetailsEnd))
                            {
                                throw HSZException.Oh("同等级电芯货位寻找失败，参考原因：未找到第一条任务的货位");
                            }
                            ZjnWmsLocationEntity loction = await _zjnWmsLocationRepository.GetFirstAsync(a => a.LocationNo == taskEnd.TaskDetailsEnd);
                            //同等级第一个任务的行列层 ZHWH-W3-001-001-003-016
                            Aisle = loction.AisleNo;
                            int Row = loction.Row;
                            int Cell = loction.Cell;
                            int Layer = loction.Layer;
                            var check = AisleList.Where(a => a.AisleNo == Aisle);
                            if (check == null)
                            {
                                continue;
                            }
                            //找出当前巷道所有匹配条件的货位
                            var locationList = await _zjnWmsLocationRepository.AsQueryable()
                            .Where(a => a.LocationStatus == "0" && a.EnabledMark == 1 && a.IsDelete == 0 && a.Warning == 0 && a.AisleNo == Aisle)
                            .OrderBy(a => a.Row).OrderBy(a => a.Layer).OrderBy(a => a.Cell, OrderByType.Desc).ToListAsync();
                            if (locationList == null || locationList.Count == 0)
                            {
                                continue;
                            }
                            temps = locationList.Where(a => a.Row == Row && a.Layer == Layer).Where(a => a.Cell == (Cell - 1)).FirstOrDefault();//相邻左
                            if (temps == null)
                            {
                                temps = locationList.Where(a => a.Row == Row && a.Layer == Layer).Where(a => a.Cell == (Cell + 1)).FirstOrDefault();//相邻右
                                if (temps == null)
                                {
                                    temps = locationList.Where(a => a.Row == Row && a.Layer == Layer).FirstOrDefault();//同行同层
                                    if (temps == null)
                                    {
                                        temps = locationList.Where(a => a.Row == Row).FirstOrDefault();//同行
                                        if (temps == null)
                                        {
                                            temps = locationList[0];//同巷道
                                        }
                                    }
                                }
                            }
                            if (temps != null)
                            {
                                locationNum = temps.LocationNo;
                                Rukutai = taskList[0].TaskDetailsEnd;
                                istrue = false;
                                break;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                #endregion

                #region //按照均衡策略或空闲入库台找货位
                int isreg = 0;
                if (temps.LocationNo == null)
                {
                    //检验启用巷道和均衡策略是否冲突  用isreg代号表示 1.巷道冲突  2.巷道不冲突，任务已满 3.巷道不冲突，任务未满，货位找不到或者货位已满
                    ZjnWmsAisleEntity ishave = AisleList.Where(a => a.AisleNo == currentAisle.NowroadwayGroup).FirstOrDefault();
                    var tempAisle = "";
                    if (ishave?.AisleNo != null)
                    {
                        tempAisle = ishave.AisleNo;
                        //1.查询该巷道所绑定的入库台
                        ZjnWmsEquipmentListEntity bind = await _zjnWmsEquipmentListRepository.GetFirstAsync(x => x.TheBinding == tempAisle && x.Type == "2");
                        if (bind == null)
                        {
                            throw HSZException.Oh("入库台存在未绑定的巷道，无法分配货位");
                        }

                        //2.校验入库台任务是否达到极限
                        List<ZjnWmsTaskDetailsEntity> TaskCountList = await _zjnTaskListDetailsRepository.GetListAsync(a => a.TaskDetailsStates < 3 && a.TaskDetailsEnd == bind.EquipmentSerialNumber && a.TaskType == 7);
                        if (TaskCountList == null || TaskCountList.Count < 2)
                        {
                            //寻找货位
                            var findLocations = await _zjnWmsLocationRepository.AsQueryable()
                            .Where(a => a.LocationStatus == "0" && a.EnabledMark == 1 && a.IsDelete == 0 && a.Warning == 0 && a.AisleNo == tempAisle)
                            .OrderBy(a => a.Row).OrderBy(a => a.Layer).OrderBy(a => a.Cell, OrderByType.Desc).FirstAsync();
                            if (findLocations != null && findLocations.LocationNo != null)
                            {
                                temps = findLocations;
                                locationNum = findLocations.LocationNo;
                                Aisle = tempAisle;
                                Rukutai = bind.EquipmentSerialNumber;
                            }
                            else
                            {
                                isreg = 3;
                            }
                        }
                        else
                        {
                            isreg = 2;
                        }
                    }
                    else
                    {
                        isreg = 1;
                    }


                    //当前巷道均衡策略和禁用巷道产生冲突、策略巷道找不到货位、货位已满 会进这里
                    if (temps.LocationNo == null)
                    {
                        List<ZjnWmsAisleEntity> aisleList = new List<ZjnWmsAisleEntity>();
                        if (isreg == 1)//剔除无法找到货位的巷道
                        {
                            aisleList = AisleList.Where(a => a.AisleNo != currentAisle.NowroadwayGroup).OrderBy(a => a.AisleNo).ToList();
                            if (aisleList == null || aisleList.Count == 0)
                            {
                                throw HSZException.Oh("所有巷道被禁用，无法分配货位");
                            }
                        }
                        else
                        {
                            aisleList = AisleList;
                        }

                        foreach (var item in aisleList)
                        {
                            //1.查询该巷道所绑定的入库台
                            ZjnWmsEquipmentListEntity case1 = await _zjnWmsEquipmentListRepository.GetFirstAsync(x => x.TheBinding == item.AisleNo && x.Type == "2");
                            if (case1 == null)
                            {
                                throw HSZException.Oh("入库台存在未绑定的巷道(" + item.AisleNo + ")，无法分配货位");
                            }
                            string itemAisle = item.AisleNo;
                            string itemRukutai = case1.EquipmentSerialNumber;

                            //寻找货位
                            var findLocations = await _zjnWmsLocationRepository.AsQueryable()
                            .Where(a => a.LocationStatus == "0" && a.EnabledMark == 1 && a.IsDelete == 0 && a.Warning == 0 && a.AisleNo == itemAisle)
                            .OrderBy(a => a.Row).OrderBy(a => a.Layer).OrderBy(a => a.Cell, OrderByType.Desc).FirstAsync();
                            if (findLocations == null)
                            {
                                continue;
                            }

                            if (isreg == 2 && aisleList.Count == 1)
                            {
                                temps = findLocations;
                                locationNum = findLocations.LocationNo;
                                Aisle = itemAisle;
                                Rukutai = itemRukutai;
                                break;
                            }
                            else
                            {
                                //2.校验入库台任务是否达到极限(一个入库台正在执行中的任务最多两个)
                                List<ZjnWmsTaskDetailsEntity> TaskCountList = await _zjnTaskListDetailsRepository.GetListAsync(a => a.TaskDetailsStates < 3 && a.TaskDetailsEnd == itemRukutai && a.TaskType == 7);
                                if (TaskCountList == null || TaskCountList.Count < 2)
                                {
                                    temps = findLocations;
                                    locationNum = findLocations.LocationNo;
                                    Aisle = itemAisle;
                                    Rukutai = itemRukutai;
                                    break;
                                }
                                //3.防止小于任务2的条件无法找到货位，所以加后续代码
                                if (temps.LocationNo == null)
                                {
                                    temps = findLocations;
                                    locationNum = findLocations.LocationNo;
                                    Aisle = findLocations.AisleNo;
                                    Rukutai = itemRukutai;
                                }
                            }
                        }
                    }

                    if (temps.LocationNo == null)
                    {
                        throw HSZException.Oh("货位分配失败，参考原因：1.巷道禁用、货位已满、入库台已满...");
                    }

                    if (currentAisle.NowroadwayGroup != temps.AisleNo)
                    {
                        isConflict = true;
                    }
                }
                #endregion

                //获取入库台绑定的行列层
                var workSiteInfo = await _zjnWcsWorkSiteRepository.GetFirstAsync(w => w.DeviceId == Rukutai);
                if (workSiteInfo == null || workSiteInfo.Row == null || workSiteInfo.Cell == null || workSiteInfo.Layer == null)
                {
                    throw HSZException.Oh("未找到入库台工作站点,或未绑定行列层");
                }
                //获取巷道绑定的堆垛机
                var aisleInfo = await _zjnWmsAisleRepository.GetFirstAsync(w => w.AisleNo == Aisle);
                if (aisleInfo == null)
                {
                    throw HSZException.Oh("未找到巷道信息");
                }

                //更新子任务
                var entity = dto.Adapt<ZjnWmsTaskDetailsEntity>();
                entity.TaskDetailsEnd = Rukutai;//入库台任务入库口
                entity.RowEnd = workSiteInfo.Row;
                entity.CellEnd = workSiteInfo.Cell;
                entity.LayerEnd = workSiteInfo.Layer;

                var next = await _ZjnTaskService.GetNextTaskDetails(entity.TaskDetailsId);
                var nextEntity = next.Adapt<ZjnWmsTaskDetailsEntity>();
                nextEntity.TaskDetailsStart = Rukutai;
                nextEntity.TaskDetailsEnd = temps.LocationNo;//入库口->货位 堆垛机任务
                nextEntity.TaskDetailsMove = aisleInfo.StackerNo;
                nextEntity.RowStart = workSiteInfo.Row;
                nextEntity.CellStart = workSiteInfo.Cell;
                nextEntity.LayerStart = workSiteInfo.Layer;
                nextEntity.RowEnd = temps.Row;
                nextEntity.CellEnd = temps.Cell;
                nextEntity.LayerEnd = temps.Layer;
                await _zjnTaskListDetailsRepository.AsUpdateable(new List<ZjnWmsTaskDetailsEntity> { entity, nextEntity }).ExecuteCommandAsync();
                //预约货位
                ZjnWmsLocationEntity locationentity = new ZjnWmsLocationEntity();
                await _zjnWmsLocationAutoService.UpdateLocationStatus(temps.LocationNo, LocationStatus.Order);
                if (istrue)
                {
                    if (isConflict)
                    {
                        currentAisle.NowroadwayGroup = (Convert.ToInt32(Aisle) + 1).ToString("d3");
                        currentAisle.NowroadwayGroupCode = Convert.ToInt32(Aisle) + 1;
                    }
                    else
                    {
                        //更新入库均衡策略
                        if (currentAisle.NowroadwayGroupCode == 12)
                        {
                            currentAisle.NowroadwayGroup = "001";
                            currentAisle.NowroadwayGroupCode = 1;
                        }
                        else
                        {
                            currentAisle.NowroadwayGroup = (Convert.ToInt32(currentAisle.NowroadwayGroupCode) + 1).ToString("d3");
                            currentAisle.NowroadwayGroupCode = currentAisle.NowroadwayGroupCode + 1;
                        }
                    }
                    await _zjnWmsRoadwayInboundPlanRepository.AsUpdateable(currentAisle).ExecuteCommandAsync();
                }
                _db.CommitTran();
                dto.taskDetailsEnd = Rukutai;
                dto.rowEnd = entity.RowEnd;
                dto.cellEnd = entity.CellEnd;
                dto.layerEnd = entity.LayerEnd;

                return dto;
            }
            catch (Exception ex)
            {
                _db.RollbackTran();
                throw HSZException.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 分配巷道
        /// </summary>
        /// <param name="locationDic">巷道所有货位</param>
        /// <param name="binds">所有绑定的入库口</param>
        /// <param name="productLevel">产品等级</param>
        /// <param name="aisleNo">巷道分组策略当前巷道</param>
        /// <returns></returns>
        private async Task<(ZjnWmsLocationEntity, ZjnWmsTaskDetailsEntity, string, string)> AllotLocation(Dictionary<string, List<ZjnWmsLocationEntity>> locationDic, ZjnWmsTaskDetailsInfoOutput[] binds, string productLevel, string aisleNo)
        {
            //var aisleCount = _locationGenerator.ReferInfo.AisleNos.Length;
            var aisleCount = locationDic.Count();
            var aisleGroups = binds.Where(w => w.productLevel == productLevel)
                .GroupBy(g => g.taskDetailsMove).OrderBy(o => o.Key);
            foreach (var aisleGroup in aisleGroups)//找相同产品等级的入库口
            {
                if (aisleGroup.Count() == 2)//入库口已满
                {
                    continue;
                }
                else if (aisleGroup.Count() == 1)
                {
                    //优先级：相邻货位，一前一后，尽量靠近,另外货位需要重新分配
                    var firstTaskDetail = aisleGroup.First();
                    if (!locationDic.ContainsKey(firstTaskDetail.taskDetailsMove))
                    {
                        continue;
                    }
                    else
                    {
                        var intoTask = await _zjnTaskListDetailsRepository.GetFirstAsync(x => x.TaskType == 5 && x.TaskDetailsStart == firstTaskDetail.taskDetailsEnd && x.TaskDetailsStates == 2);
                        if (intoTask == null) break;//有bug会进，就停止分配第2个工位
                        var firstLocation = await _zjnWmsLocationRepository.GetFirstAsync(x => x.LocationNo == intoTask.TaskDetailsEnd);
                        if (firstLocation == null) break;//有bug会进，就停止分配第2个工位
                        var locations = locationDic[firstTaskDetail.taskDetailsMove];
                        locations.Insert(0, firstLocation);//把单个货位分配的算上再重新分配
                        var locDic = locations.ToDictionary(k => k.LocationNo, v => v);
                        string nearLocationNo = string.Empty;
                        foreach (var item in locations)
                        {
                            //原有堆垛机任务也需变更，入库口任务不变
                            nearLocationNo = $"{item.ByWarehouse}-{item.AisleNo}-{item.Row.ToString("d3")}-{(item.Cell - 1).ToString("d3")}-{item.Layer.ToString("d3")}";
                            if (locDic.ContainsKey(nearLocationNo))
                            {
                                return (item, SetFirstStackerTask(locDic[nearLocationNo], intoTask), firstTaskDetail.taskDetailsMove, firstLocation.LocationNo);
                            }
                            else if (item.Row == 1)
                            {
                                nearLocationNo = $"{item.ByWarehouse}-{item.AisleNo}-002-{(item.Cell - 1).ToString("d3")}-{item.Layer.ToString("d3")}";
                                if (locDic.ContainsKey(nearLocationNo))
                                {
                                    return (item, SetFirstStackerTask(locDic[nearLocationNo], intoTask), firstTaskDetail.taskDetailsMove, firstLocation.LocationNo);
                                }
                            }
                            else if (item.Row == 2)
                            {
                                nearLocationNo = $"{item.ByWarehouse}-{item.AisleNo}-001-{(item.Cell - 1).ToString("d3")}-{item.Layer.ToString("d3")}";
                                if (locDic.ContainsKey(nearLocationNo))
                                {
                                    return (item, SetFirstStackerTask(locDic[nearLocationNo], intoTask), firstTaskDetail.taskDetailsMove, firstLocation.LocationNo);
                                }
                            }
                        }
                        return (locations[0], SetFirstStackerTask(locations[1], intoTask), firstTaskDetail.taskDetailsMove, firstLocation.LocationNo);//相邻的直接取第一个,原有任务也需变更
                    }
                }
            }
            int num = Convert.ToInt32(aisleNo);
            int cur = num + 1;
            while (cur != num)
            {
                aisleNo = cur.ToString("d3");
                if (!locationDic.ContainsKey(aisleNo) || binds.Any(a => a.taskDetailsMove == aisleNo))//所有占位的入库口直接排除
                {
                    if (cur >= aisleCount)
                    {
                        cur = 1;
                    }
                    else
                    {
                        cur++;
                    }
                    continue;
                }
                var locations = locationDic[aisleNo];
                if (cur >= aisleCount)
                {
                    cur = 1;
                }
                else
                {
                    cur++;
                }
                return (locations[0], null, aisleNo, null);
            }
            return (locationDic["001"][0], null, "001", null); ;//分配失败分配缓存位
        }

        /// <summary>
        /// 更新第一个任务的货位
        /// </summary>
        /// <param name="location">货位明细</param>
        /// <param name="detail">任务明细</param>
        /// <returns></returns>
        private ZjnWmsTaskDetailsEntity SetFirstStackerTask(ZjnWmsLocationEntity location, ZjnWmsTaskDetailsEntity detail)
        {
            detail.TaskDetailsEnd = location.LocationNo;
            detail.RowEnd = location.Row;
            detail.CellEnd = location.Cell;
            detail.LayerEnd = location.Layer;
            return detail;
        }

        /// <summary>
        /// 入库直接开始堆垛机任务（测试直接从入库口入库）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<ZjnWmsTaskDetailsInfoOutput> TestStackerAllot(ZjnWmsTaskDetailsInfoOutput dto)
        {
            try
            {
                var tray = await _zjnWmsTrayRepository.GetFirstAsync(w => w.TrayNo == dto.trayNo && w.IsDelete == 0);
                if (tray == null)
                {
                    throw HSZException.Oh("未找到托盘数据，无法分配货位");
                }
                //var goodsCode = string.Empty;
                //var trayGoods = await _zjnWmsTrayGoodsRepository.GetFirstAsync(w => w.TrayNo == dto.trayNo && w.IsDeleted == 0);
                //if (trayGoods != null)
                //{
                //    goodsCode = trayGoods.GoodsCode;
                //}
                var aisleCount = _locationGenerator.ReferInfo.AisleNos.Length;
                var executingTasks = await _zjnTaskListDetailsRepository.AsQueryable().RightJoin<ZjnWmsEquipmentListEntity>(
                    (s, t) => s.TaskDetailsEnd == t.EquipmentSerialNumber).Where((s, t) => s.TaskType == 7
                    && s.TaskDetailsStates == 2 && s.TaskDetailsDescribe == dto.productLevel && t.Type == "2"
                    && t.TheBinding == dto.taskDetailsStart)
                    .Select((s, t) => new ZjnWmsTaskDetailsInfoOutput()
                    {
                        taskDetailsEnd = t.EquipmentSerialNumber,
                        taskDetailsMove = t.TheBinding,
                        productLevel = s.TaskDetailsDescribe,
                        id = s.Id,
                        taskDetailsStart = s.TaskDetailsStart,
                        taskDetailsStates = s.TaskDetailsStates,
                        rowStart = s.RowStart,
                        cellStart = s.CellStart,
                        layerStart = s.LayerStart,
                        rowEnd = s.RowEnd,
                        cellEnd = s.CellEnd,
                        layerEnd = s.LayerEnd,
                        nodeCode = s.NodeCode,
                        nodePropertyJson = s.NodePropertyJson,
                        nodeNext = s.NodeNext,
                        nodeType = s.NodeType,
                        nodeUp = s.NodeUp,
                        trayNo = s.TrayNo,
                        resultValue = s.ResultValue,
                        taskDetailsId = s.TaskDetailsId,
                        taskId = s.TaskId,
                        taskType = s.TaskType,
                        workPathId = s.WorkPathId
                    }).ToArrayAsync();//总共2个入库口


                var AisleNo = (await _zjnWmsEquipmentListRepository.GetListAsync(x => x.EquipmentSerialNumber == dto.taskDetailsStart)).FirstOrDefault()?.TheBinding;
                var locations = await _zjnWmsLocationRepository.AsQueryable()
                    .Where(w => w.LocationStatus == "0")//空货位
                    .Where(w => w.Warning == 0)
                    .Where(w => w.AisleNo == AisleNo)
                    .Where(w => w.EnabledMark == 1 && w.IsDelete == 0)

                    //.Where(w => w.AisleNo == currentAisle.NowroadwayGroup)
                    //.WhereIF(locationGroup != null && groupDetails.Count != 0, exp.ToExpression())
                    .PartitionBy(w => new { w.Layer }).Take(_locationGenerator.MaxPerLayer)//用PartitionBy必须Take，源码默认了Take(1)
                    .OrderBy(o => o.Cell, OrderByType.Desc)
                    .OrderBy(o => o.Priority, OrderByType.Desc)
                    .OrderBy(o => o.Layer)
                    .ToListAsync();

                var locDic = locations.OrderByDescending(o => o.Cell).GroupBy(g => g.AisleNo).ToDictionary(k => k.Key, v => v.ToList());//直接找出所有巷道的数据
                if (locDic.Count == 0)
                {
                    //改为缓存区任务
                    throw HSZException.Oh("缓存区任务暂未开始");
                }
                (ZjnWmsLocationEntity, ZjnWmsTaskDetailsEntity, string, string) locationRes =
                    await AllotLocation(locDic, executingTasks, dto.productLevel, AisleNo);
                _db.BeginTran();
                if (locationRes.Item1 == null)//全部巷道已有入库任务
                {
                    //改为缓存区任务
                    throw HSZException.Oh("缓存区任务暂未开始");
                }
                ZjnWmsLocationEntity location = locationRes.Item1;
                await _zjnWmsLocationAutoService.UpdateLocationStatus(location.LocationNo, LocationStatus.Order);
                ZjnWmsTaskDetailsInfoOutput puInfo = new();
                var puList = executingTasks.Where(w => w.taskDetailsMove == dto.taskDetailsStart).OrderBy(o => o.taskDetailsEnd);
                if (!puList.Any())
                {
                    puInfo.taskDetailsEnd = dto.taskDetailsStart;
                }
                else
                {
                    if (locationRes.Item2 == null)
                    {
                        puInfo = puList.ElementAt(0);
                    }
                    else
                    {
                        puInfo = puList.ElementAt(1);
                        await _zjnTaskListDetailsRepository.UpdateAsync(locationRes.Item2);//更新任务1的终点货位
                        if (locationRes.Item2.TaskDetailsEnd != locationRes.Item4)
                        {
                            await _zjnWmsLocationAutoService.UpdateLocationStatus(locationRes.Item2.TaskDetailsEnd, LocationStatus.Order);
                            await _zjnWmsLocationAutoService.UpdateLocationStatus(locationRes.Item4, LocationStatus.Empty);
                        }
                    }

                }

                var workSiteInfo = await _zjnWcsWorkSiteRepository.GetFirstAsync(w => w.DeviceId == puInfo.taskDetailsEnd);
                if (workSiteInfo == null)
                {
                    throw HSZException.Oh("未找到入库台工作站点");
                }
                var aisleInfo = await _zjnWmsAisleRepository.GetFirstAsync(w => w.AisleNo == AisleNo);
                if (aisleInfo == null)
                {
                    throw HSZException.Oh("未找到巷道信息");
                }
                dto.taskDetailsEnd = location.LocationNo;//入库口->货位 堆垛机任务
                dto.taskDetailsMove = aisleInfo.StackerNo;
                dto.rowStart = workSiteInfo.Row;
                dto.cellStart = workSiteInfo.Cell;
                dto.layerStart = workSiteInfo.Layer;
                dto.rowEnd = location.Row;
                dto.cellEnd = location.Cell;
                dto.layerEnd = location.Layer;
                var entity = dto.Adapt<ZjnWmsTaskDetailsEntity>();
                await _zjnTaskListDetailsRepository.AsUpdateable(entity).ExecuteCommandAsync();
                _db.CommitTran();
                return dto;
            }
            catch (Exception ex)
            {
                _db.RollbackTran();
                throw HSZException.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 找空托
        /// </summary>
        /// <param name="trayType"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>

        [NonAction]
        public async Task<ZjnWmsLocationEntity> FindEmptyTrayLocation(int trayType, string deviceId)
        {

            ZjnWmsLocationEntity output = await _zjnWmsLocationRepository.GetFirstAsync(x => x.LocationNo == "");
            return output;
        }

        /// <summary>
        /// 找消防货位，优先找距离近的消防位置
        /// </summary>
        /// <param name="location">消防货位</param>
        /// <returns></returns>
        [NonAction]
        public async Task<ZjnWmsTaskDetailsInfoOutput> FindFireControlLocation(ZjnWmsTaskDetailsInfoOutput dto)
        {
            //消防货位
            ZjnWmsLocationEntity startLocation = await _zjnWmsLocationRepository.GetFirstAsync(x => x.LocationNo == dto.taskDetailsStart);
            if (startLocation == null) throw HSZException.Oh("没有找到消防起始货位！");
            dto.rowStart = startLocation.Row;
            dto.cellStart = startLocation.Cell;
            dto.layerStart = startLocation.Layer;
            //巷道找堆垛机
            var aisleInfo = await _zjnWmsAisleRepository.GetFirstAsync(x => x.AisleNo == startLocation.AisleNo);
            if (aisleInfo == null) throw HSZException.Oh("未配置巷道数据");
            dto.taskDetailsMove = aisleInfo.StackerNo;

            //找消防货位
            var fireControlList = await _zjnWmsLocationRepository.GetListAsync(x => x.Warning == 1 && x.AisleNo == startLocation.AisleNo);
            if (fireControlList == null) throw HSZException.Oh("未找到消防货位！");
            //查询货位到水箱的距离
            var fireCellList = fireControlList.Distinct().Select(x => Math.Abs(x.Cell - startLocation.Cell)).ToList();
            //获取距离最短的列
            var minCell = fireCellList.Min();
            //查询最短列的货位
            var fireLocation = fireControlList.Where(x => Math.Abs(x.Cell - startLocation.Cell) == minCell).FirstOrDefault();
            if (fireLocation == null) throw HSZException.Oh("未找到消防货位！");
            dto.taskDetailsEnd = fireLocation.LocationNo;
            dto.rowEnd = fireLocation.Row;
            dto.cellEnd = fireLocation.Cell;
            dto.layerEnd = fireLocation.Layer;

            return dto;
        }

        /// <summary>
        /// 成品库找出库货位   
        /// </summary>
        /// <param name="TaskCrInput">出库参数</param>
        /// <returns>货位和优先级</returns>
        [NonAction]
        public async Task<List<ZjnWmsTaskCrInput>> FindOutBoundLocation(ZjnWmsTaskCrInput TaskCrInput)
        {
            var trayCodes = TaskCrInput.taskList.Any(s => !string.IsNullOrEmpty(s.TrayNo))
                            ? TaskCrInput.taskList.Select(s => s.TrayNo).ToArray() : new string[0];
            var qty = TaskCrInput.quantity == null ? 0 : Convert.ToInt32(TaskCrInput.quantity);
            List<ZjnWmsTaskCrInput> res = new List<ZjnWmsTaskCrInput>();
            ZjnWmsGoodsEntity materialInfo = null;
            if (trayCodes.Length == 0)
            {
                materialInfo = await _zjnWmsGoodsRepository.GetFirstAsync(w => w.GoodsCode == TaskCrInput.materialCode);
                if (materialInfo == null)
                {
                    throw HSZException.Oh("物料数据不存在");
                }
            }
            var datas = await _zjnWmsMaterialInventoryRepository.AsQueryable()
                    .InnerJoin<ZjnWmsLocationEntity>((a, b) => a.ProductsLocation == b.LocationNo && a.IsDeleted == 0
                    && b.IsDelete == 0 && b.EnabledMark == 1)
                    .InnerJoin<ZjnWmsAisleEntity>((a, b, c) => b.AisleNo == c.AisleNo && c.EnabledMark == 1)
                    .InnerJoin<ZjnWmsTrayEntity>((a, b, c, d) => a.ProductsContainer == d.TrayNo && d.EnabledMark == 1)
                    .WhereIF(trayCodes.Length == 0, (a, b, c, d) => b.LocationStatus == "1" && a.ProductsCode == TaskCrInput.materialCode)
                    .WhereIF(trayCodes.Length > 0, (a, b, c, d) => b.LocationStatus == "1" && trayCodes.Contains(a.ProductsContainer))
                    .OrderBy((a, b) => b.Depth).OrderBy(a => a.CreateTime, OrderByType.Asc)
                    .Select((a, b) => new { a, b }).ToListAsync();//深度的靠后处理
            List<ZjnWmsLocationEntity> locations;
            if (trayCodes.Length == 0)
            {
                var expiredData = datas.Where(w => (DateTime.Now - w.a.CreateTime.Value).TotalDays
            > materialInfo.ShelfLife.ToInt32());//超出保质期的直接出，不用再完全匹配
                var restData = datas.Except(expiredData);
                foreach (var item in expiredData)
                {
                    qty -= item.a.ProductsQuantity.ToInt32();
                    if (qty <= 0)
                    {
                        break;
                    }
                }
                int[] selectedIndexs = new int[0];
                List<ZjnWmsLocationEntity> selectedLocations = new List<ZjnWmsLocationEntity>();
                if (qty > 0)
                {
                    selectedIndexs = MatchOutNum(restData.Select(s => s.a.ProductsQuantity.ToInt32()).ToList(), qty);
                    foreach (var selectedIndex in selectedIndexs)
                    {
                        selectedLocations.Add(restData.ElementAt(selectedIndex).b);
                    }
                }
                locations = expiredData.Select(s => s.b).Union(selectedLocations).ToList();
            }
            else
            {
                locations = datas.Select(s => s.b).ToList();
            }
            if (locations.Count == 0)
            {
                throw HSZException.Oh("未找到货位");
            }
            int index = 0;
            foreach (var location in locations)
            {
                await _zjnWmsLocationAutoService.UpdateLocationStatus(location.LocationNo, LocationStatus.Lock);
                res.Add(new ZjnWmsTaskCrInput()
                {
                    priority = 2,
                    trayNo = location.TrayNo,
                    positionFrom = location.LocationNo,
                    materialCode = trayCodes.Length == 0 ? TaskCrInput.materialCode : datas[index].a.ProductsCode,
                    quantity = qty
                });
                index++;
            }
            return res;
        }

        /// <summary>
        /// 匹配出库数量算法（没有则返回最接近且大于,结果尽可能靠前）
        /// </summary>
        /// <param name="qtys">可出库库存数量列表</param>
        /// <param name="val">需要匹配的值</param>
        /// <returns>下标列表</returns>
        private int[] MatchOutNum(List<int> qtys, int val)
        {
            List<int> list = new List<int>();//相等的情况
            List<int> noneList = new List<int>();//备用结果，没有相等的情况
            Dictionary<int, int[]> noneDic = new Dictionary<int, int[]>();//不知道最小的，需要全部相加才能找到
            return MatchDfs(qtys, val, 0, list, noneList, noneDic, -1, 0);
        }

        /// <summary>
        /// 回溯算法找数量逻辑，已完成
        /// </summary>
        /// <param name="qtys"></param>
        /// <param name="val"></param>
        /// <param name="index"></param>
        /// <param name="list"></param>
        /// <param name="noneList"></param>
        /// <param name="noneDic"></param>
        /// <param name="min"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        private int[] MatchDfs(List<int> qtys, int val, int index, List<int> list
            , List<int> noneList, Dictionary<int, int[]> noneDic, int min, int qty)
        {
            if (index >= qtys.Count)
            {
                do
                {
                    if (list.Count == 0)
                    {
                        if (noneDic.Count > 0)
                        {
                            return noneDic[noneDic.Keys.Min()];
                        }
                        else
                        {
                            return Enumerable.Range(0, qtys.Count).ToArray();
                        }
                    }
                    index = list[list.Count - 1] + 1;
                    qty -= qtys[list[list.Count - 1]];
                    list.RemoveAt(list.Count - 1);
                    noneList.RemoveAt(noneList.Count - 1);
                } while (index >= qtys.Count && list.Count >= 0);
            }
            qty += qtys[index];
            if (qty == val)
            {
                list.Add(index);
                return list.ToArray();
            }
            else if (qty < val)
            {
                list.Add(index);
                noneList.Add(index);
                MatchDfs(qtys, val, index + 1, list, noneList, noneDic, min, qty);
            }
            else
            {
                if (min == -1 || min > qty)
                {
                    noneList.Add(index);
                    min = qty;
                    noneDic.Add(qty, noneList.ToArray());
                    noneList.RemoveAt(noneList.Count - 1);
                }
                qty -= qtys[index];
                MatchDfs(qtys, val, index + 1, list, noneList, noneDic, min, qty);
            }
            if (list.Count > 0)
            {
                return list.ToArray();
            }
            else if (noneDic.Count > 0)
            {
                return noneDic[noneDic.Keys.Min()];
            }
            else
            {
                return Enumerable.Range(0, qtys.Count).ToArray();
            }
        }

        /// <summary>
        /// 寻找移库货位（暂不考虑跨巷道）
        /// </summary>
        /// <param name="input">移库信息</param>
        /// <returns>终点货位编码</returns>
        [NonAction]
        public async Task<string> FindMoveLocation(ZjnWmsTaskCrInput input)
        {
            return input.positionFrom;//无此业务
        }
    }
}
