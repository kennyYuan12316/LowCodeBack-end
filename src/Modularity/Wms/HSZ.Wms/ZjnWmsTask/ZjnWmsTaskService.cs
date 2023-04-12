using HSZ.Common.Const;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Common.TaskResultPubilcParms;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.Logging.Attributes;
using HSZ.System.Entitys.System;
using HSZ.UnifyResult;
using HSZ.wms.Entitys.Dto.zjnWcsProcessConfig;
using HSZ.wms.Entitys.Dto.ZjnWmsRunLoginfo;
using HSZ.wms.Entitys.Dto.ZjnWmsTask;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using HSZ.wms.Entitys.Dto.ZjnWmsTrayLocationLog;
using HSZ.wms.Interfaces.ZjnServicePathConfig;
using HSZ.wms.Interfaces.ZjnWmsLocation;
using HSZ.wms.Interfaces.ZjnWmsTask;
using HSZ.wms.ZjnWmsRunLoginfo;
using HSZ.Wms.Interfaces.ZjnWorkProsess;
using HSZ.Wms.ZjnWmsWorkPathAffirm;
using HSZ.WorkFlow.Entitys.Model.Properties;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using ZJN.Calb.Entitys.Dto;
using DateTime = System.DateTime;

namespace HSZ.wms.ZjnWmsTask
{
    /// <summary>
    /// 主任务服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnWmsTask", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnTaskService : IZjnWmsTaskService, IDynamicApiController, IScoped
    {
        private readonly ISqlSugarRepository<ZjnWmsTrayEntity> _zjnBaseTrayRepository;
        private readonly ISqlSugarRepository<ZjnWmsTaskEntity> _zjnTaskListRepository;
        private readonly ISqlSugarRepository<ZjnWmsTaskDetailsEntity> _zjnTaskListDetailsRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private readonly IZjnWcsProcessConfigService _zjnServicePathConfigService;
        private readonly ICacheManager _cacheManager;
        private readonly ISqlSugarRepository<ZjnWmsGoodsEntity> _zjnSugarRepository;
        private readonly IServiceProvider _services;
        private readonly ZjnWmsWorkPathAffirm _WmsWorkPathAffirm;
        private readonly ISqlSugarRepository<DictionaryDataEntity> _dictionaryDataRepository;// 数据字典表仓储
        private readonly ISqlSugarRepository<ZjnWmsAisleEntity> _zjnWmsAisleEntity;
        private readonly ZjnWmsRunLoginfoService _zjnWmsRunLoginfoService;
        private readonly ISqlSugarRepository<ZjnWmsTrayLocationLogEntity> _zjnWmsTrayLocationLogEntity;
        private readonly ISqlSugarRepository<ZjnWmsTrayGoodsEntity> _zjnWmsTrayGoodsEntity;

        private readonly ISqlSugarRepository<ZjnWmsLocationEntity> _zjnWmsLocationRepository;
        private readonly IZjnWmsLocationAutoService _zjnWmsLocationAutoService;
        private readonly ISqlSugarRepository<ZjnWcsWorkSiteEntity> _zjnWcsWorkSiteRepository;
        private readonly ISqlSugarRepository<ZjnWmsEquipmentListEntity> _zjnWmsEquipmentListRepository;
        private readonly ISqlSugarRepository<ZjnWmsLineSettingEntity> _zjnWmsLineSettingEntity;
        private readonly ISqlSugarRepository<ZjnWmsLineSettinglogEntity> _zjnWmsLineSettinglogEntity;


        private readonly ISqlSugarRepository<ZjnWmsMaterialInventoryEntity> _zjnWmsMaterialInventoryRepository;
        //分发业务类

        //private readonly PlaneDeviceProcess _PlaneDeviceProcess;
        //private readonly RgvDeviceProcess _RgvDeviceProcess;
        //private readonly StackerOutProcess _StackerOutProcess;
        //private readonly StackerInProcess _StackerInProcess;
        //private readonly StackerMoveProcess _StackerMoveProcess;
        //private readonly WarehousInProcess _WarehousInProcess;
        //private readonly WarehousOutProcess _WarehousOutProcess;
        //private readonly FindLocationProcess _FindLocationProcess;

        /// <summary>
        /// 初始化一个<see cref="ZjnTaskService"/>类型的新实例
        /// </summary>
        public ZjnTaskService(ISqlSugarRepository<ZjnWmsTaskEntity> zjnTaskListRepository,
            ISqlSugarRepository<ZjnWmsMaterialInventoryEntity> zjnWmsMaterialInventoryRepository,
            ISqlSugarRepository<ZjnWmsLocationEntity> zjnWmsLocationRepository,
            ISqlSugarRepository<ZjnWmsAisleEntity> zjnWmsAisleEntity,
            ISqlSugarRepository<ZjnWmsTrayEntity> zjnBaseTrayRepository,
            ISqlSugarRepository<DictionaryDataEntity> dictionaryDataRepository,
            IUserManager userManager,
            ISqlSugarRepository<ZjnWmsTaskDetailsEntity> zjnTaskListDetailsRepository,
            IZjnWcsProcessConfigService zjnServicePathConfigService,
            ISqlSugarRepository<ZjnWmsGoodsEntity> zjnSugarRepository,
            ZjnWmsWorkPathAffirm WmsWorkPathAffirm,
            ICacheManager cacheManager,
            IServiceProvider serviceProvider,
            ZjnWmsRunLoginfoService zjnWmsRunLoginfoService,
            ISqlSugarRepository<ZjnWmsTrayLocationLogEntity> zjnWmsTrayLocationLogEntity,
            IZjnWmsLocationAutoService zjnWmsLocationAutoService,
            ISqlSugarRepository<ZjnWcsWorkSiteEntity> zjnWcsWorkSiteRepository,
            ISqlSugarRepository<ZjnWmsEquipmentListEntity> zjnWmsEquipmentListRepository,
            ISqlSugarRepository<ZjnWmsTrayGoodsEntity> zjnWmsTrayGoodsEntity,
            ISqlSugarRepository<ZjnWmsLineSettingEntity> zjnWmsLineSettingEntity,
            ISqlSugarRepository<ZjnWmsLineSettinglogEntity> zjnWmsLineSettinglogEntity
            //PlaneDeviceProcess planeDeviceProcess,
            //WeighDeviceProcess weighDeviceProcess,
            //RgvDeviceProcess rgvDeviceProcess,
            //StackerOutProcess stackerOutProcess,
            //StackerInProcess stackerInProcess,
            //StackerMoveProcess stackerMoveProcess,
            //WarehousInProcess warehousInProcess,
            //WarehousOutProcess warehousOutProcess,
            //FindLocationProcess findLocationProcess
            )
        {

            _zjnWmsMaterialInventoryRepository = zjnWmsMaterialInventoryRepository;
            _zjnWmsLocationRepository = zjnWmsLocationRepository;

            _zjnWmsAisleEntity = zjnWmsAisleEntity;
            _zjnBaseTrayRepository = zjnBaseTrayRepository;
            _dictionaryDataRepository = dictionaryDataRepository;
            _WmsWorkPathAffirm = WmsWorkPathAffirm;
            _zjnSugarRepository = zjnSugarRepository;
            _zjnTaskListRepository = zjnTaskListRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _zjnTaskListDetailsRepository = zjnTaskListDetailsRepository;
            _zjnServicePathConfigService = zjnServicePathConfigService;
            _cacheManager = cacheManager;
            _services = serviceProvider;
            //_PlaneDeviceProcess= planeDeviceProcess;
            //_WeighDeviceProcess= weighDeviceProcess;
            //_RgvDeviceProcess= rgvDeviceProcess;
            //_StackerInProcess = stackerInProcess;
            //_StackerOutProcess = stackerOutProcess;
            //_StackerMoveProcess = stackerMoveProcess;
            //_WarehousInProcess = warehousInProcess;
            //_WarehousOutProcess = warehousOutProcess;
            //_FindLocationProcess = findLocationProcess;

            _zjnWmsRunLoginfoService = zjnWmsRunLoginfoService;
            _zjnWmsLocationAutoService = zjnWmsLocationAutoService;
            _zjnWcsWorkSiteRepository = zjnWcsWorkSiteRepository;
            _zjnWmsEquipmentListRepository = zjnWmsEquipmentListRepository;
            _zjnWmsTrayGoodsEntity = zjnWmsTrayGoodsEntity;
            _zjnWmsLineSettingEntity = zjnWmsLineSettingEntity;
            _zjnWmsLineSettinglogEntity = zjnWmsLineSettinglogEntity;
        }

        /// <summary>
        /// 获取主任务
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var sss = MethodBase.GetCurrentMethod();
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "获取主任务";
            logInfo.methodName = "GetInfo";
            logInfo.methodParmes = id;
            logInfo.isBug = 0;
            try
            {
                var output = (await _zjnTaskListRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsTaskInfoOutput>();
                return output;
            }
            catch (Exception ex)
            {
                logInfo.isBug = 1;
                logInfo.message = ex.Message;
                return null;
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }

        /// <summary>
		/// 获取主任务列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsTaskQueryInput input)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "获取主任务列表";
            logInfo.methodName = "GetList";
            logInfo.methodParmes = JsonConvert.SerializeObject(input);
            logInfo.taskNo = input.F_TaskNo;

            logInfo.isBug = 0;
            try
            {

                var sidx = input.sidx == null ? "F_Id" : input.sidx;
                var data = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWmsTaskEntity>()
                    .WhereIF(!string.IsNullOrEmpty(input.F_TaskNo), a => a.TaskNo.Contains(input.F_TaskNo))
                    .WhereIF(!string.IsNullOrEmpty(input.F_TaskName), a => a.TaskName.Contains(input.F_TaskName))
                    .WhereIF(!string.IsNullOrEmpty(input.F_TaskDescribe), a => a.TaskDescribe.Contains(input.F_TaskDescribe))
                    .Select((a
                    ) => new ZjnWmsTaskListOutput
                    {
                        F_Id = a.Id,
                        F_TaskNo = a.TaskNo,
                        F_TaskName = a.TaskName,
                        F_PositionFrom = a.PositionFrom,
                        F_PositionFromName = SqlFunc.Subqueryable<ZjnWcsWorkSiteEntity>().Where(s => s.StationId == a.PositionFrom).Select(s => s.Capion),
                        F_PositionTo = a.PositionTo,
                        F_PositionToName = SqlFunc.Subqueryable<ZjnWcsWorkSiteEntity>().Where(s => s.StationId == a.PositionTo).Select(s => s.Capion),
                        F_PositionCurrent = a.PositionCurrent,
                        F_RouteNo = a.RouteNo,
                        F_CreateUser = a.CreateUser,
                        F_CreateTime = a.CreateTime,
                        F_LastModifyUserId = a.LastModifyUserId,
                        F_LastModifyTime = a.LastModifyTime,
                        F_EnabledMark = a.EnabledMark,
                        F_TaskFrom = a.TaskFrom,
                        F_TaskState = a.TaskState,
                        F_TaskTypeNum = a.TaskTypeNum,
                        F_TaskTypeName = SqlFunc.Subqueryable<ZjnWcsProcessConfigEntity>().Where(s => s.Id == a.TaskTypeNum).Select(s => s.WorkName),
                        F_TaskDescribe = a.TaskDescribe,
                        F_OrderNo = a.OrderNo,
                        F_MaterialCode = a.MaterialCode,
                        F_BillNo = a.BillNo,
                        F_Priority = a.Priority,
                        F_Quantity = a.Quantity,
                        F_TrayNo = a.TrayNo
                    })
                    .Where(x => x.F_TaskState != 3)
                    .OrderBy(sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnWmsTaskListOutput>.SqlSugarPageResult(data);
            }
            catch (Exception ex)
            {
                logInfo.isBug = 1;
                logInfo.message = ex.Message;
                return null;
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }

        /// <summary>
        /// 任务详情数据
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("taskData")]
        public async Task<ZjnWcsProcessConfigJsonOutput> taskData(string id)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "任务详情数据";
            logInfo.methodName = "taskData";
            logInfo.methodParmes = id;
            logInfo.taskNo = "";
            logInfo.isBug = 0;
            try
            {
                ZjnWcsProcessConfigJsonOutput output = new ZjnWcsProcessConfigJsonOutput();
                var zjnTaskInfoOutput = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWmsTaskEntity, ZjnWcsProcessConfigEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.TaskTypeNum.ToString() == b.Id))
                   .Where(a => a.Id == id)
                   .Select((a, b) => new ZjnWmsTaskCrInput
                   {
                       id = a.Id,
                       taskNo = a.TaskNo,
                       taskName = a.TaskName,
                       taskDescribe = a.TaskDescribe,
                       enabledMark = a.EnabledMark,
                       taskFrom = a.TaskFrom,
                       taskState = a.TaskState,
                       createTime = a.CreateTime,
                       createUser = a.CreateUser,
                       lastModifyTime = a.LastModifyTime,
                       lastModifyUserId = a.LastModifyUserId,
                       positionCurrent = a.PositionCurrent,
                       positionCurrentName = SqlFunc.Subqueryable<ZjnWcsWorkSiteEntity>().Where(s => s.StationId == a.PositionCurrent && SqlFunc.IsNull(s.IsDelete, 0) == 0).Select(s => s.Capion),
                       positionFrom = a.PositionFrom,
                       positionFromName = SqlFunc.Subqueryable<ZjnWcsWorkSiteEntity>().Where(s => s.StationId == a.PositionTo && SqlFunc.IsNull(s.IsDelete, 0) == 0).Select(s => s.Capion),
                       positionTo = a.PositionTo,
                       positionToName = SqlFunc.Subqueryable<ZjnWcsWorkSiteEntity>().Where(s => s.StationId == a.PositionTo && SqlFunc.IsNull(s.IsDelete, 0) == 0).Select(s => s.Capion),
                       taskTypeNum = a.TaskTypeNum,
                       taskTypeName = b.WorkName,
                       workPath = b.WorkPath,

                       orderNo = a.OrderNo,
                       materialCode = a.MaterialCode,
                       quantity = a.Quantity,
                       billNo = a.BillNo,
                       priority = a.Priority,
                       trayNo = a.TrayNo,




                   }).FirstAsync();

                if (zjnTaskInfoOutput == null)
                {
                    logInfo.isBug = 1;
                    logInfo.message = "任务数据不存在";

                    throw HSZException.Oh("任务数据不存在");
                }
                //List<ZjnTaskListDetailsInfoOutput> ZjnTaskListDetailsList = new List<ZjnTaskListDetailsInfoOutput>();

                var ZjnTaskListDetailsList = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWmsTaskDetailsEntity>()
                  .Where(a => a.TaskId == zjnTaskInfoOutput.taskNo && (a.NodeType == "approver" || a.NodeType == "dynamic"))
                  .Select((a) => new ZjnWmsTaskDetailsInfoOutput
                  {
                      taskDetailsId = a.TaskDetailsId,
                      taskId = a.TaskId,
                      taskDetailsName = a.TaskDetailsName,
                      taskDetailsStart = a.TaskDetailsStart,
                      taskDetailsStartName = SqlFunc.Subqueryable<ZjnWcsWorkSiteEntity>().Where(s => s.StationId == a.TaskDetailsStart && SqlFunc.IsNull(s.IsDelete, 0) == 0).Select(s => s.Capion),

                      taskDetailsEnd = a.TaskDetailsEnd,
                      taskDetailsEndName = SqlFunc.Subqueryable<ZjnWcsWorkSiteEntity>().Where(s => s.StationId == a.TaskDetailsEnd && SqlFunc.IsNull(s.IsDelete, 0) == 0).Select(s => s.Capion),

                      taskDetailsMove = a.TaskDetailsMove,
                      taskDetailsMoveName = SqlFunc.Subqueryable<ZjnWcsWorkDeviceEntity>().Where(s => s.DeviceId == a.TaskDetailsMove && SqlFunc.IsNull(s.IsDelete, 0) == 0).Select(s => s.Caption),

                      workPathId = a.WorkPathId,
                      workPathname = SqlFunc.Subqueryable<ZjnWcsWorkPathEntity>().Where(s => s.PathId == a.WorkPathId && SqlFunc.IsNull(s.IsDelete, 0) == 0).Select(s => s.StationFrom),
                      taskDetailsStates = a.TaskDetailsStates,
                      taskType = a.TaskType,
                      rowStart = a.RowStart,
                      cellStart = a.CellStart,
                      layerStart = a.LayerStart,
                      rowEnd = a.RowEnd,
                      cellEnd = a.CellEnd,
                      layerEnd = a.LayerEnd,

                  }).ToListAsync();

                output.zjnTaskInfoOutput = zjnTaskInfoOutput;
                output.FlowTemplateJson = zjnTaskInfoOutput.workPath;
                output.ZjnTaskListDetailsList = ZjnTaskListDetailsList;

                return output;
            }
            catch (Exception ex)
            {
                logInfo.isBug = 1;
                logInfo.message = ex.Message;
                return null;
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }

