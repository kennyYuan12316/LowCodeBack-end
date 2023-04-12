using HSZ.Common.Core.Manager;
using HSZ.Common.DI;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.wms.Entitys.Dto.ZjnWmsTask;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using HSZ.wms.Interfaces.ZjnWmsLocation;
using HSZ.wms.Interfaces.ZjnWmsTask;
using HSZ.Wms.Entitys.Dto.ZjnWmsLocation;
using HSZ.Wms.Interfaces.zjnLocationGenerator;
using HSZ.Wms.Interfaces.ZjnWorkProsess;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HSZ.Wms.Interfaces.ZjnWorkProsess;
using HSZ.Common.DI;
using HSZ.Wms.Entitys.Dto.ZjnWmsLocation;
using HSZ.Wms.Interfaces.zjnLocationGenerator;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace HSZ.Wms.ZjnWorkProsess
{
    /// <summary>
    /// 入库寻找货位类
    /// </summary>
    [ApiDescriptionSettings(Tag = "FindLocation", Name = "FindLocation", Order = 220)]
    [Route("api/[controller]")]
    [WareDI(WareType.FrameMember)]
    public class FrameMemberFindLocationProcess : IFindLocationProcess, IDynamicApiController, ITransient
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

        private readonly IZjnWmsTaskService _zjnTaskService;
        private readonly IZjnWmsLocationAutoService _zjnWmsLocationAutoService;
        private readonly ILocationGenerator _locationGenerator;
        private readonly ISqlSugarRepository<ZjnWmsMaterialInventoryEntity> _zjnWmsMaterialInventoryRepository;
        private readonly ISqlSugarRepository<ZjnWmsTrayGoodsEntity> _zjnWmsTrayGoodsRepository;
        private readonly ISqlSugarRepository<ZjnWcsProcessConfigEntity> _zjnWcsProcessConfigRepository;
        private readonly ISqlSugarRepository<ZjnWmsTrayLocationLogEntity> _zjnWmsTrayLocationLogEntity;
        private readonly ISqlSugarRepository<ZjnWmsGoodsEntity> _zjnWmsGoodsRepository;


        /// <summary>
        /// 初始化
        /// </summary>
        public FrameMemberFindLocationProcess(
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
            ISqlSugarRepository<ZjnWmsTrayLocationLogEntity> zjnWmsTrayLocationLogEntity,
            ISqlSugarRepository<ZjnWmsGoodsEntity> zjnWmsGoodsRepository
            )
        {
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _zjnTaskService = zjnTaskService;
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
            _zjnWmsTrayLocationLogEntity = zjnWmsTrayLocationLogEntity;
            _zjnWmsGoodsRepository = zjnWmsGoodsRepository;
        }

        /// <summary>
        /// 分配货位并生成子任务
        /// </summary>
        /// <param name="dto">子任务信息</param>
        /// <returns></returns>
        [NonAction]
        public async Task<ZjnWmsTaskDetailsInfoOutput> WmsAllotLocationTask(ZjnWmsTaskDetailsInfoOutput dto)
        {
            try
            {
                var tray = await _zjnWmsTrayRepository.GetFirstAsync(w => w.TrayNo == dto.trayNo && w.IsDelete == 0);
                if (tray == null)
                {
                    throw HSZException.Oh("未找到托盘数据，无法分配货位");
                }
                var goodsCode = string.Empty;
                var trayGoods = await _zjnWmsTrayGoodsRepository.GetFirstAsync(w => w.TrayNo == dto.trayNo && w.IsDeleted == 0);
                if (trayGoods != null)
                {
                    goodsCode = trayGoods.GoodsCode;
                }
                var InDevices = await _zjnWmsEquipmentListRepository.GetListAsync(w => w.EquipmentSerialNumber == dto.taskDetailsStart);
                if (InDevices.Count == 0)
                {
                    throw HSZException.Oh("未配置RGV接驳台入库口绑定关系");
                } 
                //获取巷道均衡策略
                var currentAisle = await _zjnWmsRoadwayInboundPlanRepository.AsQueryable().FirstAsync();
                if (currentAisle == null)
                {
                    throw HSZException.Oh("未配置巷道入库均衡策略");
                }
                var aisleCount = await _zjnWmsAisleRepository.CountAsync(c => c.AisleNo.StartsWith(currentAisle.NowroadwayGroup));

                var locationGroup = await _zjnWmsLocationGroupRepository.GetFirstAsync(w => w.EnabledMark == 1);// 根据托盘类型分组，没有就不用
                var groupDetails = new List<ZjnWmsLocationGroupDetailsEntity>();
                if (locationGroup != null)
                {
                    groupDetails = await _zjnWmsLocationGroupDetailsRepository.GetListAsync(w => w.GroupCode == locationGroup.GroupNo && w.EnabledMark == 1);
                }
                
                var exp = Expressionable.Create<ZjnWmsLocationEntity>();
                foreach (var group in groupDetails)
                {
                    exp.Or(a => a.Row >= group.StartRow && a.Row <= group.EndRow &&
                    a.Cell >= group.StartCell && a.Cell <= group.EndCell &&
                    a.Layer >= group.StartLayer && a.Layer <= group.EndLayer &&
                    a.Depth >= group.StartDepth && a.Depth <= group.EndDepth);
                }
                var locations = await _zjnWmsLocationRepository.AsQueryable()
                    .InnerJoin<ZjnWmsAisleEntity>((a, b) => a.AisleNo == b.AisleNo)
                        .Where(a => a.LocationStatus == "0")//空货位
                        .WhereIF(locationGroup != null && groupDetails.Count != 0, exp.ToExpression())
                        .Where(a => a.EnabledMark == 1 && a.IsDelete == 0)
                        .Where((a, b) => b.EnabledMark == 1)
                        .PartitionBy(a => new { a.Layer }).Take(_locationGenerator.MaxPerLayer)//用PartitionBy必须Take，源码默认了Take(1)
                        //.OrderByIF(roadwayGroupNo == "A", a => a.Cell, OrderByType.Desc)
                        .OrderBy(a => a.Cell, OrderByType.Desc)
                        .OrderBy(a => a.Priority, OrderByType.Desc)
                        .OrderBy(a => a.Layer)
                        .ToListAsync();

                ZjnWmsLocationEntity locationRes = null;
                var locDic = locations.GroupBy(g => g.AisleNo).ToDictionary(k => k.Key, v => v.ToList());//直接找出所有巷道的数据
                if (locDic.Count == 0)
                {
                    throw HSZException.Oh("货位不足，不可入库。");
                }
                else
                {
                    int num = Convert.ToInt32(currentAisle.NowroadwayGroup.Substring(1));
                    int cur = num;
                    do
                    {
                        currentAisle.NowroadwayGroup = $"{currentAisle.NowroadwayGroup[0]}{cur}";

                        if (cur >= aisleCount)
                        {
                            cur = 1;
                        }
                        else
                        {
                            cur++;
                        }
                        if (!locDic.ContainsKey(currentAisle.NowroadwayGroup))
                        {
                            continue;
                        }
                        locations = locDic[currentAisle.NowroadwayGroup];
                         
                        locationRes = await AllotLocationV2(locations, goodsCode);
                    } while (cur != num && locationRes == null);
                }
                _db.BeginTran();
                
                if (locationRes == null)
                {
                    throw HSZException.Oh("未找到满足条件的货位");
                }
                await _zjnWmsLocationAutoService.UpdateLocationStatus(locationRes.LocationNo, LocationStatus.Order); 

                var puList = await _zjnWmsEquipmentListRepository.GetListAsync(w => w.TheBinding == currentAisle.NowroadwayGroup);
                var PuInfo = puList.Find(x => x.EquipmentSerialNumber[1] == dto.taskDetailsStart[1]);
                if (PuInfo == null)
                {
                    throw HSZException.Oh("未找到入库口设备");
                }
                var workSiteInfo = await _zjnWcsWorkSiteRepository.GetFirstAsync(w => w.DeviceId == PuInfo.EquipmentSerialNumber);
                if (workSiteInfo == null)
                {
                    throw HSZException.Oh("未找到入库台工作站点");
                }
                var aisleInfo = await _zjnWmsAisleRepository.GetFirstAsync(w => w.AisleNo == currentAisle.NowroadwayGroup);
                if (aisleInfo == null)
                {
                    throw HSZException.Oh("未找到巷道信息");
                }
                var entity = dto.Adapt<ZjnWmsTaskDetailsEntity>();
                entity.TaskDetailsEnd = PuInfo.EquipmentSerialNumber;//RGV接驳台->入库口 平面设备任务
                var next = await _zjnTaskService.GetNextTaskDetails(entity.TaskDetailsId);
                var nextEntity = next.Adapt<ZjnWmsTaskDetailsEntity>();
                nextEntity.TaskDetailsStart = PuInfo.EquipmentSerialNumber;
                nextEntity.TaskDetailsEnd = locationRes.LocationNo;//入库口->货位 堆垛机任务
                nextEntity.TaskDetailsMove = aisleInfo.StackerNo;
                nextEntity.RowStart = 1;
                nextEntity.CellStart = 101;
                nextEntity.LayerStart = 1;
                nextEntity.RowEnd = locationRes.Row;
                nextEntity.CellEnd = locationRes.Cell;
                nextEntity.LayerEnd = locationRes.Layer;
                await _zjnTaskListDetailsRepository.AsUpdateable(new List<ZjnWmsTaskDetailsEntity> { entity, nextEntity }).ExecuteCommandAsync();

                //更新巷道策略
                int curnum = Convert.ToInt32(currentAisle.NowroadwayGroup.Substring(1));
                if (curnum < 5)
                {
                    currentAisle.NowroadwayGroup = $"{currentAisle.NowroadwayGroup[0]}{curnum + 1}";
                }
                else
                {
                    currentAisle.NowroadwayGroup = $"{currentAisle.NowroadwayGroup[0]}{1}";
                }
                await _zjnWmsRoadwayInboundPlanRepository.AsUpdateable(currentAisle).ExecuteCommandAsync();
                _db.CommitTran();
                //dto.taskDetailsEnd = PuInfo.TheBinding;
                dto.taskDetailsEnd = PuInfo.EquipmentSerialNumber;//update by yml 2022/10/25
                //Rgv
                if (dto.taskType == 7)
                {
                    dto.rowEnd = workSiteInfo.Row;
                    dto.cellEnd = workSiteInfo.Cell;
                    dto.layerEnd = workSiteInfo.Layer;
                }
                return dto;
            }
            catch (Exception ex)
            {
                _db.RollbackTran();
                throw HSZException.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 分配双深道巷道
        /// </summary>
        /// <param name="locations">巷道所有货位</param>
        /// <param name="goodsCode">当前任务商品条码</param>
        /// <param name="trayType">托盘类型</param>
        /// <returns></returns>
        [NonAction]
        private async Task<(ZjnWmsLocationEntity, string)> AllotLocation(List<ZjnWmsLocationEntity> locations, string goodsCode, int? trayType)
        {
            bool needMove = false;//是否需要移库
            Dictionary<string, string> leftOuterDic = null;
            Dictionary<string, string> rightOuterDic = null;
            Dictionary<string, object> locGoodsDic = null;
            Dictionary<string, ZjnWmsLocationEntity> locDic = null;
            //if (trayType == 2)
            //{
            //    locDic = locations.ToDictionary(k => k.LocationNo, v => v);
            //}
            var referInfo = _locationGenerator.ReferInfo;
            ZjnWmsLocationEntity selectedLocation = null;
            string selectedNearLocationNo = null;
            foreach (var location in locations)
            {
                StaticSide side;
                if (referInfo.IsStatic)
                {
                    side = referInfo.Side;
                }
                else
                {
                    side = referInfo.Sides[location.AisleNo];
                }
                string nearLocationNo = string.Empty;
               
                if (side.LeftSide == 2 && side.RightSide == 2)
                {
                    return (location, nearLocationNo);
                }
                
                #region 分配深度为1的货位判断逻辑
                if (side.LeftSide > 1)
                {
                    if (location.Row == 1)
                    {
                        if (leftOuterDic == null)
                        {
                            leftOuterDic = locations.Where(w => w.Row == 2)
                            .ToDictionary(k => k.LocationNo, v => v.LocationStatus);
                        }
                        var outerLocationNo = $"{location.ByWarehouse}-{location.AisleNo}-002-{location.Cell.ToString("d3")}-{location.Layer.ToString("d3")}";

                        if (!leftOuterDic.ContainsKey(outerLocationNo))//外面不是空货位
                        {
                            if (selectedLocation == null)
                            {
                                needMove = true;
                                selectedLocation = location;
                            }
                            continue;
                        }
                        else
                        {
                            return (location, string.Empty);
                        }

                    }
                }
                if (side.RightSide > 1)
                {
                    if (location.Row == 4)
                    {
                        if (rightOuterDic == null)
                        {
                            rightOuterDic = locations.Where(w => w.Row == 3)
                            .ToDictionary(k => k.LocationNo, v => v.LocationStatus);
                        }
                        var outerLocationNo = $"{location.ByWarehouse}-{location.AisleNo}-003-{location.Cell.ToString("d3")}-{location.Layer.ToString("d3")}";

                        if (!rightOuterDic.ContainsKey(outerLocationNo))
                        {
                            if (selectedLocation == null)
                            {
                                needMove = true;
                                selectedLocation = location;
                            }
                            continue;
                        }
                        else
                        {
                            return (location, string.Empty);
                        }

                    }
                }
                #endregion
                #region 分配深度为0的货位判断逻辑
                if (locGoodsDic == null)
                {
                    locGoodsDic = await _zjnWmsMaterialInventoryRepository.AsQueryable()
                        .InnerJoin<ZjnWmsLocationEntity>((a, b) => a.ProductsLocation == b.LocationNo)
                        .Where((a, b) => b.AisleNo == location.AisleNo
                        && (b.Row == 1 || b.Row == 4) && a.IsDeleted != 0).ToDictionaryAsync(k => k.ProductsLocation, v => v.ProductsCode);
                }

                if (side.LeftSide > 1)
                {
                    if (location.Row == 2)
                    {
                        if (!locGoodsDic.ContainsKey(location.LocationNo))
                        {
                            continue;
                        }
                        else
                        {
                            if (locGoodsDic[location.LocationNo].ToString() == goodsCode)
                            {
                                
                                return (location, string.Empty);
                            }
                            else
                            {
                                if (selectedLocation == null)
                                {
                                    selectedLocation = location;
                                    selectedNearLocationNo = nearLocationNo;
                                }
                                continue;
                            }
                        }
                    }
                }
                if (side.RightSide > 1)
                {
                    if (location.Row == 3)
                    {
                        if (!locGoodsDic.ContainsKey(location.LocationNo))
                        {
                            continue;
                        }
                        else
                        {
                            if (locGoodsDic[location.LocationNo].ToString() == goodsCode)
                            { 
                                return (location, string.Empty);

                            }
                            else
                            {
                                if (selectedLocation == null)
                                {
                                    selectedLocation = location;
                                    selectedNearLocationNo = nearLocationNo;
                                }
                                continue;
                            }
                        }
                    }
                }
                #endregion
            }
            if (needMove)
            {
                var moveConfigId = await _zjnWcsProcessConfigRepository.GetFirstAsync(x => x.WorkType == 3);//移库
                if (moveConfigId == null)
                {
                    throw HSZException.Oh("未配置移库业务流程配置");
                }
                //await _zjnTaskService.CreateByConfigId(moveConfigId.Id, "分配货位移库任务");
                //2022-11-8 update by yml
                ZjnWmsTaskCrInput taskInput = new ZjnWmsTaskCrInput
                {
                    taskName = "分配货位移库任务"
                };
                await _zjnTaskService.CreateByConfigId(moveConfigId.Id, taskInput);
            }
            return (selectedLocation, selectedNearLocationNo);
        }

        /// <summary>
        /// 分配双深道巷道，箔材也只占一个货位，只是更大
        /// </summary>
        /// <param name="locations">巷道所有货位</param>
        /// <param name="goodsCode">当前任务商品条码</param>
        /// <returns></returns>
        [NonAction]
        private async Task<ZjnWmsLocationEntity> AllotLocationV2(List<ZjnWmsLocationEntity> locations, string goodsCode)
        {
            bool needMove = false;//是否需要移库
            Dictionary<string, string> leftOuterDic = null;
            Dictionary<string, string> rightOuterDic = null;
            Dictionary<string, object> locGoodsDic = null;
            var referInfo = _locationGenerator.ReferInfo;
            ZjnWmsLocationEntity selectedLocation = null;
            foreach (var location in locations)
            {
                StaticSide side;
                if (referInfo.IsStatic)
                {
                    side = referInfo.Side;
                }
                else
                {
                    side = referInfo.Sides[location.AisleNo];
                }
                #region 分配深度为1的货位判断逻辑
                if (side.LeftSide > 1)
                {
                    if (location.Row == 1)
                    {
                        if (leftOuterDic == null)
                        {
                            leftOuterDic = locations.Where(w => w.Row == 2)
                            .ToDictionary(k => k.LocationNo, v => v.LocationStatus);
                        }
                        var outerLocationNo = $"{location.ByWarehouse}-{location.AisleNo}-002-{location.Cell.ToString("d3")}-{location.Layer.ToString("d3")}";
                        if (!leftOuterDic.ContainsKey(outerLocationNo))//外面不是空货位
                        {
                            if (selectedLocation == null)
                            {
                                needMove = true;
                                selectedLocation = location;
                            }
                            continue;
                        }
                        else
                        {
                            return location;
                        }
                    }
                }
                if (side.RightSide > 1)
                {
                    if (location.Row == 4)
                    {
                        if (rightOuterDic == null)
                        {
                            rightOuterDic = locations.Where(w => w.Row == 3)
                            .ToDictionary(k => k.LocationNo, v => v.LocationStatus);
                        }
                        var outerLocationNo = $"{location.ByWarehouse}-{location.AisleNo}-003-{location.Cell.ToString("d3")}-{location.Layer.ToString("d3")}";
                        if (!rightOuterDic.ContainsKey(outerLocationNo))
                        {
                            if (selectedLocation == null)
                            {
                                needMove = true;
                                selectedLocation = location;
                            }
                            continue;
                        }
                        else
                        {
                            return location;
                        }
                    }
                }
                #endregion
                #region 分配深度为0的货位判断逻辑
                if (locGoodsDic == null)
                {
                    locGoodsDic = await _zjnWmsMaterialInventoryRepository.AsQueryable()
                        .InnerJoin<ZjnWmsLocationEntity>((a, b) => a.ProductsLocation == b.LocationNo
                        && a.IsDeleted == 0 && b.IsDelete == 0)
                        .Where((a, b) => b.AisleNo == location.AisleNo
                        && (b.Row == 1 || b.Row == 4)).ToDictionaryAsync(k => k.ProductsLocation, v => v.ProductsCode);
                }

                if (side.LeftSide > 1)
                {
                    if (location.Row == 2)
                    {
                        if (!locGoodsDic.ContainsKey(location.LocationNo))
                        {
                            continue;
                        }
                        else
                        {
                            if (locGoodsDic[location.LocationNo].ToString() == goodsCode)
                            {
                                return location;
                            }
                            else
                            {
                                if (selectedLocation == null)
                                {
                                    selectedLocation = location;
                                }
                                continue;
                            }
                        }
                    }
                }
                if (side.RightSide > 1)
                {
                    if (location.Row == 3)
                    {
                        if (!locGoodsDic.ContainsKey(location.LocationNo))
                        {
                            continue;
                        }
                        else
                        {
                            if (locGoodsDic[location.LocationNo].ToString() == goodsCode)
                            {
                                return location;
                            }
                            else
                            {
                                if (selectedLocation == null)
                                {
                                    selectedLocation = location;
                                }
                                continue;
                            }
                        }
                    }
                }
                #endregion
            }
            if (needMove)
            {
                var moveConfigId = await _zjnWcsProcessConfigRepository.GetFirstAsync(x => x.WorkType == 3);//移库
                if (moveConfigId == null)
                {
                    throw HSZException.Oh("未配置移库业务流程配置");
                }
                //await _zjnTaskService.CreateByConfigId(moveConfigId.Id, "分配货位移库任务");
                //2022-11-8 update by yml
                ZjnWmsTaskCrInput taskInput = new ZjnWmsTaskCrInput
                {
                    taskName = "分配货位移库任务"
                };
                await _zjnTaskService.CreateByConfigId(moveConfigId.Id, taskInput);
            }
            return selectedLocation;
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
                var goodsCode = string.Empty;
                var trayGoods = await _zjnWmsTrayGoodsRepository.GetFirstAsync(w => w.TrayNo == dto.trayNo && w.IsDeleted == 0);
                if (trayGoods != null)
                {
                    goodsCode = trayGoods.GoodsCode;
                }
                var equip = await _zjnWmsEquipmentListRepository.GetFirstAsync(w => w.EquipmentSerialNumber == dto.taskDetailsStart);
                if (equip == null)
                {
                    throw HSZException.Oh("未配置入库口巷道绑定关系");
                }
                var roadwayGroupNo = equip.TheBinding;
                var locationGroup = await _zjnWmsLocationGroupRepository.GetFirstAsync(w => w.GroupName == Convert.ToString(tray.Type) && w.EnabledMark == 1);
                var groupDetails = new List<ZjnWmsLocationGroupDetailsEntity>();
                if (locationGroup != null)
                {
                    groupDetails = await _zjnWmsLocationGroupDetailsRepository.GetListAsync(w => w.GroupCode == locationGroup.GroupNo && w.EnabledMark == 1);
                }
                var exp = Expressionable.Create<ZjnWmsLocationEntity>();
                foreach (var group in groupDetails)
                {
                    exp.Or(x => x.Row >= group.StartRow && x.Row <= group.EndRow &&
                    x.Cell >= group.StartCell && x.Cell <= group.EndCell &&
                    x.Layer >= group.StartLayer && x.Layer <= group.EndLayer &&
                    x.Depth >= group.StartDepth && x.Depth <= group.EndDepth);
                }
                var locations = await _zjnWmsLocationRepository.AsQueryable()
                    .Where(w => w.LocationStatus == "0")//空货位
                    .Where(w => w.AisleNo == roadwayGroupNo)
                    .WhereIF(locationGroup != null && groupDetails.Count != 0, exp.ToExpression())
                    .PartitionBy(w => new { w.Layer }).Take(_locationGenerator.MaxPerLayer)//用PartitionBy必须Take，源码默认了Take(1)
                    .OrderBy(o => o.Cell, OrderByType.Desc)
                    .OrderBy(o => o.Priority, OrderByType.Desc)
                    .OrderBy(o => o.Layer)
                    .ToListAsync();
                (ZjnWmsLocationEntity, string) locationRes = (null, null);
                if (locations.Count == 0)
                {
                    throw HSZException.Oh("货位已满");
                }
                else
                {
                    locationRes = await AllotLocation(locations, goodsCode, tray.Type);
                }
                _db.BeginTran();
                ZjnWmsLocationEntity location = locationRes.Item1;
                if (location == null)
                {
                    throw HSZException.Oh("未找到满足条件的货位");
                }
                await _zjnWmsLocationAutoService.UpdateLocationStatus(location.LocationNo, LocationStatus.Order);
                if (!string.IsNullOrEmpty(locationRes.Item2))
                {
                    await _zjnWmsLocationAutoService.UpdateLocationStatus(locationRes.Item2, LocationStatus.Order);
                }

                var workSiteInfo = await _zjnWcsWorkSiteRepository.GetFirstAsync(w => w.DeviceId == dto.taskDetailsStart);
                if (workSiteInfo == null)
                {
                    throw HSZException.Oh("未找到入库台工作站点");
                }
                var aisleInfo = await _zjnWmsAisleRepository.GetFirstAsync(w => w.AisleNo == roadwayGroupNo);
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
        /// 结构件找空托   
        /// </summary>
        /// <param name="trayType">托盘类型 1.小托盘 2.大托盘</param>
        /// <param name="deviceId">设备id（缓存位设备）</param>
        /// <returns></returns>
        [NonAction]
        public async Task<ZjnWmsLocationEntity> FindEmptyTrayLocation(int trayType, string deviceId)
        {
            try
            {
                var locations = await _zjnWmsMaterialInventoryRepository.AsQueryable()
                    .InnerJoin<ZjnWmsLocationEntity>((a, b) => a.ProductsLocation == b.LocationNo && a.IsDeleted == 0
                    && b.IsDelete == 0 && b.EnabledMark == 1)
                    .InnerJoin<ZjnWmsAisleEntity>((a, b, c) => b.AisleNo == c.AisleNo && c.EnabledMark == 1)
                    .InnerJoin<ZjnWmsTrayEntity>((a, b, c, d) => a.ProductsContainer == d.TrayNo && d.EnabledMark == 1)
                    .Where((a, b, c, d) => b.LocationStatus == "1" && a.ProductsCode == string.Empty
                    && d.Type == trayType).OrderBy(a => a.CreateTime, OrderByType.Asc)
                    .Select((a, b) => b).ToListAsync();
                if (locations.Count == 0)
                {
                    throw HSZException.Oh("未找到货位");
                }
                var index = 0;
                var firstLocationNo = string.Empty;
                Dictionary<string, object> locDic = new Dictionary<string, object>();
                while (locations[index].Depth == 1)//深度为1先判断外面是否有预约
                {
                    var location = locations[index];
                    if (locDic.Count == 0)
                    {
                        locDic = await _zjnWmsLocationRepository.AsQueryable()
                            .Where(w => w.Depth == 0 && w.IsDelete == 0)
                            .ToDictionaryAsync(k => k.LocationNo, v => v.LocationStatus);
                    }

                    if (location.Row == 1)
                    {
                        var outerLocationNo = $"{location.ByWarehouse}-{location.AisleNo}-002-{location.Cell.ToString("d3")}-{location.Layer.ToString("d3")}";
                        if (locDic[outerLocationNo].ToString() == "0")
                        {
                            await _zjnWmsLocationAutoService.UpdateLocationStatus(outerLocationNo, LocationStatus.Lock);//锁定外层货位，完成后解锁
                            break;
                        }
                        if (index == 0)
                        {
                            firstLocationNo = outerLocationNo;
                        }
                    }
                    if (location.Row == 4)
                    {
                        var outerLocationNo = $"{location.ByWarehouse}-{location.AisleNo}-003-{location.Cell.ToString("d3")}-{location.Layer.ToString("d3")}";
                        if (locDic[outerLocationNo].ToString() == "0")
                        {
                            await _zjnWmsLocationAutoService.UpdateLocationStatus(outerLocationNo, LocationStatus.Lock);//锁定外层货位，完成后解锁
                            break;
                        }
                        if (index == 0)
                        {
                            firstLocationNo = outerLocationNo;
                        }
                    }
                    index++;
                    if (index >= locations.Count)//需要考虑生成任务时和执行任务时时间差引起的数据变化,这里控制好优先级，深度的优先级必须更高
                    {
                        if (locDic[firstLocationNo].ToString() == "1")//之前逻辑不对，进是很正常的，但没必要移库
                        {
                            return await _zjnWmsLocationRepository.GetFirstAsync(x => x.LocationNo == firstLocationNo);
                            //var moveConfigId = await _zjnWcsProcessConfigRepository.GetFirstAsync(x => x.WorkType == 3);//移库
                            //if (moveConfigId == null)
                            //{
                            //    throw HSZException.Oh("未配置移库业务流程配置");
                            //}
                            ////await _zjnTaskService.CreateByConfigId(moveConfigId.Id, "分配货位移库任务");
                            ////2022-11-8 update by yml
                            //ZjnWmsTaskCrInput taskInput = new ZjnWmsTaskCrInput
                            //{
                            //    taskName = "分配货位移库任务"
                            //};
                            //await _zjnTaskService.CreateByConfigId(moveConfigId.Id, taskInput);
                            //return locations[0];//只有已满这种情况确定需要移库
                        }
                        //空托无需考虑其他情况
                    }
                }
                return locations[index];
            }
            catch (Exception ex)
            {

                throw HSZException.Oh("未找到货位" + ex.Message);
            }
        }

        /// <summary>
        /// 结构件库找出库货位   
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
            if (trayCodes.Length == 0 && string.IsNullOrEmpty(TaskCrInput.batchNo) && string.IsNullOrEmpty(TaskCrInput.billNo))
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
                    .WhereIF(trayCodes.Length == 0 && string.IsNullOrEmpty(TaskCrInput.batchNo) && string.IsNullOrEmpty(TaskCrInput.billNo)
                    , (a, b, c, d) => b.LocationStatus == "1" && a.ProductsCode == TaskCrInput.materialCode)
                    .WhereIF(trayCodes.Length > 0, (a, b, c, d) => b.LocationStatus == "1" && trayCodes.Contains(a.ProductsContainer))
                    .WhereIF(!string.IsNullOrEmpty(TaskCrInput.batchNo), a => a.ProductsBatch == TaskCrInput.batchNo)
                    //.WhereIF(!string.IsNullOrEmpty(TaskCrInput.billNo), a => a.ProductsBill == TaskCrInput.billNo)
                    .OrderBy((a, b) => b.Depth).OrderBy(a => a.CreateTime, OrderByType.Asc)
                    .Select((a, b) => new { a, b }).ToListAsync();//深度的靠后处理
            List<ZjnWmsLocationEntity> locations;
            if (trayCodes.Length == 0)
            {
                if (!string.IsNullOrEmpty(TaskCrInput.batchNo) || !string.IsNullOrEmpty(TaskCrInput.billNo))
                {
                    locations = datas.Select(s => s.b).ToList();
                }
                else
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
            }
            else
            {
                locations = datas.Select(s => s.b).ToList();
            }
            if (locations.Count == 0)
            {
                throw HSZException.Oh("未找到货位");
            }
            int priority = 1;
            Dictionary<string, object> locDic = new Dictionary<string, object>();
            var moveConfigId = await _zjnWcsProcessConfigRepository.GetFirstAsync(x => x.WorkType == 3);//移库
            if (moveConfigId == null)
            {
                throw HSZException.Oh("未配置移库业务流程配置");
            }
            //var aisleDic = await _zjnWmsAisleRepository.AsQueryable().ToDictionaryAsync(k => k.AisleNo, v => v);
            //var stackerTasks = await _zjnTaskListDetailsRepository.AsQueryable().Where(w => w.TaskType == 5 && w.TaskDetailsStates == 2).ToListAsync();
            //需要考虑当前堆垛机作业模式为入库引起异常
            int index = 0;
            foreach (var location in locations)
            {
                if (location.Depth == 1)//深度为1先判断外面是否有预约
                {
                    if (locDic.Count == 0)
                    {
                        locDic = await _zjnWmsLocationRepository.AsQueryable()
                            .Where(w => w.Depth == 0 && w.IsDelete == 0)
                            .ToDictionaryAsync(k => k.LocationNo, v => v.LocationStatus);
                    }

                    if (location.Row == 1 || location.Row == 4)
                    {
                        var row = location.Row == 1 ? "002" : "003";
                        var outerLocationNo = $"{location.ByWarehouse}-{location.AisleNo}-{row}-{location.Cell.ToString("d3")}-{location.Layer.ToString("d3")}";
                        if (locDic[outerLocationNo].ToString() == "0" || locDic[outerLocationNo].ToString() == "7"
                            || locDic[outerLocationNo].ToString() == "8")//外部没库存，可先占用
                        {
                            priority = 1;
                        }
                        else if (locDic[outerLocationNo].ToString() == "1" || locDic[outerLocationNo].ToString() == "2")
                        {
                            priority = 1;
                            if (!locations.Any(a => a.LocationNo == outerLocationNo))//外面的已分配，先出外面的,未分配则移库
                            {
                                //await _zjnTaskService.CreateByConfigId(moveConfigId.Id, "分配货位移库任务");
                                //2022-11-8 update by yml
                                ZjnWmsTaskCrInput taskInput = new ZjnWmsTaskCrInput
                                {
                                    taskName = "分配货位移库任务",
                                    positionFrom = outerLocationNo,
                                    positionCurrent = location.LocationNo,
                                    priority = 3,
                                };
                                await _zjnTaskService.CreateByConfigId(moveConfigId.Id, taskInput);
                            }
                        }
                        else
                        {
                            throw HSZException.Oh("原材料库无此业务");
                        }

                    }
                }
                else
                {
                    priority = 2;//外面的优先级要高
                }
                await _zjnWmsLocationAutoService.UpdateLocationStatus(location.LocationNo, LocationStatus.Lock);
                res.Add(new ZjnWmsTaskCrInput()
                {
                    priority = priority,
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
        /// 找消防货位，优先找距离近的消防位置
        /// </summary>
        /// <param name="dto">消防货位</param>
        /// <returns></returns>
        [NonAction]
        public async Task<ZjnWmsTaskDetailsInfoOutput> FindFireControlLocation(ZjnWmsTaskDetailsInfoOutput dto)
        {

            ZjnWmsLocationEntity output = await _zjnWmsLocationRepository.GetFirstAsync(x => x.LocationNo == "");
            return dto;
        }

        /// <summary>
        /// 寻找移库货位（暂不考虑跨巷道）
        /// </summary>
        /// <param name="input">移库信息</param>
        /// <returns>终点货位编码</returns>
        [NonAction]
        public async Task<string> FindMoveLocation(ZjnWmsTaskCrInput input)
        {
            var locationNo = input.positionFrom;
            var location = await _zjnWmsLocationRepository.GetFirstAsync(x => x.LocationNo == locationNo);
            var nearDic = await _zjnWmsLocationRepository.AsQueryable()
                .Where(w => w.EnabledMark == 1 && w.IsDelete == 0
                //&& w.LocationStatus == "0"
                && w.AisleNo == location.AisleNo && w.LocationNo != locationNo)
                .OrderBy(o => Math.Abs(o.Cell - location.Cell))
                .OrderBy(o => Math.Abs(o.Row - location.Row))
                .OrderBy(o => Math.Abs(o.Layer - location.Layer)).ToDictionaryAsync(k => k.LocationNo, v => v);
            var emptyDic = nearDic.Where(w => (w.Value as ZjnWmsLocationEntity).LocationStatus == "0");
            foreach (var emptyPair in emptyDic)
            {
                var emptyLoc = emptyPair.Value as ZjnWmsLocationEntity;
                if (emptyLoc.Row == 2 || emptyLoc.Row == 3)
                {
                    var row = emptyLoc.Row == 2 ? "001" : "004";
                    var innerLocationNo = $"{emptyLoc.ByWarehouse}-{emptyLoc.AisleNo}-{row}-{emptyLoc.Cell.ToString("d3")}-{emptyLoc.Layer.ToString("d3")}";
                    if (!emptyDic.Any(a => a.Key == innerLocationNo))//深度1有货
                    {
                        var innerLoc = nearDic[innerLocationNo] as ZjnWmsLocationEntity;
                        if (innerLoc.LocationStatus != "8")
                        {
                            await _zjnWmsLocationAutoService.UpdateLocationStatus(emptyPair.Key, LocationStatus.Order);
                            return emptyPair.Key;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else//深度1无货，先分配深度1
                    {
                        continue;
                    }
                }
                else
                {
                    var row = emptyLoc.Row == 1 ? "002" : "003";
                    var outerLocationNo = $"{emptyLoc.ByWarehouse}-{emptyLoc.AisleNo}-{row}-{emptyLoc.Cell.ToString("d3")}-{emptyLoc.Layer.ToString("d3")}";
                    if (!emptyDic.Any(a => a.Key == outerLocationNo))//深度0有货
                    {
                        continue;
                    }
                    else//深度0无货，先分配深度1
                    {
                        await _zjnWmsLocationAutoService.UpdateLocationStatus(emptyPair.Key, LocationStatus.Order);
                        return emptyPair.Key;
                    }
                }
            }
            //需上报异常
            throw HSZException.Oh("未找到移库货位，请出库");
        }
    }



}
