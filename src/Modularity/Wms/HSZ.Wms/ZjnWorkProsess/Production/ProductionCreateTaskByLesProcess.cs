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
using HSZ.JsonSerialization;
using static System.Net.Mime.MediaTypeNames;
using ZJN.Calb.Entitys.Dto;
using HSZ.Common.TaskResultPubilcParms;

namespace HSZ.Wms.ZjnWorkProsess
{
    /// <summary>
    /// Les接口处理类>>>>一体化出入库任务创建、出入库申请、出入库确认、任务取消
    /// </summary>
    [ApiDescriptionSettings(Tag = "CreateTaskByLes", Name = "CreateTaskByLes", Order = 220)]
    [Route("api/[controller]")]
    [WareDI(WareType.Production)]
    public class ProductionCreateTaskByLesProcess : IProductionCreateTaskByLesProcess, IDynamicApiController, ITransient
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

        /// <summary>
        /// 初始化
        /// </summary>
        public ProductionCreateTaskByLesProcess(
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
            ISqlSugarRepository<ZjnWmsMaterialInventoryEntity> zjnWmsMaterialInventoryRepository)
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
        }

        /// <summary>
        /// 一体化出入库初始化
        /// </summary>
        /// <param name="json">Les回传数据</param>
        /// <returns></returns>
        [NonAction]
        public async Task CreateTaskByLesInit(string json)
        {
            TaskWarehouseResponse response = new TaskWarehouseResponse();
            string errorMessage = "";
            try
            {
                TaskWarehouseRequest request = json?.Deserialize<TaskWarehouseRequest>();
                if (request == null)
                {
                    //调用Les异常接口
                    errorMessage = "参数传递有误，请检查,错误代码001";
                    return;
                }
                string operationDirection = request.operationDirection;//操作方向 out出库 into入库 move移库 dispatch调拨
                string operationType = request.operationType;//操作类型 emptyContainer空托 production产品
                if (string.IsNullOrEmpty(operationDirection) || string.IsNullOrEmpty(operationType))
                {
                    //调用Les异常接口
                    errorMessage = "参数传递有误，请检查,错误代码002";
                    return;
                }

                switch (operationDirection.ToUpper())
                {
                    case "INTO"://入库

                        break;
                    case "OUT": //出库 1.物料+数量 2.物料+数量+批次 3.物料+数量+容器 4.物料+数量+等级
                        string materialCode = request.materialCode;//物料编码
                        string qty = request.qty;//数量

                        if (string.IsNullOrEmpty(materialCode) || string.IsNullOrEmpty(materialCode))
                        {
                            //调用Les异常接口
                            errorMessage = "参数传递有误，请检查,错误代码003";
                            return;
                        }
                        await OutHouse(request);
                        break;
                    case "MOVE"://移动

                        break;
                    case "DISPATCH"://调拨

                        break;
                    default:
                        break;
                }




            }
            catch (Exception)
            {

                throw;
            }



        }


        /// <summary>
        /// Lse出库业务处理
        /// </summary>
        /// <returns></returns>
        public async Task OutHouse(TaskWarehouseRequest request) 
        {
            string errorMessage = "";
            try
            {
                string materialCode = request.materialCode;//物料编码
                string qty = request.qty;//数量
                string batchNo = request.batchNo;//批次
                List<TaskWarehouseRequestLine> diskInfoList = request.diskInfoList;//电池信息
                string batteryGradeNo = request.batteryGradeNo;//等级

                var materialCount =  _zjnWmsMaterialInventoryRepository.GetListAsync(x => x.ProductsCode == materialCode);

                if (!string.IsNullOrEmpty(batchNo))//按照批次出库
                {
                    //1.库存校验
                    decimal proCount = await _zjnWmsMaterialInventoryRepository.AsSugarClient().Queryable<ZjnWmsMaterialInventoryEntity>().Where(a => a.ProductsState == "1" && a.ProductsIsLock == 1 && a.ProductsBatch == batchNo).SumAsync(a => a.ProductsQuantity);
                    if (proCount < Convert.ToDecimal(qty))
                    {
                        //调用Les异常接口
                        errorMessage = "库存可用数量不足";
                        throw HSZException.Oh("Les任务创建失败，库存可用数量不足");
                    }

                    //2.获取任务流程
                    var processEntity = await _zjnTaskListRepository.AsSugarClient()
                            .Queryable<ZjnWcsProcessConfigEntity>()
                            .Where(x => x.WorkType == (int)TaskProcessType.Out && x.WorkEnd.Contains(request.toLocNo)).FirstAsync();
                    if (processEntity == null) 
                    {
                        errorMessage = "WMS系统未找到业务流程";
                        throw HSZException.Oh("Les任务创建失败，找不到业务流程");
                    }
                    //3.根据物料+批次+数量 寻找货位


                    //4.派发任务
                    List<ZjnWmsLocationEntity> locationList = new List<ZjnWmsLocationEntity>();
                    foreach (var item in locationList)
                    {
                        
                    }


                }
                else if (diskInfoList.Count > 0)//按照托盘出库
                {

                }
                else if (!string.IsNullOrEmpty(batteryGradeNo))//按照等级出库
                {

                }
                else
                {

                }






            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