        /// <summary>
        /// 通过业务ID新建主任务
        /// </summary>
        /// <param name="id">业务流程ID</param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost("CreateByConfigId")]
        public async Task<ZjnWmsTaskCrInput> CreateByConfigId(string id, ZjnWmsTaskCrInput taskInput)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "通过业务ID新建主任务";
            logInfo.methodName = "CreateByConfigId";
            logInfo.methodParmes = id + "-" + taskInput;
            logInfo.taskNo = "";
            logInfo.isBug = 0;
            try
            {
                ZjnWcsProcessConfigJsonOutput data = await _zjnServicePathConfigService.PathData(id);
                ZjnWmsTaskCrInput input = data.zjnTaskInfoOutput.Adapt<ZjnWmsTaskCrInput>();
                //input.taskName = name;

                input = taskInput.Adapt(input);
                input.taskTypeNum = id;
                input.taskList = data.ZjnTaskListDetailsList.Adapt<List<ZjnWmsTaskDetailsEntity>>();
                await this.Create(input);
                return input;
            }
            catch (Exception ex)
            {
                logInfo.isBug = 1;
                logInfo.message = ex.Message;
                throw HSZException.Oh(ex.Message);
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }

        /// <summary>
        /// 新建主任务 Les方式创建
        /// </summary>
        /// <param name="taskWarehouseRequest">参数</param>
        /// <returns></returns>
        [HttpPost("CreateByTaskWarehouse")]
        [AllowAnonymous, IgnoreLog]
        public async Task CreateByTaskWarehouse(TaskWarehouseRequest taskWarehouseRequest)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "新建主任务Les方式创建";
            logInfo.methodName = "CreateByTaskWarehouse";
            logInfo.methodParmes = JsonConvert.SerializeObject(taskWarehouseRequest);
            logInfo.taskNo = "";
            logInfo.isBug = 0;
            try
            {
                //TaskWarehouseRequest taskWarehouseRequest = JSON.Deserialize<TaskWarehouseRequest>(json);

                //string trayNo = taskWarehouseRequest.operationDirection;//出库/入库

                string configId = "344418982696060165";//业务配置id通过条件查询   逻辑暂定

                //if (taskWarehouseRequest.operationDirection == "into") {
                //    configId = "344418982696060165";
                //}
                if (!string.IsNullOrEmpty(taskWarehouseRequest.materialCode))
                {
                    if (!await _db.Queryable<ZjnWmsGoodsEntity>()
                        .AnyAsync(a => a.GoodsCode == taskWarehouseRequest.materialCode && a.IsDelete == 0))
                    {
                        logInfo.isBug = 1;
                        logInfo.message = "未找到托盘信息";

                        throw HSZException.Oh("未找到物料信息");
                    }
                }
                var trayNos = taskWarehouseRequest.diskInfoList.Select(s => s.trayBarcode);
                if (!await _db.Queryable<ZjnWmsTrayEntity>().AnyAsync(a => trayNos.Contains(a.TrayNo) && a.IsDelete == 0))
                {
                    logInfo.isBug = 1;
                    logInfo.message = "未找到托盘信息";
                    throw HSZException.Oh("未找到托盘信息");
                }
                //无法准确判断是否起点工位
                if (!await _db.Queryable<ZjnWcsWorkDeviceEntity>()
                    .AnyAsync(a => a.DeviceId == taskWarehouseRequest.fromLocNo && a.IsDelete == 0))
                {
                    logInfo.isBug = 1;
                    logInfo.message = "未找到起点工位信息";
                    throw HSZException.Oh("未找到起点工位信息");
                }
                //工位是否和物料匹配问题
                List<ZjnWmsTrayGoodsEntity> list = new List<ZjnWmsTrayGoodsEntity>();
                foreach (var item in taskWarehouseRequest.diskInfoList)
                {
                    ZjnWcsProcessConfigJsonOutput data = await _zjnServicePathConfigService.PathData(configId);
                    ZjnWmsTaskCrInput input = data.zjnTaskInfoOutput.Adapt<ZjnWmsTaskCrInput>();
                    input.taskName = "入库" + item.trayBarcode;
                    input.orderNo = taskWarehouseRequest.orderNo;
                    input.materialCode = taskWarehouseRequest.materialCode;
                    input.quantity = Convert.ToDecimal(taskWarehouseRequest.qty);
                    //input.billNo = taskWarehouseRequest.attribute1;//待确认
                    input.priority = Convert.ToInt32(taskWarehouseRequest.priority);
                    input.trayNo = item.trayBarcode;
                    input.positionFrom = taskWarehouseRequest.fromLocNo;
                    input.operationDirection = taskWarehouseRequest.operationDirection;
                    input.taskFrom = "LES";
                    if (!string.IsNullOrEmpty(input.materialCode))
                    {
                        list.Add(new ZjnWmsTrayGoodsEntity()
                        {
                            GoodsCode = input.materialCode,
                            TrayNo = input.trayNo,
                            Quantity = Convert.ToInt32(input.quantity),
                            Id = YitIdHelper.NextId().ToString(),
                            CreateTime = DateTime.Now,
                            IsDeleted = 0
                        });
                    }
                    input.taskList = data.ZjnTaskListDetailsList.Adapt<List<ZjnWmsTaskDetailsEntity>>();
                    await this.Create(input);
                }
                await _db.Insertable(list).ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                logInfo.isBug = 1;
                logInfo.message = ex.Message;
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }

        /// <summary>
        /// 新建主任务  WMS app可调用此方法
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWmsTaskCrInput input)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "新建主任务WMS-app可调用此方法";
            logInfo.methodName = "Create";
            logInfo.methodParmes = JsonConvert.SerializeObject(input);
            logInfo.taskNo = input.taskNo;
            logInfo.trayNo = input.trayNo;
            logInfo.isBug = 0;

            try
            {
                //防止重复生成任务
                var ifhaveTask = await _zjnTaskListRepository.AsQueryable()
                    .Where(l => l.TrayNo == input.trayNo && l.TaskState < 3)
                    .ToListAsync();
                if (ifhaveTask == null || ifhaveTask.Count == 0)
                {
                    if (input.operationDirection == "Into")
                    {
                        var ifInLocation = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWmsLocationEntity>()
                            .Where(l => l.TrayNo == input.trayNo && l.LocationStatus != "0").FirstAsync();
                        if (ifInLocation != null)
                        {
                            throw HSZException.Oh("此托盘已存在库里，不可生成入库任务");
                        }
                        var ifTrayInOnline = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWmsTrayEntity>()
                            .Where(l => l.TrayNo == input.trayNo && l.IsDelete == 1).ToListAsync();
                        if (ifTrayInOnline != null && ifhaveTask.Count != 0)
                        {
                            throw HSZException.Oh("此托盘已删除，不可生成入库任务");
                        }
                    }

                }
                else
                {
                    throw HSZException.Oh("此托盘任务已存在，不可重复生成");
                }
            }
            catch (Exception e)
            {
                logInfo.isBug = 1;
                logInfo.message = e.Message;
                await _zjnWmsRunLoginfoService.Create(logInfo);
                throw HSZException.Oh(e.Message);
            }
            //var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWmsTaskEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.TaskNo = _cacheManager.GettaskId().ToString();
            entity.TaskState = 1;
            entity.Priority = input.priority ?? 1;
            entity.PositionFrom = input.positionFrom;
            entity.PositionTo = input.positionTo;
            entity.BatchNo = input.batchNo;
            entity.BillNo = input.billNo;
            //entity.CreateUser = _userManager.UserId;
            entity.CreateUser = entity.TaskFrom;

            entity.CreateTime = DateTime.Now;
            List<ZjnWmsTaskDetailsEntity> list = input.taskList;
            string startNode = list.Where(x => x.NodeType == "start").FirstOrDefault().NodeCode;
            var endNodes = list.Where(x => string.IsNullOrEmpty(x.NodeNext));
            foreach (var item in list)
            {
                if (input.operationDirection == "Into")
                {
                    if (item.NodeCode == startNode)
                    {
                        //写在称重动态任务就好，动态任务就需要写逻辑找起点和终点，我的规划是配路径也行，配动态也行
                        //list.Where(l => l.NodeCode == item.NodeNext).ForEach(l =>
                        //{
                        //    l.TaskDetailsStart = entity.PositionFrom;
                        //    l.TaskDetailsEnd = entity.PositionFrom;
                        //    l.ResultValue = entity.PositionFrom;
                        //});
                        item.TaskDetailsStart = entity.PositionFrom;
                        //item.TaskDetailsEnd = entity.PositionTo;
                        //update by tml 2022-11-16 例：第一个任务是入库台，就直接找上一个任务的终点
                        item.TaskDetailsEnd = entity.PositionFrom;
                        //结束点位
                        var endSite = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWcsWorkSiteEntity>().Where(x => x.StationId == item.TaskDetailsEnd).FirstAsync();
                        if (endSite != null)
                        {
                            item.RowEnd = endSite.Row;
                            item.CellEnd = endSite.Cell;
                            item.LayerEnd = endSite.Layer;
                        }
                    }
                    item.ResultValue = entity.PositionFrom;
                }
                if (input.operationDirection == "Out")
                {
                    if (item.NodeCode == startNode)
                    {
                        item.TaskDetailsStart = entity.PositionFrom;
                        item.TaskDetailsEnd = entity.PositionTo;

                        //结束点位
                        var locationSite = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWmsLocationEntity>().Where(x => x.LocationNo == item.TaskDetailsEnd).FirstAsync();
                        if (locationSite != null)
                        {
                            item.RowEnd = locationSite.Row;
                            item.CellEnd = locationSite.Cell;
                            item.LayerEnd = locationSite.Layer;
                        }

                    }
                    if (endNodes.Any(a => a.NodeCode == item.NodeCode))
                    {
                        if (entity.PositionTo.Contains(','))
                        {
                            var endArr = entity.PositionTo.Split(',');
                            if (input.operationDirection == "Out")
                            {
                                endArr = _db.Queryable<ZjnWcsWorkDeviceEntity>()
                                    .Where(w => w.JobGroup != "Into" && endArr.Contains(w.DeviceId))
                                    .Select(s => s.DeviceId).ToArray();
                                if (endArr.Length == 0)
                                {
                                    throw HSZException.Oh("当前正在入库，暂无出库路线可分配");
                                }
                            }
                            if (input.operationDirection == "Into")
                            {
                                endArr = _db.Queryable<ZjnWcsWorkDeviceEntity>()
                                    .Where(w => w.JobGroup != "Out" && endArr.Contains(w.DeviceId))
                                    .Select(s => s.DeviceId).ToArray();
                                if (endArr.Length == 0)
                                {
                                    throw HSZException.Oh("当前正在出库，暂无入库路线可分配");
                                }
                            }
                            var index = await RedisHelper.HGetAsync<int>("AverageSite", entity.PositionTo);
                            if (index >= endArr.Length - 1)
                            {
                                await RedisHelper.HSetAsync("AverageSite", entity.PositionTo, 0);
                            }
                            else
                            {
                                await RedisHelper.HSetAsync("AverageSite", entity.PositionTo, index + 1);
                            }
                            entity.PositionTo = endArr[index];
                        }

                        //最后一条出库子任务--结构件库需要通过托盘属性TrayAttr改变终点工位（11-单空托赋111111,22-多空托赋222222,33-实托赋333333）                         
                        var trayItem = await _zjnBaseTrayRepository.GetFirstAsync(x => x.TrayNo == input.trayNo);

                        switch (trayItem.TrayAttr)
                        {
                            case 11:
                                item.TaskDetailsEnd = "111111";
                                break;
                            case 22:
                                item.TaskDetailsEnd = "222222";
                                break;
                            case 33:
                                item.TaskDetailsEnd = "333333";
                                break;
                            default:
                                break;
                        }
                        if (string.IsNullOrEmpty(item.TaskDetailsEnd))//写的路径或者已动态计算忽略，优先级低
                        {
                            item.TaskDetailsEnd = entity.PositionTo;
                        }
                    }

                    item.ResultValue = entity.PositionTo;

                }
                if (input.operationDirection == "Move")
                {
                    if (item.NodeCode == startNode)
                    {
                        item.TaskDetailsStart = entity.PositionFrom;
                        item.TaskDetailsEnd = entity.PositionFrom;
                    }


                }

                item.Id = YitIdHelper.NextId().ToString();
                item.TaskDetailsId = _cacheManager.GettaskId().ToString();
                item.TaskId = entity.TaskNo;
                if (item.WorkPathId == null) item.WorkPathId = "";
                if (item.TaskDetailsStart == null) item.TaskDetailsStart = "";
                if (item.TaskDetailsEnd == null) item.TaskDetailsEnd = "";
                if (item.TaskDetailsStates == null) item.TaskDetailsStates = 0;
                //item.CreateUser = _userManager.UserId;
                item.CreateUser = "les";
                item.CreateTime = DateTime.Now;
                item.TrayNo = entity.TrayNo;
                item.TaskDetailsStates = 1;
                item.ProductLevel = input.productLevel;
            }
            try
            {
                _db.BeginTran();

                var isOk = await _zjnTaskListRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();

                var Ok = await _zjnTaskListDetailsRepository.AsInsertable(list).ExecuteCommandAsync();

                if (input.operationDirection == "Out")
                {
                    await _db.Updateable<ZjnWcsWorkDeviceEntity>().SetColumns(x => x.JobGroup == input.operationDirection)
                            .Where(w => w.DeviceId == entity.PositionTo).ExecuteCommandAsync();//作用为控制出入不重叠，是否增加字段
                }
                if (input.operationDirection == "Into")
                {
                    await _db.Updateable<ZjnWcsWorkDeviceEntity>().SetColumns(x => x.JobGroup == input.operationDirection)
                            .Where(w => w.DeviceId == entity.PositionFrom).ExecuteCommandAsync();//作用为控制出入不重叠，是否增加字段
                }

                ZjnWmsTaskDetailsInfoOutput FirstTaskDetails = await GetFirstTaskDetails(list);

                //获取出库任务的第一条任务，修改行列层
                if (FirstTaskDetails != null)
                {
                    await _zjnTaskListDetailsRepository.UpdateAsync(x => new ZjnWmsTaskDetailsEntity() { RowEnd = 3, CellEnd = 101, LayerEnd = 1 }
                            , x => x.TaskDetailsId == FirstTaskDetails.taskDetailsId);

                }
                string CreateUser = input.taskFrom == "WCS" ? "WCS" : _userManager.UserId;

                if (input.operationDirection == "Into")//托盘物料绑定
                {
                    if (input.operationType == "production")//2022-11-14 update by yml 只有实托才绑定物料
                    {
                        await _db.Insertable(new ZjnWmsTrayGoodsEntity()
                        {
                            Id = YitIdHelper.NextId().ToString(),
                            TrayNo = input.trayNo,
                            GoodsCode = input.materialCode,
                            CreateTime = DateTime.Now,
                            CreateUser = CreateUser,
                            IsDeleted = 0,
                            Quantity = Convert.ToInt32(input.quantity),
                            GoodsId = "0",
                            Unit = "",
                            EnabledMark = 1,
                        }).ExecuteCommandAsync();
                        await _db.Insertable(new ZjnWmsTrayGoodsLogEntity()
                        {
                            Id = YitIdHelper.NextId().ToString(),
                            TrayNo = input.trayNo,
                            GoodsCode = input.materialCode,
                            CreateTime = DateTime.Now,
                            CreateUser = CreateUser,
                            Quantity = Convert.ToInt32(input.quantity),
                            GoodsId = "0",
                            Unit = "",
                            EnabledMark = 1
                        }).ExecuteCommandAsync();


                        //await _zjnBaseTrayRepository.UpdateAsync(b => new ZjnWmsTrayEntity() { TrayStates = 2 }
                        //        , a => a.TrayNo == input.trayNo);

                        //结构件库使用托盘属性区分--单个空托11，多个空托22，实物托盘33
                        await _zjnBaseTrayRepository.UpdateAsync(b => new ZjnWmsTrayEntity() { TrayStates = 2, TrayAttr = 33 }
                                , a => a.TrayNo == input.trayNo);

                    }
                    else
                    {
                        //结构件库使用托盘属性区分--单个空托11，多个空托22，实物托盘33
                        if (input.quantity > 1)
                        {
                            await _zjnBaseTrayRepository.UpdateAsync(b => new ZjnWmsTrayEntity() { TrayStates = 2, TrayAttr = 22 }
                                , a => a.TrayNo == input.trayNo);
                        }
                        else
                        {
                            await _zjnBaseTrayRepository.UpdateAsync(b => new ZjnWmsTrayEntity() { TrayStates = 2, TrayAttr = 11 }
                                , a => a.TrayNo == input.trayNo);
                        }
                    }

                }
                RedisHelper.HSet(CommonConst.CACHE_KEY_TASK_LIST, FirstTaskDetails.taskDetailsId, FirstTaskDetails);
                _db.CommitTran();
            }
            catch (Exception e)
            {
                string es = e.Message;
                logInfo.isBug = 1;
                logInfo.message = es;

                _db.RollbackTran();
                throw HSZException.Oh(e.Message);

            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }

        }

        /// <summary>
        /// 组盘入线体存储新建任务
        /// </summary>
        /// <param name="taskCrInput"></param>
        /// <returns></returns>
        public async Task CreateInLine([FromBody] ZjnWmsTaskCrInput taskCrInput)
        {
            if (string.IsNullOrWhiteSpace(taskCrInput.materialCode))
            {
                throw HSZException.Oh("物料编码不能空");
            }
            if (string.IsNullOrWhiteSpace(taskCrInput.positionFrom))
            {
                throw HSZException.Oh("起点编号不能空");
            }
            if (taskCrInput.taskList == null || taskCrInput.taskList.Count == 0)
            {
                throw HSZException.Oh("业务子任务不存在");
            }
            //防止重复生成任务
            var ifhaveTask = await _zjnTaskListRepository.AsQueryable()
                .Where(l => l.TrayNo == taskCrInput.taskList[0].TrayNo && l.TaskState < 3)
                .ToListAsync();
            if (ifhaveTask != null && ifhaveTask.Count > 0)
            {
                throw HSZException.Oh("此托盘任务已存在，不可重复生成");
            }
            var ifInLocation = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWmsLocationEntity>()
                            .Where(l => l.TrayNo == taskCrInput.taskList[0].TrayNo && l.LocationStatus != "0").FirstAsync();
            if (ifInLocation != null)
            {
                throw HSZException.Oh("此托盘已存在库里，不可生成入库任务");
            }
            //判断组盘信息是否已经在存储线体里面
            var existsTray = await _zjnWmsLineSettinglogEntity.AsSugarClient().Queryable<ZjnWmsLineSettinglogEntity>()
                            .Where(l => l.TrayNo == taskCrInput.taskList[0].TrayNo && l.Status == 1).FirstAsync();
            if (existsTray != null)
            {
                throw HSZException.Oh("此托盘已存在线体里，不可生成入线体任务");
            }
            var ifTrayInOnline = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWmsTrayEntity>()
                .Where(l => l.TrayNo == taskCrInput.taskList[0].TrayNo && l.IsDelete == 1).ToListAsync();
            if (ifTrayInOnline != null && ifhaveTask.Count != 0)
            {
                throw HSZException.Oh("此托盘已删除，不可生成入线体任务");
            }

            //获取物料信息
            ZjnWmsGoodsEntity goods = await _zjnSugarRepository.GetFirstAsync(g => g.GoodsCode == taskCrInput.materialCode && g.IsDelete == 0);
            if (goods == null)
            {
                throw HSZException.Oh("未找到该物料数据");
            }
            //通过物料类型找对应存储线体
            var lineInfo = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWmsLineSettingEntity>()
                .Where(l => l.GoodsType == goods.GoodsType).FirstAsync();
            if (lineInfo == null)
            {
                throw HSZException.Oh("该物料的存储线体不存在");
            }
            if (lineInfo.LineNowWork >= lineInfo.LineMaxWork)
            {
                throw HSZException.Oh("该存储线体已经满");
            }
            //获取操作名称
            var yDataRepository = (await _dictionaryDataRepository.GetFirstAsync(p => p.DictionaryTypeId == "349315174420710661" && p.EnCode == taskCrInput.operationDirection));

            //线体存储业务单独获取业务唯一ID
            var PathId = await _WmsWorkPathAffirm.GetWorkPathIdByLine(taskCrInput.positionFrom, Convert.ToInt32(taskCrInput.operationDirection));
            //根据业务Id获取到子任务
            ZjnWcsProcessConfigJsonOutput data = await _zjnServicePathConfigService.PathData(PathId);
            ZjnWmsTaskCrInput wmsTaskCrInput = data.zjnTaskInfoOutput.Adapt<ZjnWmsTaskCrInput>();
            wmsTaskCrInput.taskName = taskCrInput.taskList[0].TrayNo + yDataRepository.FullName;
            wmsTaskCrInput.orderNo = data.zjnTaskInfoOutput.orderNo;
            wmsTaskCrInput.materialCode = taskCrInput.materialCode;
            wmsTaskCrInput.quantity = taskCrInput.quantity;
            //wmsTaskCrInput.billNo = taskWarehouseRequest.attribute1;//待确认
            wmsTaskCrInput.priority = 1;
            wmsTaskCrInput.trayNo = taskCrInput.taskList[0].TrayNo;
            wmsTaskCrInput.positionFrom = taskCrInput.positionFrom;
            //wmsTaskCrInput.operationDirection = "Into";
            wmsTaskCrInput.taskFrom = "LES";
            wmsTaskCrInput.taskList = data.ZjnTaskListDetailsList.Adapt<List<ZjnWmsTaskDetailsEntity>>();


            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "线体存储新建主任务";
            logInfo.methodName = "Create";
            logInfo.methodParmes = JsonConvert.SerializeObject(taskCrInput);
            logInfo.taskNo = taskCrInput.taskNo;
            logInfo.trayNo = wmsTaskCrInput.trayNo;
            logInfo.isBug = 0;
            //新建任务
            var entity = wmsTaskCrInput.Adapt<ZjnWmsTaskEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.TaskNo = _cacheManager.GettaskId().ToString();
            entity.TaskState = 1;
            entity.Priority = wmsTaskCrInput.priority ?? 1;
            entity.PositionFrom = wmsTaskCrInput.positionFrom;
            entity.PositionTo = lineInfo.LineStart;//任务的终点工位是线体的起点工位
            entity.CreateUser = entity.TaskFrom;
            entity.CreateTime = DateTime.Now;

            entity.TaskName = wmsTaskCrInput.trayNo + yDataRepository.FullName;
            entity.OrderNo = data.zjnTaskInfoOutput.orderNo;
            entity.MaterialCode = wmsTaskCrInput.materialCode;
            entity.Quantity = wmsTaskCrInput.quantity;
            //wmsTaskCrInput.billNo = taskWarehouseRequest.attribute1;//待确认
            entity.Priority = 1;
            entity.TrayNo = wmsTaskCrInput.trayNo;
            entity.TaskFrom = "LES";

            List<ZjnWmsTaskDetailsEntity> list = wmsTaskCrInput.taskList;
            if (list != null)
            {
                foreach (var item in list)
                {
                    item.TaskDetailsStart = entity.PositionFrom;
                    item.TaskDetailsEnd = entity.PositionTo;
                    //结束点位
                    var endSite = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWcsWorkSiteEntity>().Where(x => x.StationId == item.TaskDetailsEnd).FirstAsync();
                    if (endSite != null)
                    {
                        item.RowEnd = endSite.Row;
                        item.CellEnd = endSite.Cell;
                        item.LayerEnd = endSite.Layer;
                    }

                    item.ResultValue = entity.PositionFrom;
                    item.Id = YitIdHelper.NextId().ToString();
                    item.TaskDetailsId = _cacheManager.GettaskId().ToString();
                    item.TaskId = entity.TaskNo;
                    if (item.WorkPathId == null) item.WorkPathId = "";
                    if (item.TaskDetailsStart == null) item.TaskDetailsStart = "";
                    if (item.TaskDetailsEnd == null) item.TaskDetailsEnd = "";
                    if (item.TaskDetailsStates == null) item.TaskDetailsStates = 0;
                    //item.CreateUser = _userManager.UserId;
                    item.CreateUser = "les";
                    item.CreateTime = DateTime.Now;
                    item.TrayNo = entity.TrayNo;
                    item.TaskDetailsStates = 1;
                    item.ProductLevel = taskCrInput.productLevel;
                }
            }
            try
            {
                _db.BeginTran();
                var isOk = await _zjnTaskListRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
                var Ok = await _zjnTaskListDetailsRepository.AsInsertable(list).ExecuteCommandAsync();

                //作用为控制出入不重叠，是否增加字段
                //await _db.Updateable<ZjnWcsWorkDeviceEntity>().SetColumns(x => x.JobGroup == input.operationDirection)
                //        .Where(w => w.DeviceId == entity.PositionFrom).ExecuteCommandAsync();

                ZjnWmsTaskDetailsInfoOutput FirstTaskDetails = await GetFirstTaskDetails(list);
                string CreateUser = taskCrInput.taskFrom == "WCS" ? "WCS" : _userManager.UserId;
                if (wmsTaskCrInput.operationType == "production")//2022-11-14 update by yml 只有实托才绑定物料
                {
                    await _db.Insertable(new ZjnWmsTrayGoodsEntity()
                    {
                        Id = YitIdHelper.NextId().ToString(),
                        TrayNo = wmsTaskCrInput.trayNo,
                        GoodsCode = taskCrInput.materialCode,
                        CreateTime = DateTime.Now,
                        CreateUser = CreateUser,
                        IsDeleted = 0,
                        Quantity = Convert.ToInt32(taskCrInput.quantity),
                        GoodsId = "0",
                        Unit = "",
                        EnabledMark = 1,
                    }).ExecuteCommandAsync();
                    await _db.Insertable(new ZjnWmsTrayGoodsLogEntity()
                    {
                        Id = YitIdHelper.NextId().ToString(),
                        TrayNo = wmsTaskCrInput.trayNo,
                        GoodsCode = taskCrInput.materialCode,
                        CreateTime = DateTime.Now,
                        CreateUser = CreateUser,
                        Quantity = Convert.ToInt32(taskCrInput.quantity),
                        GoodsId = "0",
                        Unit = "",
                        EnabledMark = 1
                    }).ExecuteCommandAsync();
                    await _zjnBaseTrayRepository.UpdateAsync(b => new ZjnWmsTrayEntity() { TrayStates = 2 }, a => a.TrayNo == wmsTaskCrInput.trayNo);
                }

                RedisHelper.HSet(CommonConst.CACHE_KEY_TASK_LIST, FirstTaskDetails.taskDetailsId, FirstTaskDetails);

                //入线体物料压入Redis队列,避免出入同条线体时更新当前存储数量字段死锁(左边存储，右边取)
                await _cacheManager.LineStoragePush<ZjnWmsLineSettingEntity>(CommonConst.cache_queue_linelist + lineInfo.LineNo, lineInfo);

                _db.CommitTran();
            }
            catch (Exception e)
            {
                string es = e.Message;
                logInfo.isBug = 1;
                logInfo.message = es;

                _db.RollbackTran();
                throw HSZException.Oh(e.Message);

            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }

        }
        /// <summary>
        /// 投产叫料出线体存储新建任务
        /// </summary>
        /// <param name="taskCrInput"></param>
        /// <returns></returns>
        public async Task CreateOutLine([FromBody] ZjnWmsTaskCrInput taskCrInput)
        {
            if (string.IsNullOrWhiteSpace(taskCrInput.materialCode))
            {
                throw HSZException.Oh("物料编码不能空");
            }
            if (string.IsNullOrWhiteSpace(taskCrInput.positionFrom))
            {
                throw HSZException.Oh("终点编号不能空");
            }
            if (taskCrInput.taskList == null || taskCrInput.taskList.Count == 0)
            {
                throw HSZException.Oh("业务子任务不存在");
            }
            var taskitem = taskCrInput.taskList[0];
            //获取物料信息
            ZjnWmsGoodsEntity goods = await _zjnSugarRepository.GetFirstAsync(g => g.GoodsCode == taskCrInput.materialCode && g.IsDelete == 0);
            if (goods == null)
            {
                throw HSZException.Oh("未找到该物料数据");
            }
            //按先进先出排序获取线体存储内的托盘物料(后期看看是否需要改用缓存队列来处理)
            var lineLogInfo = await _zjnWmsLineSettinglogEntity.AsSugarClient().Queryable<ZjnWmsLineSettinglogEntity>()
                .Where(l => l.GoodsType == goods.GoodsType && l.Status == 1).OrderBy(l => l.CreateTime, OrderByType.Asc).FirstAsync();
            if (lineLogInfo == null)
            {
                throw HSZException.Oh("存储线体不存在该物料组盘");
            }
            //查找存储线体
            var lineInfo = await _zjnWmsLineSettingEntity.AsSugarClient().Queryable<ZjnWmsLineSettingEntity>()
                .Where(l => l.GoodsType == goods.GoodsType && l.LineNo == lineLogInfo.LineNo).FirstAsync();
            if (lineInfo == null)
            {
                throw HSZException.Oh("该物料的存储线体不存在");
            }
            if (lineInfo.LineNowWork <= 0)
            {
                throw HSZException.Oh("存储线体上没有该物料");
            }
            //获取操作名称
            var yDataRepository = (await _dictionaryDataRepository.GetFirstAsync(p => p.DictionaryTypeId == "349315174420710661" && p.EnCode == taskCrInput.operationDirection));

            //线体存储业务单独获取业务唯一ID
            var pathId = await _WmsWorkPathAffirm.GetWorkPathIdByLine(taskCrInput.positionFrom, Convert.ToInt32(taskCrInput.operationDirection));
            //根据业务Id获取到子任务
            ZjnWcsProcessConfigJsonOutput data = await _zjnServicePathConfigService.PathData(pathId);
            ZjnWmsTaskCrInput wmsTaskCrInput = data.zjnTaskInfoOutput.Adapt<ZjnWmsTaskCrInput>();
            wmsTaskCrInput.taskName = taskitem.TrayNo + yDataRepository.FullName;
            wmsTaskCrInput.orderNo = data.zjnTaskInfoOutput.orderNo;
            wmsTaskCrInput.materialCode = taskCrInput.materialCode;
            wmsTaskCrInput.quantity = taskCrInput.quantity;
            //wmsTaskCrInput.billNo = taskWarehouseRequest.attribute1;//待确认
            wmsTaskCrInput.priority = 1;
            wmsTaskCrInput.trayNo = taskitem.TrayNo;
            wmsTaskCrInput.positionFrom = taskCrInput.positionFrom;
            //wmsTaskCrInput.operationDirection = "Into";
            wmsTaskCrInput.taskFrom = "LES";
            wmsTaskCrInput.taskList = data.ZjnTaskListDetailsList.Adapt<List<ZjnWmsTaskDetailsEntity>>();

            //新建任务
            var entity = wmsTaskCrInput.Adapt<ZjnWmsTaskEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.TaskNo = _cacheManager.GettaskId().ToString();
            entity.TaskState = 1;
            entity.Priority = wmsTaskCrInput.priority ?? 1;
            entity.PositionFrom = lineLogInfo.LineEnd;//线体出存任务的开始工位是线体的终点工位
            entity.PositionTo = wmsTaskCrInput.positionFrom;
            entity.CreateUser = entity.TaskFrom;
            entity.CreateTime = DateTime.Now;

            entity.TaskName = wmsTaskCrInput.trayNo + yDataRepository.FullName;
            entity.OrderNo = data.zjnTaskInfoOutput.orderNo;
            entity.MaterialCode = wmsTaskCrInput.materialCode;
            entity.Quantity = wmsTaskCrInput.quantity;
            //wmsTaskCrInput.billNo = taskWarehouseRequest.attribute1;//待确认
            entity.Priority = 1;
            entity.TrayNo = wmsTaskCrInput.trayNo;
            entity.TaskFrom = "LES";
            List<ZjnWmsTaskDetailsEntity> list = wmsTaskCrInput.taskList;

            string startNode = list.Where(x => x.NodeType == "start").FirstOrDefault().NodeCode;
            var endNodes = list.Where(x => string.IsNullOrEmpty(x.NodeNext));
            foreach (var item in list)
            {
                item.TaskDetailsStart = entity.PositionFrom;
                item.TaskDetailsEnd = entity.PositionTo;
                //起点工位
                var stratSite = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWcsWorkSiteEntity>().Where(x => x.StationId == item.TaskDetailsStart).FirstAsync();
                if (stratSite != null)
                {
                    item.RowEnd = stratSite.Row;
                    item.CellEnd = stratSite.Cell;
                    item.LayerEnd = stratSite.Layer;
                }
                item.ResultValue = entity.PositionFrom;
                item.Id = YitIdHelper.NextId().ToString();
                item.TaskDetailsId = _cacheManager.GettaskId().ToString();
                item.TaskId = entity.TaskNo;
                if (item.WorkPathId == null) item.WorkPathId = "";
                if (item.TaskDetailsStart == null) item.TaskDetailsStart = "";
                if (item.TaskDetailsEnd == null) item.TaskDetailsEnd = "";
                if (item.TaskDetailsStates == null) item.TaskDetailsStates = 0;
                //item.CreateUser = _userManager.UserId;
                item.CreateUser = "les";
                item.CreateTime = DateTime.Now;
                item.TrayNo = entity.TrayNo;
                item.TaskDetailsStates = 1;
                item.ProductLevel = taskCrInput.productLevel;
            }
            try
            {
                _db.BeginTran();
                var isOk = await _zjnTaskListRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
                var Ok = await _zjnTaskListDetailsRepository.AsInsertable(list).ExecuteCommandAsync();

                //作用为控制出入不重叠，是否增加字段
                await _db.Updateable<ZjnWcsWorkDeviceEntity>().SetColumns(x => x.JobGroup == taskCrInput.operationDirection)
                        .Where(w => w.DeviceId == entity.PositionTo).ExecuteCommandAsync();

                ZjnWmsTaskDetailsInfoOutput FirstTaskDetails = await GetFirstTaskDetails(list);
                string CreateUser = taskCrInput.taskFrom == "WCS" ? "WCS" : _userManager.UserId;

                RedisHelper.HSet(CommonConst.CACHE_KEY_TASK_LIST, FirstTaskDetails.taskDetailsId, FirstTaskDetails);
                //出线体组盘压入Redis队列,避免出入同条线体时更新当前存储数量字段死锁(左边存储，右边取)
                await _cacheManager.LineStoragePush<ZjnWmsLineSettingEntity>(CommonConst.cache_queue_linelist + lineLogInfo.LineNo, lineInfo);

                _db.CommitTran();
            }
            catch (Exception e)
            {
                string es = e.Message;
                _db.RollbackTran();
                throw HSZException.Oh(e.Message);

            }

        }

        /// <summary>
        /// 获取第一条任务
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<ZjnWmsTaskDetailsInfoOutput> GetFirstTaskDetails(List<ZjnWmsTaskDetailsEntity> list)
        {
            var start = list.Where(x => x.NodeType == "start").FirstOrDefault();
            //var first = list.Where(x => x.NodeCode == start.NodeNext).FirstOrDefault();
            //var output = await getzjnwmstaskdetailsinfooutput(first.taskdetailsid);
            //if (output.tasktype == 7)//入库台任务
            //{
            //    output = await _services.getrequiredservice<ifindlocationprocess>().wmsallotlocationtask(output);
            //}
            var output = await GetNextTaskDetails(start.TaskDetailsId);
            return output;
        }

        /// <summary>
        /// 获取下一个节点
        /// </summary>
        /// <param name="taskDetailsId">子任务id</param>
        /// <returns></returns>
        public async Task<ZjnWmsTaskDetailsInfoOutput> GetNextTaskDetails(string taskDetailsId)
        {
            ZjnWmsTaskDetailsInfoOutput res = new ZjnWmsTaskDetailsInfoOutput();
            ZjnWmsTaskDetailsInfoOutput resNext = new ZjnWmsTaskDetailsInfoOutput();
            var list = await GetZjnWmsTaskDetailsInfoOutputList(taskDetailsId);
            var nowTask = list.Where(x => x.taskDetailsId == taskDetailsId).FirstOrDefault();
            if (nowTask == null) throw HSZException.Oh("当前任务不存在！");
            if (string.IsNullOrEmpty(nowTask.nodeNext)) return null;
            if (!nowTask.nodeNext.Contains(","))
            {
                res = list.Where(x => x.nodeCode == nowTask.nodeNext).FirstOrDefault();
            }
            else
            {
                var ResultValue = nowTask.resultValue;
                if (ResultValue == null) throw HSZException.Oh("当前分支节点没有返回值！");
                var nextCode = GetConditionValue(list, nowTask.resultValue, nowTask.nodeNext);
                res = list.Where(x => x.nodeCode == nextCode).FirstOrDefault();
            }

            if (nowTask.nodeType == "dynamic" && res.taskType != 5)//上个任务为动态任务的，有路径也直接改起点，RGV点位和堆垛机点位冲突
            {
                res.taskDetailsStart = nowTask.taskDetailsEnd;
                res.rowStart = nowTask.rowEnd;
                res.cellStart = nowTask.cellEnd;
                res.layerStart = nowTask.layerEnd;
            }

            //Rgv任务，勿动
            if (res.taskType == 3)
            {
                resNext = list.Where(l => l.nodeCode == res.nodeNext).FirstOrDefault();

                if (resNext != null)
                {
                    resNext.taskDetailsStart = res.taskDetailsEnd;
                    var entityNext = resNext.Adapt<ZjnWmsTaskDetailsEntity>();
                    var result = await _zjnTaskListDetailsRepository.AsUpdateable(entityNext).ExecuteCommandAsync();
                    if (result == 0)
                    {
                        throw HSZException.Oh("更让新Rgv出库台到目标路径错误");
                    }
                }
            }

            //绑定接驳台，动态任务起点为上个任务终点
            if (res.nodeType == "dynamic")
            {
                if (res.taskType != 4 && res.taskType != 5)
                {
                    res.taskDetailsStart = nowTask.taskDetailsEnd;
                    res.rowStart = nowTask.rowEnd;
                    res.cellStart = nowTask.cellEnd;
                    res.layerStart = nowTask.layerEnd;
                }

                if (res.taskType == 2)//称重任务，意义是过渡
                {
                    res.taskDetailsStart = nowTask.taskDetailsStart;
                    res.taskDetailsEnd = nowTask.taskDetailsStart;
                    res.resultValue = res.taskDetailsStart;
                }
                if (res.taskType == 1)//平面调度任务，意义是过渡
                {
                    res.taskDetailsStart = nowTask.taskDetailsEnd;
                    res.taskDetailsEnd = nowTask.taskDetailsEnd;
                    res.resultValue = res.taskDetailsEnd;
                }

                //else if (res.taskType == 5 && nowTask.taskType != 7)//堆垛机任务
                //{
                //    if (string.IsNullOrEmpty(res.taskDetailsMove))//未分配货位，直接测试堆垛机会进
                //    {
                //        res = await _services.GetRequiredService<IFindLocationProcess>().TestStackerAllot(res);
                //    }
                //}

                if (res.taskType == 4)//堆垛机出库任务
                {
                    //货位id找巷道
                    var location = await _zjnWmsLocationRepository.GetFirstAsync(x => x.LocationNo == nowTask.taskDetailsStart);
                    if (location == null) throw HSZException.Oh("查找货位异常");
                    res.taskDetailsStart = location.LocationNo;
                    res.rowStart = location.Row;
                    res.cellStart = location.Cell;
                    res.layerStart = location.Layer;
                    //巷道找出库台,楼层不同，出库台不同，先用个快点的办法，先根据第二位
                    var endDevices = await _zjnWmsEquipmentListRepository
                        .GetListAsync(x => x.EquipmentSerialNumber == location.AisleNo && x.Type == "1");
                    var endDevice = endDevices.Find(w => w.TheBinding[1] == nowTask.taskDetailsEnd[1]);
                    //var endDevice = endDevices.Find(w => res.nodePropertyJson.Contains(w.TheBinding));
                    if (endDevice == null) throw HSZException.Oh("未找到绑定出库台，请先配置");
                    res.taskDetailsEnd = endDevice.TheBinding;
                    res.resultValue = endDevice.TheBinding;
                    //巷道找堆垛机
                    var aisleInfo = await _zjnWmsAisleEntity.GetFirstAsync(x => x.AisleNo == location.AisleNo);
                    if (aisleInfo == null) throw HSZException.Oh("未配置巷道数据");
                    res.taskDetailsMove = aisleInfo.StackerNo;
                    //获取出库台 行列层              
                    var outSite = await _zjnWcsWorkSiteRepository.GetFirstAsync(x => x.StationId == endDevice.TheBinding);
                    if (outSite == null) throw HSZException.Oh("未配置出库口站点");
                    res.rowEnd = outSite.Row;
                    res.cellEnd = outSite.Cell;
                    res.layerEnd = outSite.Layer;
                }
                if (res.taskType == 3)//RGV任务
                {
                    if (res.taskProcessType == TaskProcessType.Out || res.taskProcessType == TaskProcessType.EmptyTrayOut)
                    {
                        //原材料二楼出库是堆垛机出库后直接开始RGV任务，没有出库台任务
                        var startSite = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWcsWorkSiteEntity>().Where(x => x.StationId == res.taskDetailsStart).FirstAsync();
                        if (startSite != null)
                        {
                            res.rowStart = startSite.Row2;
                            res.cellStart = startSite.Cell2;
                            res.layerStart = startSite.Layer2;
                        }
                        var moveDevice = await _zjnWmsEquipmentListRepository.GetFirstAsync(x => x.EquipmentSerialNumber == startSite.DeviceId);
                        if (moveDevice != null)
                        {
                            res.taskDetailsMove = moveDevice.TheBinding;
                        }
                    }
                    if (res.taskProcessType == TaskProcessType.EmptyTrayInto || res.taskProcessType == TaskProcessType.Into)
                    {
                        res.taskDetailsStart = nowTask.taskDetailsEnd;
                        //相反二楼RGV任务直接到了入库台，没办法在入库台任务找货位
                        var startSite = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWcsWorkSiteEntity>().Where(x => x.StationId == res.taskDetailsStart).FirstAsync();
                        if (startSite != null)
                        {
                            res.rowStart = startSite.Row;
                            res.cellStart = startSite.Cell;
                            res.layerStart = startSite.Layer;
                        }
                        res = await _services.GetRequiredService<IFindLocationProcess>().WmsAllotLocationTask(res);
                        var endSite = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWcsWorkSiteEntity>().Where(x => x.StationId == res.taskDetailsEnd).FirstAsync();
                        if (endSite != null)
                        {
                            res.rowEnd = endSite.Row2;
                            res.cellEnd = endSite.Cell2;
                            res.layerEnd = endSite.Layer2;
                        }
                        var moveDevice = await _zjnWmsEquipmentListRepository.GetFirstAsync(x => x.TheBinding == res.taskDetailsEnd && x.EquipmentSerialNumber.StartsWith("RGV"));
                        if (moveDevice != null)
                        {
                            res.taskDetailsMove = moveDevice.EquipmentSerialNumber;
                        }
                    }


                }
                if (res.taskType == 6)//堆垛机移库任务
                {
                    res.taskDetailsStart = nowTask.taskDetailsStart;
                    res.taskDetailsEnd = nowTask.taskDetailsEnd;
                    var location = await _zjnWmsLocationRepository.GetFirstAsync(x => x.LocationNo == res.taskDetailsStart);
                    if (location == null) throw HSZException.Oh("查找货位异常");
                    var aisleInfo = await _zjnWmsAisleEntity.GetFirstAsync(x => x.AisleNo == location.AisleNo);
                    if (aisleInfo == null) throw HSZException.Oh("未配置巷道数据");
                    res.taskDetailsMove = aisleInfo.StackerNo;
                    res.rowStart = location.Row;
                    res.cellStart = location.Cell;
                    res.layerStart = location.Layer;
                    var endLocation = await _zjnWmsLocationRepository.GetFirstAsync(x => x.LocationNo == res.taskDetailsEnd);
                    if (endLocation != null)
                    {
                        res.rowEnd = endLocation.Row;
                        res.cellEnd = endLocation.Cell;
                        res.layerEnd = endLocation.Layer;
                    }
                }
                if (string.IsNullOrEmpty(res.taskDetailsEnd))
                {
                    string endSite = "";//终点

                    if (!string.IsNullOrEmpty(res.nodePropertyJson))
                    {
                        DynamicProperties properties;
                        try
                        {
                            properties = res.nodePropertyJson.Deserialize<DynamicProperties>();
                        }
                        catch (Exception)
                        {
                            throw HSZException.Oh("nodePropertyJson转化DynamicProperties不成功！");
                        }
                        if (properties.bindEndSite != null && properties.bindEndSite.Count() > 0)
                        {
                            if (properties.bindEndSite.Count() == 1)
                            {
                                endSite = properties.bindEndSite[0];
                            }
                            else
                            {
                                string[] endArr = properties.bindEndSite;
                                string end = string.Join(',', endArr);
                                var index = await RedisHelper.HGetAsync<int>("AverageSite", end);
                                if (index >= endArr.Length - 1)
                                {
                                    await RedisHelper.HSetAsync("AverageSite", end, 0);
                                }
                                else
                                {
                                    await RedisHelper.HSetAsync("AverageSite", end, index + 1);
                                }
                                endSite = endArr[index];
                                //endSite = await FindDeviceIdByTask(endArr);
                            }
                            //设备好，任务的起点和终点后，修改任务数据
                        }
                        //        }

                        if (!string.IsNullOrEmpty(endSite))
                        {
                            var workSiteInfo = await _zjnWcsWorkSiteRepository.GetFirstAsync(w => w.DeviceId == endSite);
                            if (workSiteInfo == null)
                            {
                                throw HSZException.Oh("未找到下个节点的站点数据");
                            }
                            res.taskDetailsEnd = endSite;
                            res.rowEnd = workSiteInfo.Row;
                            res.cellEnd = workSiteInfo.Cell;
                            res.layerEnd = workSiteInfo.Layer;
                        }
                    }
                    res.resultValue = res.taskDetailsEnd;
                }
            }
            else if (res.nodeType == "approver")
            {

                if (res.taskType == 5 && nowTask.taskType == 8)//堆垛机任务
                {
                    res = await _services.GetRequiredService<IFindLocationProcess>().TestStackerAllot(res);

                }
            }

            if (res.taskType == 7)//入库台任务
            {
                if (nowTask.nodeType == "start")
                {
                    res.taskDetailsStart = nowTask.taskDetailsStart;
                }
                res = await _services.GetRequiredService<IFindLocationProcess>().WmsAllotLocationTask(res);
            }

            if (res.taskType == 10)//堆垛机消防任务
            {
                if (nowTask.nodeType == "start")
                {
                    res.taskDetailsStart = nowTask.taskDetailsStart;
                }
                res = await _services.GetRequiredService<IFindLocationProcess>().FindFireControlLocation(res);
            }

            //最后一个任务  找出库口,将主任务的终点写入
            //if (string.IsNullOrEmpty(res.nodeNext)) {
            //    if (!string.IsNullOrEmpty(res.positionTo))
            //    {
            //        if (!res.positionTo.Contains(","))
            //        {
            //            res.taskDetailsEnd = res.positionTo;
            //        }
            //        else
            //        {
            //            //绑定多个出库口
            //            var siteArr = res.positionTo.Split(",");
            //            res.taskDetailsEnd = await FindDeviceIdByTask(siteArr);
            //        }
            //    }
            //}

            var entity = res.Adapt<ZjnWmsTaskDetailsEntity>();
            var isOk = await _zjnTaskListDetailsRepository.AsUpdateable(entity).ExecuteCommandAsync();
            if (isOk <= 0)
            {
                throw HSZException.Oh("修改任务失败");
            }
            //await RedisHelper.HDelAsync(CommonConst.CACHE_KEY_TASK_LIST, taskDetailsId);
            //await RedisHelper.HSetAsync(CommonConst.CACHE_KEY_TASK_LIST, res.taskDetailsId, res);
            return res;
        }

        /// <summary>
        /// 支持多次使用条件
        /// </summary>
        /// <param name="list"></param>
        /// <param name="ResultValue"></param>
        /// <param name="nodeNext"></param>
        /// <returns></returns>
        private string GetConditionValue(List<ZjnWmsTaskDetailsInfoOutput> list, string ResultValue, string nodeNext)
        {
            if (string.IsNullOrEmpty(nodeNext))
            {
                throw HSZException.Oh("条件节点后面必须接流程，请检查流程配置");
            }
            var childrnCode = nodeNext.Split(",");
            if (childrnCode.Length == 1)
            {
                return nodeNext;
            }
            ZjnWmsTaskDetailsInfoOutput nextNode = null;
            var conditionList = list.Where(x => childrnCode.Contains(x.nodeCode)).ToList();
            foreach (var item in conditionList)
            {
                var json = item.nodePropertyJson.Deserialize<ConditionProperties>();
                var flag = false;
                //目前只考虑一种条件
                foreach (var conditionsItem in json.conditions)
                {
                    //conditionsItem.field
                    var symbol = conditionsItem.symbol.Equals("==") ? "=" : conditionsItem.symbol;
                    switch (symbol)
                    {
                        case ">=":
                            flag = Convert.ToInt32(ResultValue) >= Convert.ToInt32(conditionsItem.filedValue);
                            break;
                        case ">":
                            flag = Convert.ToInt32(ResultValue) > Convert.ToInt32(conditionsItem.filedValue);
                            break;
                        case "=":
                            flag = ResultValue.Equals(conditionsItem.filedValue);
                            break;
                        case "<=":
                            flag = Convert.ToInt32(ResultValue) <= Convert.ToInt32(conditionsItem.filedValue);
                            break;
                        case "<":
                            flag = Convert.ToInt32(ResultValue) < Convert.ToInt32(conditionsItem.filedValue);
                            break;
                        case "<>":
                            flag = !ResultValue.Equals(conditionsItem.filedValue);
                            break;
                        case "like":
                            flag = ResultValue.Contains(conditionsItem.filedValue);
                            break;
                        case "notLike":
                            flag = !ResultValue.Contains(conditionsItem.filedValue);
                            break;
                        default:
                            break;
                    }
                }
                if (flag)
                {
                    nextNode = item;
                    break;
                }
            }
            return GetConditionValue(list, nextNode.resultValue, nextNode.nodeNext);
        }

        /// <summary>
        /// 通过任务均分设备终点
        /// </summary>
        /// <param name="siteArr"></param>
        /// <returns></returns>
        public async Task<string> FindDeviceIdByTask(string[] siteArr)
        {
            //获取缓存任务
            var taskHs = await RedisHelper.HGetAllAsync<ZjnWmsTaskDetailsInfoOutput>(CommonConst.CACHE_KEY_TASK_LIST);
            var tasklist = new List<ZjnWmsTaskDetailsInfoOutput>(taskHs.Values);
            //分组设备任务
            var taskGroup = tasklist.Where(x => x.taskDetailsStates == 2 && siteArr.Contains(x.taskDetailsEnd)).GroupBy(x => x.taskDetailsEnd);
            //返回的设备id
            var deviceId = "";
            //最小数量
            int min = -1;
            //判断哪个设备任务数量最少
            foreach (var item in taskGroup)
            {
                if (min == -1)
                {
                    deviceId = item.Key;
                    item.Count();
                }
                if (item.Count() < min)
                {
                    deviceId = item.Key;
                    min = item.Count();
                }
            }
            return deviceId;
        }

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="taskDetails">子任务</param>
        /// <param name="taskState">状态</param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<ZjnWmsTaskDetailsInfoOutput> FinishTask(ZjnWmsTaskDetailsEntity taskDetails, int taskState, TaskResultPubilcParameter parameter = null)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "完成任务";
            logInfo.methodName = "FinishTask";
            logInfo.methodParmes = taskState + "-" + JsonConvert.SerializeObject(taskDetails);
            logInfo.trayNo = taskDetails.TrayNo;
            logInfo.isBug = 0;

            try
            {
                ZjnWmsTaskDetailsInfoOutput res;
                try
                {
                    res = await GetNextTaskDetails(taskDetails.TaskDetailsId);
                }
                catch (Exception ex)
                {
                    logInfo.isBug = 1;
                    logInfo.message = "子任务查找异常";

                    throw HSZException.Oh("子任务查找异常1");
                }

                if (res == null)
                {
                    //2022-11-14 update by yml 出库最后一个任务，托盘解绑物料
                    var taskDto = await GetZjnWmsTaskDetailsInfoOutput(taskDetails.TaskDetailsId);
                    try
                    {
                        var task = await _zjnTaskListRepository.GetFirstAsync(f => f.TaskNo == taskDetails.TaskId);
                        var positionTo = task.PositionTo.Split(',');
                        _db.BeginTran();
                        if ((int)taskDto.taskProcessType == 1 || (int)taskDto.taskProcessType == 6)
                        {
                            if ((int)taskDto.taskProcessType == 1)
                            {
                                //物料解绑
                                await _zjnWmsTrayGoodsEntity.AsUpdateable().SetColumns(x => x.IsDeleted == 1).Where(x => x.TrayNo == taskDetails.TrayNo).ExecuteCommandAsync();
                            }
                            //托盘状态变为空闲
                            await _zjnBaseTrayRepository.AsUpdateable().SetColumns(x => x.TrayStates == 1).SetColumns(x => x.TrayAttr == 44).Where(x => x.TrayNo == taskDetails.TrayNo).ExecuteCommandAsync();
                        }
                        //处理组盘入线体存储任务
                        if (taskDto.taskProcessType == TaskProcessType.LineInto || taskDto.taskProcessType == TaskProcessType.LineOut)
                        {
                            await LineFinish(task, taskDto);
                        }
                        //修改任务为完成状态
                        await _zjnTaskListRepository.AsUpdateable().SetColumns(x => x.TaskState == taskState).Where(x => x.TaskNo == taskDetails.TaskId).ExecuteCommandAsync();
                        await _zjnTaskListDetailsRepository.AsUpdateable().SetColumns(x => x.TaskDetailsStates == taskState).Where(x => x.TaskDetailsId == taskDetails.TaskDetailsId).ExecuteCommandAsync();

                        _db.CommitTran();
                    }
                    catch (Exception ex)
                    {
                        _db.RollbackTran();

                        logInfo.isBug = 1;
                        logInfo.message = ex.Message;
                        throw HSZException.Oh("子任务查找异常2");
                    }
                    //删除缓存
                    await RedisHelper.HDelAsync(CommonConst.CACHE_KEY_TASK_LIST, taskDetails.TaskDetailsId);
                }
                else
                {
                    if (res.taskType == 3 && res.nodeType != "dynamic")//RGV调度
                    {
                        string targetSite = "";//目标位置B01043
                        if (taskDetails.TaskDetailsEnd == "I01079")//B01013
                        {
                            switch (parameter.targetType)
                            {
                                case 1:
                                    targetSite = "I01109";//B01043
                                    break;
                                case 2:
                                    targetSite = "I01109";//B01051
                                    break;
                                case 3:
                                    targetSite = "I01109";//B01043
                                    break;

                                default:
                                    break;
                            }
                        }

                        try
                        {
                            _db.BeginTran();
                            await ResetTask(taskDetails.TaskDetailsId, taskState);
                            await RedisHelper.HDelAsync(CommonConst.CACHE_KEY_TASK_LIST, taskDetails.TaskDetailsId);

                            //寻找Rgv坐标
                            var nextDevice = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWmsTaskDetailsEntity>().Where(x => x.TaskDetailsId == res.taskDetailsId).FirstAsync();
                            var startsiteentity = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWcsWorkSiteEntity>().Where(x => x.StationId == nextDevice.TaskDetailsStart).FirstAsync();
                            if (startsiteentity != null)
                            {
                                if ((int)res.taskProcessType == 6)
                                    targetSite = nextDevice.TaskDetailsEnd;
                                var siteentity = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWcsWorkSiteEntity>().Where(x => x.StationId == targetSite).FirstAsync();
                                if (siteentity != null)
                                {
                                    if ((int)res.taskProcessType != 6)
                                        res.taskDetailsEnd = targetSite;
                                    res.rowStart = startsiteentity.Row;
                                    res.cellStart = startsiteentity.Cell;
                                    res.layerStart = startsiteentity.Layer;
                                    res.rowEnd = siteentity.Row;
                                    res.cellEnd = siteentity.Cell;
                                    res.layerEnd = siteentity.Layer;
                                }

                            }
                            await RedisHelper.HSetAsync(CommonConst.CACHE_KEY_TASK_LIST, res.taskDetailsId, res);
                            if (!string.IsNullOrEmpty(targetSite))
                            {
                                await UpadteTaskSite(res.taskDetailsId, targetSite, (int)res.taskProcessType);
                            }
                            else
                            {
                                await UpadteTaskSite(res.taskDetailsId, res.taskDetailsEnd, (int)res.taskProcessType);
                            }


                            _db.CommitTran();
                        }
                        catch (Exception)
                        {
                            _db.RollbackTran();

                            throw;
                        }
                        return res;
                    }

                    //结构件库修改出库台的行列层
                    var rgvSite = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWcsWorkSiteEntity>().Where(x => x.StationId == res.taskDetailsStart).FirstAsync();
                    if (rgvSite != null)
                    {
                        await _zjnTaskListDetailsRepository.AsUpdateable().SetColumns(x => x.CellStart == rgvSite.Cell).SetColumns(x => x.RowStart == rgvSite.Row)
                                .SetColumns(x => x.LayerStart == rgvSite.Layer)
                                .Where(x => x.TaskDetailsId == res.taskDetailsId).ExecuteCommandAsync();
                    }
                    //上报入库台任务，判断双叉堆垛机，货叉2  列-1
                    if (parameter != null && parameter.targetType == 2)
                    {
                        --res.cellStart;
                        await _zjnTaskListDetailsRepository.AsUpdateable().SetColumns(x => x.CellStart == res.cellStart).Where(x => x.TaskDetailsId == res.taskDetailsId).ExecuteCommandAsync();
                    }


                    await ResetTask(taskDetails.TaskDetailsId, taskState);
                    await RedisHelper.HDelAsync(CommonConst.CACHE_KEY_TASK_LIST, taskDetails.TaskDetailsId);
                    await RedisHelper.HSetAsync(CommonConst.CACHE_KEY_TASK_LIST, res.taskDetailsId, res);
                }
                return res;
            }
            catch (Exception ex)
            {
                logInfo.isBug = 1;
                logInfo.message = ex.Message;
                throw HSZException.Oh("子任务查找异常");
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }

        /// <summary>
        /// 线体存储任务完成处理业务
        /// </summary>
        /// <param name="mainTask"></param>
        /// <param name="taskDto"></param>
        /// <returns></returns>
        public async Task LineFinish(ZjnWmsTaskEntity mainTask, ZjnWmsTaskDetailsInfoOutput taskDto)
        {
            if (taskDto == null)
            {
                return;
            }
            ZjnWmsLineSettingEntity lineInfo = null;
            switch (taskDto.taskProcessType)
            {
                case TaskProcessType.LineInto:
                    if (string.IsNullOrWhiteSpace(taskDto.taskDetailsEnd))
                    {
                        throw HSZException.Oh("任务终点编号不能空");
                    }
                    //入存获取线体信息
                    lineInfo = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWmsLineSettingEntity>()
                        .Where(l => l.LineStart == taskDto.taskDetailsEnd).FirstAsync();
                    if (lineInfo == null)
                    {
                        throw HSZException.Oh("该起点的存储线体不存在");
                    }
                    var lineNowWork = lineInfo.LineNowWork;
                    //从线体存储List队列中获取排队的信息,避免出入同条线体时更新当前存储数量字段死锁(左边存储，右边取)
                    lineInfo = await _cacheManager.LineStorageRPop<ZjnWmsLineSettingEntity>(CommonConst.cache_queue_linelist + lineInfo.LineNo);
                    if (lineInfo == null)
                    {
                        throw HSZException.Oh("该存储线体队列不存在");
                    }
                    //获取物料信息
                    //ZjnWmsGoodsEntity goods = await _zjnSugarRepository.GetFirstAsync(g => g.GoodsType == lineInfo.GoodsType && g.IsDelete == 0);
                    //if (goods == null)
                    //{
                    //    throw HSZException.Oh("未找到该物料数据");
                    //}
                    //更新当前线体的当前存储数量
                    lineNowWork++;
                    if (lineNowWork > 12)
                    {
                        lineNowWork = 12;
                    }
                    //更新线体当前存储数量
                    await _zjnWmsLineSettingEntity.AsUpdateable().SetColumns(x => x.LineNowWork == lineNowWork).Where(w => w.Id == lineInfo.Id).ExecuteCommandAsync();

                    //plc任务完成写线体进出日志
                    ZjnWmsLineSettinglogEntity lineLog = new ZjnWmsLineSettinglogEntity();
                    lineLog.Id = YitIdHelper.NextId().ToString();
                    lineLog.LineNo = lineInfo.LineNo;
                    lineLog.LineName = lineInfo.LineName;
                    lineLog.TrayNo = taskDto.trayNo;
                    lineLog.GoodsType = lineInfo.GoodsType;
                    lineLog.GoodsCode = mainTask.MaterialCode;
                    lineLog.LineStart = lineInfo.LineStart;
                    lineLog.LineEnd = lineInfo.LineEnd;
                    lineLog.LineLayer = lineInfo.LineLayer;
                    lineLog.LineMaxWork = lineInfo.LineMaxWork;
                    lineLog.LineNowWork = lineNowWork;
                    lineLog.Description = lineInfo.Description;
                    lineLog.Expand = lineInfo.Expand;
                    lineLog.Status = 1;
                    lineLog.CreateUser = _userManager.UserId;
                    lineLog.CreateTime = DateTime.Now;
                    if ((int)taskDto.taskProcessType == 8)
                    {
                        lineLog.OutTime = DateTime.Now;
                    }
                    await _db.Insertable(lineLog).ExecuteCommandAsync();
                    break;
                case TaskProcessType.LineOut:
                    if (string.IsNullOrWhiteSpace(taskDto.taskDetailsStart))
                    {
                        throw HSZException.Oh("任务起点编号不能空");
                    }
                    //按先进先出排序获取线体存储内的托盘物料(后期看看是否需要改用缓存队列)
                    var lineLogInfo = await _zjnWmsLineSettinglogEntity.AsSugarClient().Queryable<ZjnWmsLineSettinglogEntity>()
                        .Where(l => l.GoodsCode == mainTask.MaterialCode && l.Status == 1).OrderBy(l => l.CreateTime, OrderByType.Asc).FirstAsync();
                    if (lineLogInfo == null)
                    {
                        throw HSZException.Oh("存储线体不存在该物料组盘");
                    }
                    //从线体存储List队列中获取排队的信息,避免出入同条线体时更新当前存储数量字段死锁(左边存储，右边取)
                    ZjnWmsLineSettingEntity lineQueue = await _cacheManager.LineStorageRPop<ZjnWmsLineSettingEntity>(CommonConst.cache_queue_linelist + lineLogInfo.LineNo);
                    if (lineQueue == null)
                    {
                        throw HSZException.Oh("该存储线体队列不存在");
                    }
                    //出存获取线体信息
                    lineInfo = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWmsLineSettingEntity>()
                        .Where(l => l.Id == lineQueue.Id).FirstAsync();
                    if (lineInfo == null)
                    {
                        throw HSZException.Oh("该存储线体不存在");
                    }
                    //更新当前线体的当前存储数量
                    lineInfo.LineNowWork--;
                    if (lineInfo.LineNowWork < 0)
                    {
                        lineInfo.LineNowWork = 0;
                    }
                    //更新线体当前存储数量
                    await _zjnWmsLineSettingEntity.AsUpdateable().SetColumns(x => x.LineNowWork == lineInfo.LineNowWork).Where(w => w.Id == lineQueue.Id).ExecuteCommandAsync();
                    //更新线体存储日志
                    //await _zjnWmsLineSettinglogEntity.AsUpdateable().SetColumns(x => x.LineNowWork == lineInfo.LineNowWork && x.Status == 2 && x.OutTime == DateTime.Now).Where(w => w.Id == lineLogInfo.Id).ExecuteCommandAsync();
                    lineLogInfo.LineNowWork = lineInfo.LineNowWork;
                    lineLogInfo.Status = 2;
                    lineLogInfo.OutTime = DateTime.Now;
                    var isOk = await _zjnWmsLineSettinglogEntity.AsUpdateable(lineLogInfo).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
                    //物料解绑
                    await _zjnWmsTrayGoodsEntity.AsUpdateable().SetColumns(x => x.IsDeleted == 1).Where(x => x.TrayNo == mainTask.TrayNo).ExecuteCommandAsync();

                    //托盘状态变为空闲
                    await _zjnBaseTrayRepository.AsUpdateable().SetColumns(x => x.TrayStates == 1).Where(x => x.TrayNo == mainTask.TrayNo).ExecuteCommandAsync();

                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// 获取子任务dto List
        /// </summary>
        /// <param name="taskDetailsId">子任务id</param>
        /// <returns></returns>
        public async Task<List<ZjnWmsTaskDetailsInfoOutput>> GetZjnWmsTaskDetailsInfoOutputList(string taskDetailsId)
        {

            //var taskDetail = await _zjnTaskListDetailsRepository.AsSugarClient().Queryable<ZjnWmsTaskDetailsEntity>().Where(x => x.TaskDetailsId == taskDetailsId).FirstAsync();

            var TaskId = (await _zjnTaskListDetailsRepository.GetFirstAsync(x => x.TaskDetailsId == taskDetailsId)).TaskId;

            List<ZjnWmsTaskDetailsInfoOutput> list = await _zjnTaskListDetailsRepository.AsSugarClient().Queryable<ZjnWmsTaskDetailsEntity, ZjnWmsTaskEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.TaskId == b.TaskNo))
              .Where(a => a.TaskId == TaskId)
              .Select((a, b) => new ZjnWmsTaskDetailsInfoOutput
              {
                  id = a.Id,
                  taskDetailsId = a.TaskDetailsId,
                  taskDetailsName = a.TaskDetailsName,
                  taskId = a.TaskId,
                  taskDetailsStart = a.TaskDetailsStart,
                  taskDetailsEnd = a.TaskDetailsEnd,
                  taskDetailsMove = a.TaskDetailsMove,
                  taskDetailsStates = a.TaskDetailsStates,
                  taskDetailsDescribe = a.TaskDetailsDescribe,
                  createUser = a.CreateUser,
                  createTime = a.CreateTime,
                  modifiedUser = a.ModifiedUser,
                  modifiedTime = a.ModifiedTime,
                  workPathId = a.WorkPathId,
                  trayNo = a.TrayNo,
                  rowStart = a.RowStart,
                  cellStart = a.CellStart,
                  layerStart = a.LayerStart,
                  rowEnd = a.RowEnd,
                  cellEnd = a.CellEnd,
                  layerEnd = a.LayerEnd,
                  nodeCode = a.NodeCode,
                  nodeUp = a.NodeUp,
                  nodeNext = a.NodeNext,
                  nodeType = a.NodeType,
                  nodePropertyJson = a.NodePropertyJson,
                  resultValue = a.ResultValue,
                  taskType = a.TaskType,
                  priority = b.Priority,
                  taskProcessType = (TaskProcessType)
                  SqlFunc.MappingColumn(default(int), $" (select F_work_type from [zjn_wcs_process_config] where F_Id= {b.TaskTypeNum} ) "),
                  positionTo = b.PositionTo,
                  productLevel = a.ProductLevel,
              })
              .ToListAsync();
            return list;

        }

        /// <summary>
        /// 获取子任务dto
        /// </summary>
        /// <param name="taskDetailsId">子任务id</param>
        /// <returns></returns>
        public async Task<ZjnWmsTaskDetailsInfoOutput> GetZjnWmsTaskDetailsInfoOutput(string taskDetailsId)
        {

            ZjnWmsTaskDetailsInfoOutput list = await _zjnTaskListDetailsRepository.AsSugarClient().Queryable<ZjnWmsTaskDetailsEntity, ZjnWmsTaskEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.TaskId == b.TaskNo))
              .Where(a => a.TaskDetailsId == taskDetailsId)
              .Select((a, b) => new ZjnWmsTaskDetailsInfoOutput
              {
                  id = a.Id,
                  taskDetailsId = a.TaskDetailsId,
                  taskDetailsName = a.TaskDetailsName,
                  taskId = a.TaskId,
                  taskDetailsStart = a.TaskDetailsStart,
                  taskDetailsEnd = a.TaskDetailsEnd,
                  taskDetailsMove = a.TaskDetailsMove,
                  taskDetailsStates = a.TaskDetailsStates,
                  taskDetailsDescribe = a.TaskDetailsDescribe,
                  createUser = a.CreateUser,
                  createTime = a.CreateTime,
                  modifiedUser = a.ModifiedUser,
                  modifiedTime = a.ModifiedTime,
                  workPathId = a.WorkPathId,
                  trayNo = a.TrayNo,
                  rowStart = a.RowStart,
                  cellStart = a.CellStart,
                  layerStart = a.LayerStart,
                  rowEnd = a.RowEnd,
                  cellEnd = a.CellEnd,
                  layerEnd = a.LayerEnd,
                  nodeCode = a.NodeCode,
                  nodeUp = a.NodeUp,
                  nodeNext = a.NodeNext,
                  nodeType = a.NodeType,
                  nodePropertyJson = a.NodePropertyJson,
                  resultValue = a.ResultValue,
                  taskType = a.TaskType,
                  priority = b.Priority,
                  taskProcessType = (TaskProcessType)
                  SqlFunc.MappingColumn(default(int), $" (select F_work_type from [zjn_wcs_process_config] where F_Id= {b.TaskTypeNum} ) "),
                  positionTo = b.PositionTo,
                  productLevel = a.ProductLevel,

              }).FirstAsync();
            return list;

        }

        ///<summary>
        /// 更新主任务 
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWmsTaskUpInput input)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "更新主任务";
            logInfo.methodName = "Update";
            logInfo.methodParmes = JsonConvert.SerializeObject(input);
            logInfo.taskNo = input.taskNo;
            logInfo.isBug = 0;
            logInfo.trayNo = input.trayNo;

            try
            {

                var entity = input.Adapt<ZjnWmsTaskEntity>();
                entity.LastModifyUserId = _userManager.UserId;
                entity.LastModifyTime = DateTime.Now;
                var isOk = await _zjnTaskListRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
                if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
            }
            catch (Exception ex)
            {
                logInfo.isBug = 1;
                logInfo.message = ex.Message;
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }

        /// <summary>
        /// 更新子任务状态 
        /// </summary>
        /// <param name="id">子任务id</param>
        /// <param name="states">任务状态</param>
        /// <returns></returns>
        [HttpPut("{id}/ResetTask/{states}")]
        public async Task ResetTask(string id, int states = 1)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "更新子任务状态";
            logInfo.methodName = "ResetTask";
            logInfo.methodParmes = id + "-" + states;
            logInfo.taskNo = "";
            logInfo.isBug = 0;
            try
            {
                var isOk = await _zjnTaskListRepository.AsSugarClient().Updateable<ZjnWmsTaskDetailsEntity>().SetColumns(x => x.TaskDetailsStates == states).Where(x => x.TaskDetailsId == id).ExecuteCommandAsync();
                if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
                _cacheManager.TaskReset(id, states);
            }
            catch (Exception ex)
            {
                logInfo.isBug = 1;
                logInfo.message = ex.Message;
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }

        /// <summary>
        /// 上报子任务 
        /// </summary>
        /// <param name="id">子任务id</param>

        [HttpPut("{id}/FinishTaskDetails")]
        public async Task FinishTaskDetails(string id)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "上报子任务";
            logInfo.methodName = "ResetTask";
            logInfo.methodParmes = id;
            logInfo.taskNo = "";
            logInfo.isBug = 0;
            try
            {
                _db.BeginTran();
                await WCSReceivesTheSubtaskCallback(id, 2);
                var res = await GetZjnWmsTaskDetailsInfoOutput(id);
                await TaskProcesByWcs(Convert.ToInt32(id), 3, new TaskResultPubilcParameter { deviceNo = res.taskDetailsEnd, targetType = 1 });
                _db.CommitTran();

            }
            catch (Exception ex)
            {
                _db.RollbackTran();
                throw HSZException.Oh("上报失败！");
                logInfo.isBug = 1;
                logInfo.message = ex.Message;
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }

        /// <summary>
        /// 更新子任务目的地
        /// </summary>
        /// <param name="id">子任务id</param>
        /// <param name="siteId">站点id</param>
        /// <param name="processType">流程类型</param>
        /// <returns></returns>
        [HttpPut("{id}/UpadteTaskSite/{siteId}")]
        public async Task UpadteTaskSite(string id, string siteId, int processType)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "更新子任务目的地";
            logInfo.methodName = "UpadteTaskSite";
            logInfo.methodParmes = id + "-" + siteId;
            logInfo.taskNo = "";
            logInfo.isBug = 0;
            try
            {
                //原材料用，修改请通知
                var nextDevice = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWmsTaskDetailsEntity>().Where(x => x.TaskDetailsId == id).FirstAsync();

                var startsiteentity = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWcsWorkSiteEntity>().Where(x => x.StationId == nextDevice.TaskDetailsStart).FirstAsync();

                var siteentity = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWcsWorkSiteEntity>().Where(x => x.StationId == siteId).FirstAsync();

                nextDevice.RowStart = startsiteentity.Row;
                nextDevice.CellStart = startsiteentity.Cell;
                nextDevice.LayerStart = startsiteentity.Layer;
                nextDevice.RowEnd = siteentity.Row;
                nextDevice.CellEnd = siteentity.Cell;
                nextDevice.LayerEnd = siteentity.Layer;

                //不是空托出库
                if (processType != 6)
                {
                    nextDevice.TaskDetailsEnd = siteId;
                }

                var isOk = await _zjnTaskListRepository.AsSugarClient().Updateable<ZjnWmsTaskDetailsEntity>(nextDevice).Where(x => x.TaskDetailsId == id).ExecuteCommandAsync();
                if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
                _cacheManager.TaskUpadteSite(id, siteId, processType, nextDevice);
            }
            catch (Exception ex)
            {
                logInfo.isBug = 1;
                logInfo.message = ex.Message;
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }

        /// <summary>
        /// 更新子任务
        /// </summary>
        /// <param name="input">子任务</param>
        /// <returns></returns>
        [HttpPut("UpadteTask")]
        public async Task UpadteTask(ZjnWmsTaskDetailsInfoOutput input)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "更新子任务";
            logInfo.methodName = "UpadteTaskSite";
            logInfo.methodParmes = input.ToJson();
            logInfo.taskNo = "";
            logInfo.isBug = 0;
            try
            {
                //var siteentity = await _zjnTaskListRepository.AsSugarClient().Queryable<ZjnWcsWorkSiteEntity>().Where(x => x.StationId == siteId).FirstAsync();
                var isOk = await _zjnTaskListRepository.AsSugarClient().Updateable<ZjnWmsTaskDetailsEntity>()
                    .SetColumns(x => new ZjnWmsTaskDetailsEntity()
                    {
                        TaskType = input.taskType,
                        TaskDetailsStart = input.taskDetailsStart,
                        RowStart = input.rowStart,
                        CellStart = input.cellStart,
                        LayerStart = input.layerStart,
                        TaskDetailsEnd = input.taskDetailsEnd,
                        RowEnd = input.rowEnd,
                        CellEnd = input.cellEnd,
                        LayerEnd = input.layerEnd,
                        TaskDetailsStates = input.taskDetailsStates,
                        TaskDetailsMove = input.taskDetailsMove,
                    }).Where(x => x.TaskDetailsId == input.taskDetailsId).ExecuteCommandAsync();
                if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
                //_cacheManager.TaskUpadteSite(id, siteId);
                var info = await GetZjnWmsTaskDetailsInfoOutput(input.taskDetailsId);
                await RedisHelper.HSetAsync(CommonConst.CACHE_KEY_TASK_LIST, info.taskDetailsId, info);

            }
            catch (Exception ex)
            {
                logInfo.isBug = 1;
                logInfo.message = ex.Message;
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="id">子任务id</param>
        /// <returns></returns>
        [HttpPut("{id}/CancelTask")]
        public async Task CancelTask(string id)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "取消任务";
            logInfo.methodName = "CancelTask";
            logInfo.methodParmes = id;
            logInfo.taskNo = "";
            logInfo.isBug = 0;

            int cancelState = 4;
            try
            {
                _db.BeginTran();
                var isOk = await _zjnTaskListRepository.AsSugarClient().Updateable<ZjnWmsTaskDetailsEntity>().SetColumns(x => x.TaskDetailsStates == cancelState).Where(x => x.TaskDetailsId == id).ExecuteCommandAsync();

                var isOk1 = await _zjnTaskListRepository.AsSugarClient().Updateable<ZjnWmsTaskEntity>().SetColumns(x => x.TaskState == 4).Where(x => x.TaskNo == SqlFunc.Subqueryable<ZjnWmsTaskDetailsEntity>().Where(s => s.TaskDetailsId == id).Select(s => s.TaskId)).ExecuteCommandAsync();
                _cacheManager.TaskReset(id, cancelState);

                _db.CommitTran();
            }
            catch (Exception ex)
            {
                _db.RollbackTran();

                logInfo.isBug = 1;
                logInfo.message = ex.Message;

                throw HSZException.Oh(ErrorCode.COM1001);
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }

        /// <summary>
        /// 删除主任务
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "删除主任务";
            logInfo.methodName = "Delete";
            logInfo.methodParmes = id;
            logInfo.taskNo = "";
            logInfo.isBug = 0;
            try
            {
                var entity = await _zjnTaskListRepository.GetFirstAsync(p => p.Id.Equals(id));
                _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
                var isOk = await _zjnTaskListRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
                if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
            }
            catch (Exception ex)
            {
                logInfo.isBug = 1;
                logInfo.message = ex.Message;
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }

        #region WCS完成任务上报给WMS函数

        /// <summary>
        ///  WCS完成任务上报函数 根据传入任务类型 再分发函数进行处理
        /// </summary>
        /// <param name="TaskNo">子任务号</param>
        /// <param name="TaskState">任务完成状态</param>
        /// <param name="parameter">公共参数</param>
        /// <returns></returns>
        [HttpPost("TaskProcesByWcs")]
        [AllowAnonymous]
        [NonUnify]
        public async Task<RESTfulResult<ZjnWmsTaskDetailsInfoOutput>> TaskProcesByWcs(int TaskNo, int TaskState, TaskResultPubilcParameter parameter)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "WCS完成任务上报函数 根据传入任务类型 再分发函数进行处理";
            logInfo.methodName = "TaskProcesByWcs";
            logInfo.methodParmes = TaskNo + "-" + TaskState + "-" + JsonConvert.SerializeObject(parameter);
            logInfo.taskNo = TaskNo.ToString();
            logInfo.deviceNo = parameter?.deviceNo;
            logInfo.isBug = 0;

            RESTfulResult<ZjnWmsTaskDetailsInfoOutput> rESTfulResult = new RESTfulResult<ZjnWmsTaskDetailsInfoOutput>();
            ZjnWmsTaskDetailsInfoOutput output = null;
            try
            {
                var WmsTaskData = (await _zjnTaskListDetailsRepository.GetFirstAsync(p => p.TaskDetailsId == TaskNo.ToString()));

                switch (Convert.ToInt32(WmsTaskData.TaskType))
                {
                    case (int)ChildrenTaskType.PlaneTask://平面设备调度任务
                        output = await _services.GetRequiredService<IPlaneDeviceProcess>().PlaneDeviceTask(WmsTaskData, TaskState, parameter);
                        break;
                    case (int)ChildrenTaskType.WeighTask://称重校验任务
                        output = await _services.GetRequiredService<IWeighDeviceProcess>().TaskDetailsStart(WmsTaskData, TaskState, parameter);
                        break;
                    case (int)ChildrenTaskType.RgvTask://Rgv调度任务
                        output = await _services.GetRequiredService<IRgvDeviceProcess>().RGVDetailStart(WmsTaskData, TaskState);
                        break;
                    case (int)ChildrenTaskType.StockerOutTask://堆垛机出库任务
                        output = await _services.GetRequiredService<IStackerOutProcess>().StackerOutOfStorageTask(WmsTaskData, TaskState, parameter);
                        break;
                    case (int)ChildrenTaskType.StockerInTask://堆垛机入库任务
                        await _services.GetRequiredService<IStackerInProcess>().singleScTask(WmsTaskData, TaskState, parameter);
                        break;
                    case (int)ChildrenTaskType.StockerMoveTask://堆垛机移库任务
                        output = await _services.GetRequiredService<IStackerMoveProcess>().StackerMoveTask(WmsTaskData, TaskState, parameter);
                        break;
                    case (int)ChildrenTaskType.DeskInTask://入库台任务
                        output = await _services.GetRequiredService<IWarehousInProcess>().WarehousingStationTask(WmsTaskData, TaskState, parameter);
                        break;
                    case (int)ChildrenTaskType.DeskOutTask://出库台任务
                        output = await _services.GetRequiredService<IWarehousOutProcess>().OutboundDeskTask(WmsTaskData, TaskState);
                        break;
                    case (int)ChildrenTaskType.HoistTask://提升机调度任务
                        output = await _services.GetRequiredService<IElevatorProcess>().HoistMoveStart(WmsTaskData, TaskState, parameter);
                        break;
                    case (int)ChildrenTaskType.FireControlTask://堆垛机消防任务
                        output = await _services.GetRequiredService<IFireControlProcess>().FireControlStart(WmsTaskData, TaskState, parameter);
                        break;
                    case (int)ChildrenTaskType.AgvTask://Agv调度任务
                        output = await _services.GetRequiredService<IAgvDeviceProcess>().AgvDetailStart(WmsTaskData, TaskState, parameter);
                        break;
                    default:
                        rESTfulResult.code = 500;
                        //日志找不到任务类型
                        return rESTfulResult;
                }
                rESTfulResult.code = 200;
                rESTfulResult.data = output;
                return rESTfulResult;
            }
            catch (Exception ex)
            {
                logInfo.isBug = 1;
                logInfo.message = ex.Message;

                rESTfulResult.code = 500;
                rESTfulResult.msg = ex.Message;
                return rESTfulResult;
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }


        /// <summary>
        /// WCS接收子任务回调函数
        /// </summary>
        /// <param name="TaskNo">子任务号</param>
        /// <param name="TaskState">状态</param>
        /// <returns></returns>
        [HttpGet("WCSReceivesTheSubtaskCallback")]
        [AllowAnonymous]
        [NonUnify]
        public async Task<RESTfulResult<bool>> WCSReceivesTheSubtaskCallback(string TaskNo, int TaskState)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "WCS完成任务上报函数 根据传入任务类型 再分发函数进行处理";
            logInfo.methodName = "WCSReceivesTheSubtaskCallback";
            logInfo.methodParmes = TaskNo + "-" + TaskState;

            logInfo.taskNo = TaskNo;
            logInfo.isBug = 0;

            RESTfulResult<bool> rESTfulResult = new RESTfulResult<bool>();
            try
            {
                var WmsTaskData = (await _zjnTaskListDetailsRepository.GetFirstAsync(p => p.TaskDetailsId == TaskNo.ToString()));
                if (WmsTaskData == null)
                {
                    rESTfulResult.code = 500;
                    rESTfulResult.msg = "确定认任务号是否正确";
                }
                else
                {
                    WmsTaskData.TaskDetailsStates = TaskState;
                    var isOk = await _zjnTaskListDetailsRepository.AsUpdateable(WmsTaskData).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
                    if (isOk > 0)
                    {
                        var data = await this.GetZjnWmsTaskDetailsInfoOutput(TaskNo);
                        await RedisHelper.HDelAsync(CommonConst.CACHE_KEY_TASK_LIST, TaskNo);
                        await RedisHelper.HSetAsync(CommonConst.CACHE_KEY_TASK_LIST, TaskNo, data);
                    }
                    else
                    {
                        rESTfulResult.code = 500;
                        rESTfulResult.msg = "修改状态失败!";
                    };

                }
                rESTfulResult.code = 200;
                rESTfulResult.data = true;
                return rESTfulResult;
            }
            catch (Exception ex)
            {
                logInfo.isBug = 1;
                logInfo.message = ex.Message;

                rESTfulResult.code = 500;
                rESTfulResult.data = false;
                rESTfulResult.msg = ex.Message;
                return rESTfulResult;
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }


        #endregion

        /// <summary>
        /// App插入缓存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("TaskCrInputsAsync")]
        public async Task<List<ZjnWmsTaskCrInput>> TaskCrInputsAsync([FromBody] ZjnWmsTaskCrInput input)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "App插入缓存";
            logInfo.methodName = "TaskCrInputsAsync";
            logInfo.methodParmes = JsonConvert.SerializeObject(input);
            logInfo.taskNo = input.taskNo;
            logInfo.trayNo = input.trayNo;
            logInfo.isBug = 0;
            try
            {
                if (string.IsNullOrEmpty(input.operationDirection))
                {
                    throw HSZException.Oh("不能为空");
                }
                if (input.operationDirection == "Into" && string.IsNullOrEmpty(input.positionFrom))
                {
                    throw HSZException.Oh("不能为空");
                }
                List<ZjnWmsTaskCrInput> zjnWmsTasks = new List<ZjnWmsTaskCrInput>();
                await RedisHelper.HSetAsync(input.createUser, input.orderNo, input);
                var lsit = await RedisHelper.HGetAllAsync(input.createUser);
                foreach (var item in lsit)
                {
                    var s = JsonConvert.DeserializeObject<ZjnWmsTaskCrInput>(item.Value);
                    zjnWmsTasks.Add(s);
                }
                return zjnWmsTasks;
            }
            catch (Exception ex)
            {
                logInfo.isBug = 1;
                logInfo.message = ex.Message;

                throw HSZException.Oh(ErrorCode.COM1000);
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }

        /// <summary>
        /// 查询缓存
        /// </summary>
        /// <param name="createUser"></param>
        /// <returns></returns>
        [HttpGet("GetListTaskCrInputsAsync")]
        public async Task<List<ZjnWmsTaskCrInput>> GetListTaskCrInputsAsync(string createUser)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "查询缓存";
            logInfo.methodName = "GetListTaskCrInputsAsync";
            logInfo.methodParmes = createUser;
            logInfo.isBug = 0;
            try
            {

                List<ZjnWmsTaskCrInput> zjnWmsTasks = new List<ZjnWmsTaskCrInput>();

                var lsit = await RedisHelper.HGetAllAsync(createUser);
                foreach (var item in lsit)
                {
                    // Into/入库 Out/出库 Move
                    var TaskCrInput = JsonConvert.DeserializeObject<ZjnWmsTaskCrInput>(item.Value);

                    //获取操作名称
                    var yDataRepository = (await _dictionaryDataRepository.GetFirstAsync(p => p.DictionaryTypeId == "349315174420710661" && p.EnCode == TaskCrInput.operationDirection));
                    TaskCrInput.operationDirection = yDataRepository.FullName;

                    //if (TaskCrInput.operationDirection == "2")
                    //{
                    //    TaskCrInput.operationDirection = "入库";
                    //}
                    //else if (TaskCrInput.operationDirection == "1")
                    //{
                    //    TaskCrInput.operationDirection = "出库";
                    //}
                    //else
                    //{

                    //    TaskCrInput.operationDirection = "空托入库";
                    //}
                    zjnWmsTasks.Add(TaskCrInput);
                }
                return zjnWmsTasks;
            }
            catch (Exception ex)
            {
                logInfo.isBug = 1;
                logInfo.message = ex.Message;

                throw HSZException.Oh(ex.Message).StatusCode(300);
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="createUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("RESTfulResult")]
        public async Task<List<ZjnWmsTaskCrInput>> RESTfulResult(string createUser, string id)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "删除缓存";
            logInfo.methodName = "RESTfulResult";
            logInfo.methodParmes = createUser + "-" + id;
            logInfo.isBug = 0;

            try
            {
                await RedisHelper.HDelAsync(createUser, id);
                List<ZjnWmsTaskCrInput> zjnWmsTasks = new List<ZjnWmsTaskCrInput>();
                var lsit = await RedisHelper.HGetAllAsync(createUser);
                foreach (var item in lsit)
                {
                    var s = JsonConvert.DeserializeObject<ZjnWmsTaskCrInput>(item.Value);
                    zjnWmsTasks.Add(s);
                }
                return zjnWmsTasks;

            }
            catch (Exception ex)
            {
                logInfo.isBug = 1;
                logInfo.message = ex.Message;

                throw HSZException.Oh(ErrorCode.COM1002);
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }
        /// <summary>
        ///APP出入库
        /// </summary>
        /// <param name="createUser"></param>
        /// <returns></returns>
        [HttpGet("DetermineTheOperatingApp")]
        public async Task DetermineTheOperatingApp(string createUser)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "APP出入库";
            logInfo.methodName = "DetermineTheOperatingApp";
            logInfo.methodParmes = createUser;
            logInfo.isBug = 0;
            try
            {
                var lsit = await RedisHelper.HGetAllAsync(createUser);
                foreach (var item in lsit)
                {
                    var TaskCrInput = JsonConvert.DeserializeObject<ZjnWmsTaskCrInput>(item.Value);
                    //数据字典＞WMS＞流程类型：1出库 2入库 3移库 4其他 5空托入库 6空托出库 7结构件库4F组盘入线体 8结构件库4F产线叫料出线体
                    if (TaskCrInput.operationDirection == "1" || TaskCrInput.operationDirection == "6")
                    {
                        //出库操作
                        //await AppOutbound(TaskCrInput);
                        //结构件库出库
                        //await AppOutFrameMember(TaskCrInput);
                        //出库找货位
                        var taskList = await _services.GetService<IFindLocationProcess>().FindOutBoundLocation(TaskCrInput);
                        foreach (var taskitem in taskList)
                        {
                            //获取业务唯一ID
                            var PathId = await _WmsWorkPathAffirm.GetWorkPathId(taskitem.trayNo, TaskCrInput.positionFrom,
                                taskitem.materialCode, TaskCrInput.operationDirection.ToInt32());
                            //根据业务Id获取到子任务
                            ZjnWcsProcessConfigJsonOutput data = await _zjnServicePathConfigService.PathData(PathId);
                            ZjnWmsTaskCrInput wmsTaskCrInput = data.zjnTaskInfoOutput.Adapt<ZjnWmsTaskCrInput>();
                            wmsTaskCrInput.taskName = taskitem.trayNo + "出库";
                            wmsTaskCrInput.orderNo = taskitem.orderNo;
                            wmsTaskCrInput.materialCode = taskitem.materialCode;
                            wmsTaskCrInput.quantity = taskitem.quantity;
                            //wmsTaskCrInput.billNo = taskWarehouseRequest.attribute1;//待确认
                            wmsTaskCrInput.priority = taskitem.priority;
                            wmsTaskCrInput.trayNo = taskitem.trayNo;
                            wmsTaskCrInput.positionFrom = taskitem.positionFrom;
                            wmsTaskCrInput.billNo = taskitem.billNo;
                            wmsTaskCrInput.batchNo = taskitem.batchNo;
                            if (!string.IsNullOrEmpty(TaskCrInput.positionTo))
                            {
                                wmsTaskCrInput.positionTo = TaskCrInput.positionTo;
                            }

                            wmsTaskCrInput.operationDirection = "Out";
                            wmsTaskCrInput.taskFrom = "LES";
                            wmsTaskCrInput.taskList = data.ZjnTaskListDetailsList.Adapt<List<ZjnWmsTaskDetailsEntity>>();
                            await Create(wmsTaskCrInput);
                        }
                    }
                    else if (TaskCrInput.operationDirection == "7")
                    {
                        //入线体存储新建任务
                        await CreateInLine(TaskCrInput);
                    }
                    else if (TaskCrInput.operationDirection == "8")
                    {
                        //产线叫料出线体存储新建任务
                        await CreateOutLine(TaskCrInput);
                    }
                    else
                    {
                        foreach (var taskitem in TaskCrInput.taskList)
                        {
                            //获取操作名称 select d.F_FullName from zjn_base_dictionary_type t,zjn_base_dictionary_data d where t.F_EnCode = 'liuchengleixing' and t.f_id = d.F_DictionaryTypeId and d.F_EnCode = 'TaskCrInput.operationDirection'
                            var yDataRepository = (await _dictionaryDataRepository.GetFirstAsync(p => p.DictionaryTypeId == "349315174420710661" && p.EnCode == TaskCrInput.operationDirection));

                            //获取业务唯一ID
                            var PathId = await _WmsWorkPathAffirm.GetWorkPathId(taskitem.TrayNo, TaskCrInput.positionFrom, TaskCrInput.materialCode, Convert.ToInt32(TaskCrInput.operationDirection));
                            //根据业务Id获取到子任务
                            ZjnWcsProcessConfigJsonOutput data = await _zjnServicePathConfigService.PathData(PathId);
                            ZjnWmsTaskCrInput wmsTaskCrInput = data.zjnTaskInfoOutput.Adapt<ZjnWmsTaskCrInput>();
                            wmsTaskCrInput.taskName = taskitem.TrayNo + yDataRepository.FullName;
                            wmsTaskCrInput.orderNo = TaskCrInput.orderNo;
                            wmsTaskCrInput.materialCode = TaskCrInput.materialCode;
                            wmsTaskCrInput.quantity = TaskCrInput.quantity;
                            //wmsTaskCrInput.billNo = taskWarehouseRequest.attribute1;//待确认
                            wmsTaskCrInput.priority = 1;
                            wmsTaskCrInput.trayNo = taskitem.TrayNo;
                            wmsTaskCrInput.positionFrom = TaskCrInput.positionFrom;
                            wmsTaskCrInput.billNo = TaskCrInput.billNo;
                            wmsTaskCrInput.batchNo = TaskCrInput.batchNo;
                            wmsTaskCrInput.operationDirection = "Into";
                            wmsTaskCrInput.taskFrom = "LES";
                            wmsTaskCrInput.taskList = data.ZjnTaskListDetailsList.Adapt<List<ZjnWmsTaskDetailsEntity>>();
                            await Create(wmsTaskCrInput);
                        }
                    }
                    await RedisHelper.HDelAsync(createUser, item.Key);
                }
            }
            catch (Exception ex)
            {

                throw HSZException.Oh(ex.Message);
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }
        /// <summary>
        /// App出库
        /// </summary>
        /// <param name="TaskCrInput">出库信息</param>
        /// <returns></returns>
        public async Task AppOutbound(ZjnWmsTaskCrInput TaskCrInput)
        {
            //托盘类型，大小盘
            int trayType = 0;

            try
            {
                if (TaskCrInput.taskList.Count > 0)
                {
                    if (!string.IsNullOrEmpty(TaskCrInput.taskList[0].TrayNo))//指定托盘出库可以这样
                    {
                        var traynoType = TaskCrInput.taskList[0].TrayNo.Substring(2, 1);
                        switch (traynoType)
                        {
                            case "A"://小盘
                                trayType = 1;
                                break;
                            case "B"://大盘
                                trayType = 2;
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (TaskCrInput.operationDirection == "6")
                {
                    #region 空托盘


                    var processEntity = await _zjnTaskListRepository.AsSugarClient()
                        .Queryable<ZjnWcsProcessConfigEntity>()
                        .Where(x => x.WorkType == (int)TaskProcessType.EmptyTrayOut && x.WorkEnd.Contains(TaskCrInput.positionFrom)).FirstAsync();
                    if (processEntity == null) throw HSZException.Oh("找不到业务流程");
                    var processId = processEntity.Id;
                    //空托盘货位
                    //var location = await _findLocationProcess.FindEmptyTrayLocation(TaskCrInput.TrayType.ToInt32(), TaskCrInput.positionFrom);
                    var locations = await _zjnWmsMaterialInventoryRepository.AsQueryable()
                        .InnerJoin<ZjnWmsLocationEntity>((a, b) => a.ProductsLocation == b.LocationNo && a.IsDeleted == 0
                        && b.IsDelete == 0 && b.EnabledMark == 1)
                        .InnerJoin<ZjnWmsAisleEntity>((a, b, c) => b.AisleNo == c.AisleNo && c.EnabledMark == 1)
                        .InnerJoin<ZjnWmsTrayEntity>((a, b, c, d) => a.ProductsContainer == d.TrayNo && d.EnabledMark == 1)
                        .Where((a, b, c, d) => b.LocationStatus == "1" && a.ProductsCode == string.Empty
                        && d.Type == trayType).OrderBy(a => a.CreateTime, OrderByType.Asc).OrderBy((a, b) => b.Depth)
                        .Select((a, b) => b).ToListAsync();
                    if (locations.Count == 0)
                    {
                        throw HSZException.Oh("未找到货位");
                    }

                    _db.BeginTran();

                    await _zjnWmsLocationAutoService.UpdateLocationStatus(locations[0].LocationNo, LocationStatus.Order);

                    //创建任务
                    ZjnWmsTaskCrInput taskInput = new ZjnWmsTaskCrInput
                    {
                        trayNo = locations[0].TrayNo,//托盘
                        positionFrom = locations[0].LocationNo,//起点货位
                        positionTo = TaskCrInput.positionFrom,//终点
                        operationDirection = "Out",//出库
                        taskFrom = "LES",
                        taskName = "空托出库",
                    };
                    //传入添加任务必须参数
                    var list = await CreateByConfigId(processId, taskInput);
                    _db.CommitTran();
                    #endregion
                    return;
                }
                #region
                //switch (TaskCrInput.operationDirection)
                //{
                //    case "6"://空托出库
                //        #region 空托盘


                //        var processEntity = await _zjnTaskListRepository.AsSugarClient()
                //            .Queryable<ZjnWcsProcessConfigEntity>()
                //            .Where(x => x.WorkType == (int)TaskProcessType.EmptyTrayOut && x.WorkEnd.Contains(TaskCrInput.positionFrom)).FirstAsync();
                //        if (processEntity == null) throw HSZException.Oh("找不到业务流程");
                //        var processId = processEntity.Id;
                //        //空托盘货位
                //        //var location = await _findLocationProcess.FindEmptyTrayLocation(TaskCrInput.TrayType.ToInt32(), TaskCrInput.positionFrom);
                //        var locations = await _zjnWmsMaterialInventoryRepository.AsQueryable()
                //            .InnerJoin<ZjnWmsLocationEntity>((a, b) => a.ProductsLocation == b.LocationNo && a.IsDeleted == 0
                //            && b.IsDelete == 0 && b.EnabledMark == 1)
                //            .InnerJoin<ZjnWmsAisleEntity>((a, b, c) => b.AisleNo == c.AisleNo && c.EnabledMark == 1)
                //            .InnerJoin<ZjnWmsTrayEntity>((a, b, c, d) => a.ProductsContainer == d.TrayNo && d.EnabledMark == 1)
                //            .Where((a, b, c, d) => b.LocationStatus == "1" && a.ProductsCode == string.Empty
                //            && d.Type == trayType).OrderBy(a => a.CreateTime, OrderByType.Asc).OrderBy((a, b) => b.Depth)
                //            .Select((a, b) => b).ToListAsync();
                //        if (locations.Count == 0)
                //        {
                //            throw HSZException.Oh("未找到货位");
                //        }

                //        _db.BeginTran();

                //        await _zjnWmsLocationAutoService.UpdateLocationStatus(locations[0].LocationNo, LocationStatus.Order);

                //        //创建任务
                //        ZjnWmsTaskCrInput taskInput = new ZjnWmsTaskCrInput
                //        {
                //            trayNo = locations[0].TrayNo,//托盘
                //            positionFrom = locations[0].LocationNo,//起点货位
                //            positionTo = TaskCrInput.positionFrom,//终点
                //            operationDirection = "Out",//出库
                //            taskFrom = "LES",
                //            taskName = "空托出库",
                //        };
                //        //传入添加任务必须参数
                //        var list = await CreateByConfigId(processId, taskInput);
                //        _db.CommitTran();
                //        #endregion
                //        return;
                //    default:
                //        break;
                //}
                #endregion

                string TheBinding = null;
                //根据物料号,查询出的库存信息
                var Inventory = await _zjnWmsMaterialInventoryRepository.AsQueryable()
                    .InnerJoin<ZjnWmsLocationEntity>((a, b) => a.ProductsLocation == b.LocationNo && a.IsDeleted == 0
                    && b.IsDelete == 0 && b.EnabledMark == 1)
                    .InnerJoin<ZjnWmsAisleEntity>((a, b, c) => b.AisleNo == c.AisleNo && c.EnabledMark == 1)
                    .InnerJoin<ZjnWmsTrayEntity>((a, b, c, d) => a.ProductsContainer == d.TrayNo && d.EnabledMark == 1)
                    .Where((a, b, c, d) => b.LocationStatus == "1"
                    && a.ProductsCode == TaskCrInput.materialCode)
                    .OrderBy(a => a.CreateTime, OrderByType.Asc)
                    .Select((a, b) => a).ToListAsync();
                if (Inventory.Count() == 0)
                {
                    var InventoryInput = await _zjnWmsMaterialInventoryRepository.GetListAsync(x => string.IsNullOrEmpty(x.ProductsCode.ToString()) && x.IsDeleted == 0 && x.ProductsLocation != null);
                    if (InventoryInput.Count == 0)
                    {
                        throw HSZException.Oh("空托出库失败!查询不到空托盘.");
                    }
                    //空托叫料须知道大小盘，目标位置
                    foreach (var item in InventoryInput)
                    {

                        ////查询空托盘的货位消息
                        //var Locationstory = (await _zjnWmsLocationRepository.GetFirstAsync(p => p.EnabledMark == 1 && p.LocationStatus == "0" && p.LocationNo == item.ProductsLocation));

                        ////查询到巷道
                        //var AisleEntity = await _zjnWmsAisleEntity.GetFirstAsync(x => x.AisleNo == Locationstory.AisleNo);
                        ////查询托盘信息
                        //var BaseTrays = await _zjnBaseTrayRepository.GetFirstAsync(x => x.TrayNo == item.ProductsContainer);



                        ////判断正负极
                        //if (AisleEntity.StackerNo.Contains("A"))
                        //{
                        //    switch (TaskCrInput.positionFrom)
                        //    {
                        //        case "B01079":
                        //            if (BaseTrays.Type == 2)
                        //            {
                        //                bool Details = await this.CreateTaskDetails(item.ProductsContainer, TaskCrInput.positionFrom, item.ProductsLocation, null, 6, 1, "Out");
                        //                if (Details)
                        //                {
                        //                    await _zjnWmsLocationAutoService.UpdateLocationStatus(item.ProductsLocation, LocationStatus.Order);
                        //                }
                        //            }
                        //            else
                        //            {
                        //                continue;
                        //            }
                        //            break;
                        //        case "B01031":
                        //            if (BaseTrays.Type != 2)
                        //            {
                        //                bool Details = await this.CreateTaskDetails(item.ProductsContainer, TheBinding, item.ProductsLocation, null, 6, 1, "Out");
                        //                if (Details)
                        //                {
                        //                    await _zjnWmsLocationAutoService.UpdateLocationStatus(item.ProductsLocation, LocationStatus.Order);
                        //                }
                        //            }
                        //            else
                        //            {
                        //                continue;
                        //            }
                        //            break;
                        //        default:
                        //            break;
                        //    }
                        //}
                        //else
                        //{
                        //    if (TaskCrInput.positionFrom == "B01029")
                        //    {
                        //        if (BaseTrays.TrayNo == "2")
                        //        {
                        //            bool Details = await this.CreateTaskDetails(item.ProductsContainer, TheBinding, item.ProductsLocation, null, 6, 1, "Out");
                        //            if (Details)
                        //            {
                        //                await _zjnWmsLocationAutoService.UpdateLocationStatus(item.ProductsLocation, LocationStatus.Order);
                        //            }

                        //        }
                        //        else
                        //        {
                        //            continue;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        if (BaseTrays.TrayNo != "2")
                        //        {
                        //            bool Details = await this.CreateTaskDetails(item.ProductsContainer, TheBinding, item.ProductsLocation, null, 6, 1, "Out");
                        //            if (Details)
                        //            {
                        //                await _zjnWmsLocationAutoService.UpdateLocationStatus(item.ProductsLocation, LocationStatus.Order);
                        //            }

                        //        }
                        //        else
                        //        {
                        //            continue;
                        //        }
                        //    }

                        //}
                        return;

                    }

                }
                else
                {
                    //按照时间排序
                    decimal TheTotalNumber = Convert.ToDecimal(TaskCrInput.quantity);
                    foreach (var item in Inventory)
                    {
                        //查询托盘信息
                        var BaseTrays = await _zjnBaseTrayRepository.GetFirstAsync(x => x.TrayNo == item.ProductsContainer);

                        //小托盘（1）
                        if (BaseTrays.Type == 1)
                        {
                            //查询货位信息，区分大小托盘（1:深工位）
                            var Locations = (await _zjnWmsLocationRepository.GetFirstAsync(p => p.EnabledMark == 1 && p.LocationNo == item.ProductsLocation));
                            if (Locations.Depth == 1)
                            {
                                var Loca = Convert.ToInt32(Locations.LocationNo.Substring(11, 3));
                                string Locastring = Loca.ToString("D3");
                                //获取到外工位的货位编号
                                var Locationss = Locations.LocationNo.Substring(0, 11).ToString() + Locastring + Locations.LocationNo.Substring(14, 4).ToString();
                                //查询外货位是否存在货物
                                var Locationlist = Inventory.Where(x => x.ProductsLocation == Locationss).ToList();
                                if (Locationlist.Count() > 0)
                                {
                                    //物料已经存在库存一个月优先出库，不然就先出外货位
                                    if (item.CreateTime <= DateTime.Now.AddDays(30))
                                    {
                                        //查询空货位
                                        var Locationstory = (await _zjnWmsLocationRepository.GetFirstAsync(p => p.EnabledMark == 1 && p.LocationStatus == "0"));
                                        //进行移库   
                                        //根据业务Id获取到子任务
                                        ZjnWcsProcessConfigJsonOutput data = await _zjnServicePathConfigService.PathData("355526898056430853");//业务IP写死
                                        ZjnWmsTaskCrInput wmsTaskCrInput = data.zjnTaskInfoOutput.Adapt<ZjnWmsTaskCrInput>();
                                        wmsTaskCrInput.taskName = Locationlist[0].ProductsContainer + "移库";
                                        wmsTaskCrInput.orderNo = data.zjnTaskInfoOutput.orderNo;
                                        wmsTaskCrInput.materialCode = Locationlist[0].ProductsCode;
                                        wmsTaskCrInput.quantity = Locationlist[0].ProductsQuantity;
                                        //wmsTaskCrInput.billNo = taskWarehouseRequest.attribute1;//待确认
                                        wmsTaskCrInput.priority = 1;
                                        wmsTaskCrInput.trayNo = Locationlist[0].ProductsContainer;
                                        wmsTaskCrInput.positionTo = Locationstory.LocationNo;
                                        wmsTaskCrInput.positionFrom = Locationlist[0].ProductsLocation;
                                        wmsTaskCrInput.operationDirection = "Out";
                                        wmsTaskCrInput.taskFrom = "App";
                                        wmsTaskCrInput.taskList = data.ZjnTaskListDetailsList.Adapt<List<ZjnWmsTaskDetailsEntity>>();
                                        await Create(wmsTaskCrInput);
                                        //出库深位物料
                                        bool Details = await this.CreateTaskDetails(item.ProductsContainer, TaskCrInput.positionFrom, item.ProductsLocation, TaskCrInput.materialCode, 1, item.ProductsQuantity, "1");
                                        if (Details)
                                        {
                                            TheTotalNumber = TheTotalNumber - item.ProductsQuantity;
                                            await _zjnWmsLocationAutoService.UpdateLocationStatus(item.ProductsLocation, LocationStatus.Order);
                                        }
                                    }
                                    else
                                    {
                                        continue;

                                    }
                                }
                                else
                                {

                                    bool Details = await this.CreateTaskDetails(item.ProductsContainer, TaskCrInput.positionFrom, item.ProductsLocation, TaskCrInput.materialCode, 1, TaskCrInput.quantity, "1");
                                    if (Details)
                                    {
                                        TheTotalNumber = TheTotalNumber - item.ProductsQuantity;
                                        await _zjnWmsLocationAutoService.UpdateLocationStatus(item.ProductsLocation, LocationStatus.Order);
                                    }


                                }

                            }
                            else
                            {
                                bool Details = await this.CreateTaskDetails(item.ProductsContainer, TaskCrInput.positionFrom, item.ProductsLocation, TaskCrInput.materialCode, 1, TaskCrInput.quantity, "1");
                                if (Details)
                                {
                                    TheTotalNumber = TheTotalNumber - item.ProductsQuantity;
                                    await _zjnWmsLocationAutoService.UpdateLocationStatus(item.ProductsLocation, LocationStatus.Order);
                                }

                            }

                        }
                        else
                        {
                            bool Details = await this.CreateTaskDetails(item.ProductsContainer, TaskCrInput.positionFrom, item.ProductsLocation, TaskCrInput.materialCode, 1, TaskCrInput.quantity, "1");

                            if (Details)
                            {
                                TheTotalNumber = TheTotalNumber - item.ProductsQuantity;
                                await _zjnWmsLocationAutoService.UpdateLocationStatus(item.ProductsLocation, LocationStatus.Order);
                            }
                        }

                        if (TheTotalNumber <= 0)
                        {
                            break;
                        }

                    }
                }

            }
            catch (Exception ex)
            {

                throw HSZException.Oh(ex.Message);
            }


        }
        /// <summary>
        /// 结构件库App出库
        /// </summary>
        /// <param name="TaskCrInput"></param>
        /// <returns></returns>
        public async Task AppOutFrameMember(ZjnWmsTaskCrInput TaskCrInput)
        {
            try
            {
                //空托出库
                if (TaskCrInput.operationDirection == "6")
                {
                    var processEntity = await _zjnTaskListRepository.AsSugarClient()
                            .Queryable<ZjnWcsProcessConfigEntity>()
                            .Where(x => x.WorkType == (int)TaskProcessType.EmptyTrayOut && x.WorkEnd.Contains(TaskCrInput.positionFrom)).FirstAsync();
                    if (processEntity == null) throw HSZException.Oh("找不到业务流程");
                    var processId = processEntity.Id;

                    //获取空托数据
                    var EmptryTrayList = await _zjnWmsMaterialInventoryRepository.AsQueryable()
                    .InnerJoin<ZjnWmsLocationEntity>((a, b) => a.ProductsLocation == b.LocationNo && a.IsDeleted == 0
                    && b.IsDelete == 0 && b.EnabledMark == 1 && a.ProductsCode == string.Empty)
                    .InnerJoin<ZjnWmsAisleEntity>((a, b, c) => b.AisleNo == c.AisleNo && c.EnabledMark == 1)
                    .InnerJoin<ZjnWmsTrayEntity>((a, b, c, d) => a.ProductsContainer == d.TrayNo && d.EnabledMark == 1)
                    .Where((a, b, c, d) => b.LocationStatus == "1")
                    .OrderBy(a => a.CreateTime, OrderByType.Asc)
                    .OrderBy((a, b) => b.Depth, OrderByType.Asc)
                    .Select((a, b, c, d) => new { AisleNo = b.AisleNo, TrayNo = a.ProductsContainer, Qty = a.ProductsQuantity, LocationNo = b.LocationNo, TrayType = d.Type, TrayAttr = d.TrayAttr })
                    .FirstAsync();

                    if (EmptryTrayList == null)
                    {
                        throw HSZException.Oh("空托出库失败!查询不到空托盘.");
                    }
                    else
                    {
                        _db.BeginTran();
                        await _zjnWmsLocationAutoService.UpdateLocationStatus(EmptryTrayList.LocationNo, LocationStatus.Order);

                        //创建任务
                        ZjnWmsTaskCrInput taskInput = new ZjnWmsTaskCrInput
                        {
                            trayNo = EmptryTrayList.TrayNo,//托盘
                            positionFrom = EmptryTrayList.LocationNo,//起点货位
                            positionTo = TaskCrInput.positionFrom,//终点
                            operationDirection = "Out",//出库
                            taskFrom = "LES",
                            taskName = "空托出库",
                            TrayType = EmptryTrayList.TrayType.ToString()//托盘类型
                        };
                        //传入添加任务必须参数
                        var list = await CreateByConfigId(processId, taskInput);
                        _db.CommitTran();
                    }
                }
                else
                {
                    //实托出库 
                    var MatterTrayList = await _zjnWmsMaterialInventoryRepository.AsQueryable()
                    .InnerJoin<ZjnWmsLocationEntity>((a, b) => a.ProductsLocation == b.LocationNo && a.IsDeleted == 0
                    && b.IsDelete == 0 && b.EnabledMark == 1)
                    .InnerJoin<ZjnWmsAisleEntity>((a, b, c) => b.AisleNo == c.AisleNo && c.EnabledMark == 1)
                    .InnerJoin<ZjnWmsTrayEntity>((a, b, c, d) => a.ProductsContainer == d.TrayNo && d.EnabledMark == 1)
                    .WhereIF(TaskCrInput.materialCode != null && TaskCrInput.materialCode != "", a => a.ProductsCode == TaskCrInput.materialCode)
                    .WhereIF(TaskCrInput.trayNo != null && TaskCrInput.trayNo != "", a => a.ProductsContainer == TaskCrInput.trayNo)
                    .Where((a, b, c, d) => b.LocationStatus == "1")
                    .OrderBy(a => a.CreateTime, OrderByType.Desc)
                    .OrderBy((a, b, c, d) => b.Depth, OrderByType.Asc)
                    .Select((a, b) => a).ToListAsync();

                    if (MatterTrayList.Count == 0)
                    {
                        throw HSZException.Oh("出库失败，查询不到该物料的数据。");
                    }
                    else
                    {
                        //出库逻辑--按托盘条码出库，物料条码+数量出库
                        if (TaskCrInput.trayNo != null || TaskCrInput.trayNo != "")//按托盘条码出库
                        {
                            var OutTray = await _zjnWmsMaterialInventoryRepository.GetFirstAsync(x => x.ProductsContainer == TaskCrInput.trayNo && x.IsDeleted == 0);
                            if (OutTray == null)
                            {
                                throw HSZException.Oh("出库失败，该托盘不在立库中。");
                            }
                            else
                            {
                                var Locations = await _zjnWmsLocationRepository.GetFirstAsync(p => p.EnabledMark == 1 && p.LocationNo == OutTray.ProductsLocation);
                                if (Locations.Depth == 1)
                                {
                                    var curRow = Locations.Row;
                                    int adjoinRow = 0;
                                    if (curRow == 1)
                                    {
                                        adjoinRow = curRow + 1;
                                    }
                                    if (curRow == 4)
                                    {
                                        adjoinRow = curRow - 1;
                                    }
                                    //同列深度为0的货位
                                    var adjoinLocations = await _zjnWmsLocationRepository.GetFirstAsync(p => p.EnabledMark == 1 && p.AisleNo == Locations.AisleNo && p.Row == adjoinRow
                                    && p.Cell == Locations.Cell && p.Layer == Locations.Layer);
                                    //是否有托盘，有托盘 先移库后出库
                                    if (adjoinLocations.TrayNo != null || adjoinLocations.TrayNo != "")
                                    {


                                    }
                                    else
                                    {

                                    }

                                }
                            }

                        }
                        else//按物料条码+数量出库
                        {

                        }


                    }

                }


            }
            catch (Exception ex)
            {
                throw HSZException.Oh(ex.Message);
            }

        }
        /// <summary>
        /// 调用创建任务接口
        /// </summary>
        /// <param name="TrayNo">托盘号</param>
        /// <param name="TheBinding">终点设备号</param>
        /// <param name="positionTo">起点设备号</param>
        /// <param name="materialCode">物料编号</param>
        /// <param name="workType">操作类型</param>
        /// <param name="quantity">数量</param>
        /// <param name="EnCode">操作名称编号</param>
        /// <returns></returns>
        public async Task<bool> CreateTaskDetails(string TrayNo, string TheBinding, string positionTo, string materialCode, int workType, decimal? quantity, string EnCode)
        {

            try
            {
                //获取操作名称
                var yDataRepository = (await _dictionaryDataRepository.GetFirstAsync(p => p.DictionaryTypeId == "349315174420710661" && p.EnCode == EnCode));
                //获取业务唯一ID
                var PathId = await _WmsWorkPathAffirm.GetWorkPathId(TrayNo, TheBinding, materialCode, workType);
                //根据业务Id获取到子任务
                ZjnWcsProcessConfigJsonOutput data = await _zjnServicePathConfigService.PathData(PathId);
                ZjnWmsTaskCrInput wmsTaskCrInput = data.zjnTaskInfoOutput.Adapt<ZjnWmsTaskCrInput>();
                wmsTaskCrInput.taskName = TrayNo + yDataRepository.FullName;
                wmsTaskCrInput.orderNo = data.zjnTaskInfoOutput.orderNo;
                wmsTaskCrInput.materialCode = materialCode;
                wmsTaskCrInput.quantity = quantity;
                //wmsTaskCrInput.billNo = taskWarehouseRequest.attribute1;//待确认
                wmsTaskCrInput.priority = Convert.ToInt32(quantity);
                wmsTaskCrInput.trayNo = TrayNo;
                wmsTaskCrInput.positionFrom = positionTo;
                if (!string.IsNullOrEmpty(TheBinding))
                {
                    wmsTaskCrInput.positionTo = TheBinding;
                }

                wmsTaskCrInput.operationDirection = "Out";
                wmsTaskCrInput.taskFrom = "App";
                wmsTaskCrInput.taskList = data.ZjnTaskListDetailsList.Adapt<List<ZjnWmsTaskDetailsEntity>>();
                await Create(wmsTaskCrInput);
                return true;
            }
            catch (Exception ex)
            {

                throw HSZException.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 根据盘号获取出库基础信息
        /// </summary>
        /// <param name="TrayNo">托盘号</param>
        /// <returns></returns>
        [HttpGet("GetLocation")]
        public async Task<dynamic> GetLocation(string TrayNo)
        {
            ZjnWmsRunLoginfoCrInput logInfo = new ZjnWmsRunLoginfoCrInput();
            logInfo.taskType = "根据盘号获取出库基础信息";
            logInfo.methodName = "GetLocation";
            logInfo.methodParmes = TrayNo;
            logInfo.trayNo = TrayNo;
            logInfo.isBug = 0;
            try
            {
                var data = await _zjnBaseTrayRepository.AsSugarClient().Queryable<ZjnWmsTrayEntity>()
                    .LeftJoin<ZjnWmsTrayLocationLogEntity>((a, b) => a.TrayNo == b.TrayNo)
                    .LeftJoin<ZjnWmsGoodsEntity>((a, b, c) => b.GoodsCode == c.GoodsCode)
                    .InnerJoin<ZjnWmsLocationEntity>((a, b, c, e) => b.LocationNo == e.LocationNo)
                    .Where((a, b, c, e) => a.TrayNo == TrayNo)
                    .Select((a, b, c, e) => new ZjnWmsTrayLocationLogListOutput
                    {
                        F_GoodsCodeNmae = c.GoodsName,
                        F_GoodsCode = b.GoodsCode,
                        F_Quantity = b.Quantity,
                        F_Unit = b.Unit,
                        F_TrayNo = a.TrayNo,
                        F_TrayName = a.TrayName,
                        F_LocationNo = b.LocationNo,
                        F_GoodsType = c.GoodsType,
                        F_LocationName = e.Description,
                        F_NumberOfRoadway = e.AisleNo,

                    }).ToPagedListAsync(1, 2);
                ZjnWmsTrayLocationLogListOutput zjnWmsTray = new ZjnWmsTrayLocationLogListOutput();
                foreach (var item in data.list)
                {
                    zjnWmsTray = item;
                }
                return zjnWmsTray;
            }
            catch (Exception ex)
            {

                throw HSZException.Oh(ex.Message);
            }
            finally
            {
                await _zjnWmsRunLoginfoService.Create(logInfo);
            }
        }


    }
}


