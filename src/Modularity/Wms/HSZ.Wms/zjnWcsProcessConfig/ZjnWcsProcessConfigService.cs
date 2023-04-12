using HSZ.ChangeDataBase;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.System.Entitys.Entity.System;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.System;
using HSZ.VisualDev.Entitys.Model.VisualDevModelData;
using HSZ.wms.Entitys.Dto.zjnWcsProcessConfig;
using HSZ.wms.Interfaces.ZjnServicePathConfig;
using HSZ.WorkFlow.Entitys;
using HSZ.WorkFlow.Entitys.Model;
using HSZ.WorkFlow.Entitys.Model.Properties;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using HSZ.wms.Entitys.Dto.ZjnWmsTask;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using Newtonsoft.Json;
using System.Runtime.Intrinsics.X86;
using Microsoft.OpenApi.Extensions;

namespace HSZ.wms.ZjnServicePathConfig
{
    /// <summary>
    /// 业务路径配置表服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnWcsProcessConfig", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWcsProcessConfigService : IZjnWcsProcessConfigService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsProcessConfigEntity> _zjnServicePathConfigRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private readonly IDataInterfaceService _dataInterfaceService;
        private readonly IChangeDataBase _changeDataBase;
        private readonly IDictionaryDataService _dictionaryDataService;
        private readonly ISqlSugarRepository<ZjnWcsWorkPathEntity> _zjnWcsWorkPathRepository;
        private readonly ICacheManager _cacheManager;


        /// <summary>
        /// 初始化一个<see cref="ZjnWcsProcessConfigService"/>类型的新实例
        /// </summary>
        public ZjnWcsProcessConfigService(ISqlSugarRepository<ZjnWcsProcessConfigEntity> zjnServicePathConfigRepository,
            IUserManager userManager, IDataInterfaceService dataInterfaceService, IChangeDataBase changeDataBase, IDictionaryDataService dictionaryDataService, ISqlSugarRepository<ZjnWcsWorkPathEntity> ZjnWcsWorkPathRepository, ICacheManager cacheManager)
        {
            _zjnServicePathConfigRepository = zjnServicePathConfigRepository;
            _zjnWcsWorkPathRepository = ZjnWcsWorkPathRepository;

            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _dataInterfaceService = dataInterfaceService;
            _changeDataBase = changeDataBase;
            _dictionaryDataService = dictionaryDataService;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// 获取业务路径配置表
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnServicePathConfigRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWcsProcessConfigInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取业务路径配置表列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWcsProcessConfigListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnServicePathConfigRepository.AsSugarClient().Queryable<ZjnWcsProcessConfigEntity, DictionaryDataEntity, ZjnWmsWarehouseEntity>((a, b, c) => new JoinQueryInfos(
                JoinType.Left, a.WorkType.ToString() == b.EnCode,
                JoinType.Left, a.WorkWarehouse == c.WarehouseNo))
                .Where((a, b) => b.DictionaryTypeId == "349315174420710661" && SqlFunc.IsNull(b.DeleteMark, 0) == 0)
                .Where((a, b, c) => SqlFunc.IsNull(c.IsDelete, 0) == 0)
                .WhereIF(!string.IsNullOrEmpty(input.F_work_no), a => a.WorkNo.Contains(input.F_work_no))
                .WhereIF(!string.IsNullOrEmpty(input.F_work_name), a => a.WorkName.Contains(input.F_work_name))
                .WhereIF(!string.IsNullOrEmpty(input.F_work_type), a => a.WorkType.Equals(input.F_work_type))
                .WhereIF(!string.IsNullOrEmpty(input.F_work_start), a => a.WorkStart.Equals(input.F_work_start))
                .WhereIF(!string.IsNullOrEmpty(input.F_work_end), a => a.WorkEnd.Equals(input.F_work_end))
                .Select((a, b, c) => new ZjnWcsProcessConfigListOutput
                {
                    F_Id = a.Id,
                    F_work_no = a.WorkNo,
                    F_work_name = a.WorkName,
                    F_work_type = a.WorkType,
                    F_GoodsType = a.GoodsType,
                    F_GoodsTypeName = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.GoodsType && s.DictionaryTypeId == "325449144728552709" && SqlFunc.IsNull(b.DeleteMark, 0) == 0).Select(s => s.FullName),
                    F_work_typeName = b.FullName,
                    F_work_warehouse = a.WorkWarehouse,
                    F_work_warehouseName = c.WarehouseName,
                    F_work_nodes = SqlFunc.IIF(a.WorkNodes == false, "关", "开"),
                    F_work_start = a.WorkStart,
                    F_work_startName = SqlFunc.Subqueryable<ZjnWcsWorkSiteEntity>().Where(s => s.StationId == a.WorkStart).Select(s => s.Capion),
                    F_work_end = a.WorkEnd,
                    F_work_endName = SqlFunc.Subqueryable<ZjnWcsWorkSiteEntity>().Where(s => s.StationId == a.WorkEnd).Select(s => s.Capion),
                    F_CreateUser = SqlFunc.Subqueryable<UserEntity>().Where(s => s.Id == a.CreateUser).Select(s => s.RealName),
                    F_CreateTime = a.CreateTime,
                    F_LastModifyUserId = SqlFunc.Subqueryable<UserEntity>().Where(s => s.Id == a.LastModifyUserId).Select(s => s.RealName),
                    F_LastModifyTime = a.LastModifyTime,
                })
                .MergeTable()
                .OrderBy(sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnWcsProcessConfigListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("PageListAll")]
        public async Task<dynamic> GetListPageAll([FromQuery] ZjnWcsProcessConfigListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;

            var data = await _zjnServicePathConfigRepository.AsSugarClient().Queryable<ZjnWcsProcessConfigEntity, DictionaryDataEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.WorkType.ToString() == b.EnCode))
                .Where((a, b) => b.DictionaryTypeId == "349315174420710661" && SqlFunc.IsNull(b.DeleteMark, 0) == 0)
                .WhereIF(!string.IsNullOrEmpty(input.F_work_no), a => a.WorkNo.Contains(input.F_work_no))
                .WhereIF(!string.IsNullOrEmpty(input.keyword), a => a.WorkName.Contains(input.keyword))
                .WhereIF(!string.IsNullOrEmpty(input.F_work_type), a => a.WorkType.Equals(input.F_work_type))
                .Distinct()
                .Select((a, b
) => new ZjnWcsProcessConfigListOutput
{

    F_Id = a.Id,
    F_work_no = a.WorkNo,
    F_work_name = a.WorkName,
    F_work_type = a.WorkType,
    F_work_type_Name = b.FullName,

    F_work_warehouse = a.WorkWarehouse,
    F_work_nodes = SqlFunc.IIF(a.WorkNodes == false, "关", "开"),
    F_work_start = a.WorkStart,
    F_work_end = a.WorkEnd,
    F_work_Path = a.WorkPath,
    F_CreateUser = a.CreateUser,
    F_CreateTime = a.CreateTime,
    F_LastModifyUserId = a.LastModifyUserId,
    F_LastModifyTime = a.LastModifyTime,
}).OrderBy(sidx + " " + input.sort).ToListAsync();
            //return PageResult<ZjnTaskListOutput>.SqlSugarPageResult(data);
            var pageList = new SqlSugarPagedList<ZjnWcsProcessConfigListOutput>()
            {
                list = data,
                pagination = new PagedModel()
                {
                    PageIndex = input.currentPage,
                    PageSize = input.pageSize,
                    Total = data.Count
                }
            };
            return PageResult<ZjnWcsProcessConfigListOutput>.SqlSugarPageResult(pageList);
        }



        /// <summary>
        /// 新建业务路径配置表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWcsProcessConfigCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWcsProcessConfigEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;

            var isOk = await _zjnServicePathConfigRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新业务路径配置表
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWcsProcessConfigUpInput input)
        {
            var entity = input.Adapt<ZjnWcsProcessConfigEntity>();
            entity.LastModifyUserId = _userManager.UserId;
            entity.LastModifyTime = DateTime.Now;
            var isOk = await _zjnServicePathConfigRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除业务路径配置表
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnServicePathConfigRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnServicePathConfigRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }


        #region 解析模板数据
        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("PathData")]
        public async Task<ZjnWcsProcessConfigJsonOutput> PathData(string id)
        {
            ZjnWcsProcessConfigJsonOutput jsonOutput = new ZjnWcsProcessConfigJsonOutput();

            //var output = (await _zjnServicePathConfigRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnServicePathConfigInfoOutput>();

            var zjnTaskInfoOutput = await _zjnServicePathConfigRepository.AsSugarClient().Queryable<ZjnWcsProcessConfigEntity>()
                .Where(a => a.Id == id)
                .Select((a) => new ZjnWmsTaskCrInput
                {
                    //positionCurrent = a.WorkStart,
                    //positionCurrentName = SqlFunc.Subqueryable<ZjnWcsWorkSiteEntity>().Where(s => s.StationId == a.WorkStart && SqlFunc.IsNull(s.IsDelete, 0) == 0).Select(s => s.Capion),
                    positionFrom = a.WorkStart,
                    positionFromName = SqlFunc.Subqueryable<ZjnWcsWorkSiteEntity>().Where(s => s.StationId == a.WorkStart && SqlFunc.IsNull(s.IsDelete, 0) == 0).Select(s => s.Capion),
                    positionTo = a.WorkEnd,
                    positionToName = SqlFunc.Subqueryable<ZjnWcsWorkSiteEntity>().Where(s => s.StationId == a.WorkEnd && SqlFunc.IsNull(s.IsDelete, 0) == 0).Select(s => s.Capion),
                    taskTypeNum = a.Id,
                    taskTypeName = a.WorkName,
                    taskName = a.WorkName,
                    workPath = a.WorkPath,
                    operationDirection = SqlFunc.IF(a.WorkType == 1 || a.WorkType == 6)
                                         .Return("Out")
                                         .ElseIF(a.WorkType == 2 || a.WorkType == 5)
                                         .Return("Into").End("Move"),
                    operationType = SqlFunc.IIF(a.WorkType==5 || a.WorkType==6, "emptyContainer", "production")


                    //materialCode= "WL-001",
                }).FirstAsync();

            if (zjnTaskInfoOutput == null)
            {
                throw HSZException.Oh("业务路径数据不存在");
            }

            FlowEngineEntity flowEngineEntity = new FlowEngineEntity();
            flowEngineEntity.FlowTemplateJson = zjnTaskInfoOutput.workPath;
            List<FlowTaskNodeEntity> flowTaskNodeEntityList = ParsingTemplateGetNodeList(flowEngineEntity, "", "");

            List<ZjnWmsTaskDetailsInfoOutput> ZjnTaskListDetailsList = new List<ZjnWmsTaskDetailsInfoOutput>();
            foreach (var item in flowTaskNodeEntityList)
            {
                ZjnWmsTaskDetailsInfoOutput zjnTaskListDetailsInfoOutput = new ZjnWmsTaskDetailsInfoOutput();


                if (item.NodeType == "start")
                {

                }
                if (item.NodeType == "condition")
                {
                    //var flowTemplateJsonModel = item.NodePropertyJson.Deserialize<ConditionProperties>();

                }
                if (item.NodeType == "dynamic")
                {
                    var flowTemplateJsonModel = item.NodePropertyJson.Deserialize<DynamicProperties>();

                    zjnTaskListDetailsInfoOutput.taskType = flowTemplateJsonModel.taskType;
                }
                if (item.NodeType == "approver")
                {
                    var flowTemplateJsonModel = item.NodePropertyJson.Deserialize<ApproversProperties>();


                    //var WorkPathEntity = (await _zjnWcsWorkPathRepository.GetFirstAsync(x => x.PathId == flowTemplateJsonModel.approverPath)).Adapt<ZjnWcsWorkPathEntity>();

                    zjnTaskListDetailsInfoOutput = await _zjnWcsWorkPathRepository.AsSugarClient().Queryable<ZjnWcsWorkPathEntity>()
               .Where(a => a.PathId == flowTemplateJsonModel.approverPath)
               .Select((a) => new ZjnWmsTaskDetailsInfoOutput
               {
                   taskDetailsStart = a.DeviceFrom,
                   taskDetailsStartName = SqlFunc.Subqueryable<ZjnWcsWorkSiteEntity>().Where(s => s.StationId == a.DeviceFrom && SqlFunc.IsNull(s.IsDelete, 0) == 0).Select(s => s.Capion),

                   taskDetailsEnd = a.DeviceTo,
                   taskDetailsEndName = SqlFunc.Subqueryable<ZjnWcsWorkSiteEntity>().Where(s => s.StationId == a.DeviceTo && SqlFunc.IsNull(s.IsDelete, 0) == 0).Select(s => s.Capion),

                   taskDetailsMove = a.StationTo,
                   taskDetailsMoveName = SqlFunc.Subqueryable<ZjnWcsWorkDeviceEntity>().Where(s => s.DeviceId == a.StationTo).Select(s => s.Caption),

                   workPathId = a.PathId,
                   workPathname = a.StationFrom

               }).FirstAsync();
                    zjnTaskListDetailsInfoOutput.taskType = flowTemplateJsonModel.taskType;
                }
                zjnTaskListDetailsInfoOutput.nodePropertyJson = item.NodePropertyJson;

                //zjnTaskListDetailsInfoOutput.taskDetailsId = _cacheManager.GettaskId().ToString();
                zjnTaskListDetailsInfoOutput.nodeCode = item.NodeCode;
                zjnTaskListDetailsInfoOutput.nodeType = item.NodeType;
                zjnTaskListDetailsInfoOutput.taskDetailsName = item.NodeName;
                zjnTaskListDetailsInfoOutput.nodeNext = item.NodeNext;
                zjnTaskListDetailsInfoOutput.nodeUp = item.NodeUp;
                ZjnTaskListDetailsList.Add(zjnTaskListDetailsInfoOutput);
            }
            jsonOutput.zjnTaskInfoOutput = zjnTaskInfoOutput;
            jsonOutput.FlowTemplateJson = zjnTaskInfoOutput.workPath;
            jsonOutput.ZjnTaskListDetailsList = ZjnTaskListDetailsList;

            return jsonOutput;
        }

        /// <summary>
        /// 根据表单数据解析模板获取流程节点
        /// </summary>
        /// <param name="flowEngineEntity"></param>
        /// <param name="fromData"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private List<FlowTaskNodeEntity> ParsingTemplateGetNodeList(FlowEngineEntity flowEngineEntity, string fromData, string taskId)
        {
            var taskNodeList = new List<TaskNodeModel>();
            var flowTemplateJsonModel = flowEngineEntity.FlowTemplateJson.Deserialize<FlowTemplateJsonModel>();
            #region 流程模板所有节点
            var flowTemplateJsonModelList = new List<FlowTemplateJsonModel>();
            var childNodeIdList = new List<string>();
            GetChildNodeIdList(flowTemplateJsonModel, childNodeIdList);
            GetFlowTemplateList(flowTemplateJsonModel, flowTemplateJsonModelList);
            #endregion
            GetFlowTemplateAll(flowTemplateJsonModel, taskNodeList, flowTemplateJsonModelList, childNodeIdList);
            if (fromData.IsNullOrEmpty())
            {
                return taskNodeList.Adapt<List<FlowTaskNodeEntity>>();
            }
            DeleteConditionTaskNodeModel(taskNodeList, fromData, new Dictionary<string, string>(), new Dictionary<string, object>());
            var flowNodeList = taskNodeList.Adapt<List<FlowTaskNodeEntity>>();
            DeleteEmptyOrTimerTaskNode(flowNodeList);
            return flowNodeList;
        }

        /// <summary>
        /// 递归获取流程模板数组
        /// </summary>
        /// <param name="template">流程模板</param>
        /// <param name="templateList">流程模板数组</param>
        private void GetFlowTemplateList(FlowTemplateJsonModel template, List<FlowTemplateJsonModel> templateList)
        {
            if (template.IsNotEmptyOrNull())
            {
                var haschildNode = template.childNode.IsNotEmptyOrNull();
                var hasconditionNodes = template.conditionNodes.IsNotEmptyOrNull() && template.conditionNodes.Count > 0;

                templateList.Add(template);

                if (hasconditionNodes)
                {
                    foreach (var conditionNode in template.conditionNodes)
                    {
                        GetFlowTemplateList(conditionNode, templateList);
                    }
                }
                if (haschildNode)
                {
                    GetFlowTemplateList(template.childNode, templateList);
                }
            }
        }

        /// <summary>
        /// 递归获取流程模板最外层childNode中所有nodeid
        /// </summary>
        /// <param name="template"></param>
        /// <param name="childNodeIdList"></param>
        private void GetChildNodeIdList(FlowTemplateJsonModel template, List<string> childNodeIdList)
        {
            if (template.IsNotEmptyOrNull())
            {
                if (template.childNode.IsNotEmptyOrNull())
                {
                    childNodeIdList.Add(template.childNode.nodeId);
                    GetChildNodeIdList(template.childNode, childNodeIdList);
                }
            }
        }

        /// <summary>
        /// 递归审批模板获取所有节点
        /// </summary>
        /// <param name="template">当前审批流程json</param>
        /// <param name="nodeList">流程节点数组</param>
        /// <param name="templateList">流程模板数组</param>
        private void GetFlowTemplateAll(FlowTemplateJsonModel template, List<TaskNodeModel> nodeList, List<FlowTemplateJsonModel> templateList, List<string> childNodeIdList, string taskId = "")
        {
            if (template.IsNotEmptyOrNull())
            {
                var taskNodeModel = template.Adapt<TaskNodeModel>();
                taskNodeModel.taskId = taskId;
                taskNodeModel.propertyJson = GetPropertyByType(template.type, template.properties);
                var haschildNode = template.childNode.IsNotEmptyOrNull();
                var hasconditionNodes = template.conditionNodes.IsNotEmptyOrNull() && template.conditionNodes.Count > 0;
                List<string> nextNodeIdList = new List<string> { "" };
                if (templateList.Count > 1)
                {
                    nextNodeIdList = GetNextNodeIdList(templateList, template, childNodeIdList);
                }
                taskNodeModel.nextNodeId = string.Join(',', nextNodeIdList.ToArray());
                nodeList.Add(taskNodeModel);

                if (hasconditionNodes)
                {
                    foreach (var conditionNode in template.conditionNodes)
                    {
                        GetFlowTemplateAll(conditionNode, nodeList, templateList, childNodeIdList, taskId);
                    }
                }
                if (haschildNode)
                {
                    GetFlowTemplateAll(template.childNode, nodeList, templateList, childNodeIdList, taskId);
                }
            }
        }

        /// <summary>
        /// 根据类型获取不同属性对象
        /// </summary>
        /// <param name="type">属性类型</param>
        /// <param name="jobj">数据</param>
        /// <returns></returns>
        private dynamic GetPropertyByType(string type, JObject jobj)
        {
            switch (type)
            {
                case "approver":
                    return jobj.ToObject<ApproversProperties>();
                case "timer":
                    return jobj.ToObject<TimerProperties>();
                case "start":
                    return jobj.ToObject<StartProperties>();
                case "condition":
                    return jobj.ToObject<ConditionProperties>();
                case "subFlow":
                    return jobj.ToObject<ChildTaskProperties>();
                case "dynamic":
                    return jobj.ToObject<DynamicProperties>();
                default:
                    return jobj;
            }
        }

        /// <summary>
        /// 获取当前模板的下一节点
        /// 下一节点数据来源：conditionNodes和childnode (conditionNodes优先级大于childnode)
        /// conditionNodes非空：下一节点则为conditionNodes数组中所有nodeID
        /// conditionNodes非空childNode非空：下一节点则为childNode的nodeId
        /// conditionNodes空childNode空则为最终节点(两种情况：当前模板属于conditionNodes的最终节点或childNode的最终节点)
        /// conditionNodes的最终节点:下一节点为与conditionNodes同级的childNode的nodeid,没有则继续递归，直到最外层的childNode
        /// childNode的最终节点直接为""
        /// </summary>
        /// <param name="templateList">模板数组</param>
        /// <param name="template">当前模板</param>
        /// <param name="childNodeIdList">最外层childnode的nodeid集合</param>
        /// <returns></returns>
        private List<string> GetNextNodeIdList(List<FlowTemplateJsonModel> templateList, FlowTemplateJsonModel template, List<string> childNodeIdList)
        {
            List<string> nextNodeIdList = new List<string>();
            if (template.conditionNodes.IsNotEmptyOrNull() && template.conditionNodes.Count > 0)
            {
                nextNodeIdList = template.conditionNodes.Select(x => x.nodeId).ToList();
            }
            else
            {
                if (template.childNode.IsNotEmptyOrNull())
                {
                    nextNodeIdList.Add(template.childNode.nodeId);
                }
                else
                {
                    //判断是否是最外层的节点
                    if (childNodeIdList.Contains(template.nodeId))
                    {
                        nextNodeIdList.Add("");
                    }
                    else
                    {
                        //conditionNodes中最终节点
                        nextNodeIdList.Add(GetChildId(templateList, template, childNodeIdList));
                    }
                }
            }
            return nextNodeIdList;
        }

        /// <summary>
        /// 递归获取conditionNodes最终节点下一节点
        /// </summary>
        /// <param name="templateList">流程模板数组</param>
        /// <param name="template">当前模板</param>
        /// <param name="childNodeIdList">最外层childNode的节点数据</param>
        /// <returns></returns>
        private string GetChildId(List<FlowTemplateJsonModel> templateList, FlowTemplateJsonModel template, List<string> childNodeIdList)
        {
            var prevModel = new FlowTemplateJsonModel();
            if (template.prevId.IsNotEmptyOrNull())
            {
                prevModel = templateList.Find(x => x.nodeId.Equals(template.prevId));
                if (prevModel.childNode.IsNotEmptyOrNull() && prevModel.childNode.nodeId != template.nodeId)
                {
                    return prevModel.childNode.nodeId;
                }
                if (childNodeIdList.Contains(prevModel.nodeId))
                {
                    return prevModel.childNode.IsNullOrEmpty() ? "" : prevModel.childNode.nodeId;
                }
                else
                {
                    return GetChildId(templateList, prevModel, childNodeIdList);
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 删除条件节点
        /// 将条件的上非条件的节点的nextnode替换成当前条件的nextnode
        /// </summary>
        /// <param name="taskNodeModelList">所有节点数据</param>
        /// <param name="formDataJson">填写表单数据</param>
        /// <param name="hszKey"></param>
        /// <param name="keyList"></param>
        /// <returns></returns>
        private void DeleteConditionTaskNodeModel(List<TaskNodeModel> taskNodeModelList, string formDataJson, Dictionary<string, string> hszKey, Dictionary<string, object> keyList)
        {
            var conditionTaskNodeModelList = taskNodeModelList.FindAll(x => x.type.Equals("condition"));
            //条件的默认情况判断（同层条件的父节点是一样的，只要非默认的匹配成功则不需要走默认的）
            var isDefault = new List<string>();
            foreach (var item in conditionTaskNodeModelList)
            {
                //条件节点的父节点且为非条件的节点
                var upTaskNodeModel = taskNodeModelList.Where(x => x.nodeId == item.upNodeId).FirstOrDefault();
                if (upTaskNodeModel.type.Equals("condition"))
                {
                    upTaskNodeModel = GetUpTaskNodeModelIsNotCondition(taskNodeModelList, upTaskNodeModel);
                }
                if (!item.propertyJson.isDefault && ConditionNodeJudge(formDataJson, item.propertyJson, hszKey, keyList))
                {
                    upTaskNodeModel.nextNodeId = item.nextNodeId;
                    isDefault.Add(item.upNodeId);
                }
                else
                {
                    if (!isDefault.Contains(item.upNodeId) && item.propertyJson.isDefault)
                    {
                        upTaskNodeModel.nextNodeId = item.nextNodeId;
                    }
                }

            }
            taskNodeModelList.RemoveAll(x => "condition".Equals(x.type));
        }

        /// <summary>
        /// 向上递获取非条件的节点
        /// </summary>
        /// <param name="taskNodeModelList">所有节点数据</param>
        /// <param name="taskNodeModel">当前节点</param>
        /// <returns></returns>
        private TaskNodeModel GetUpTaskNodeModelIsNotCondition(List<TaskNodeModel> taskNodeModelList, TaskNodeModel taskNodeModel)
        {
            var preTaskNodeModel = taskNodeModelList.Find(x => x.nodeId == taskNodeModel.upNodeId);
            if (preTaskNodeModel.type.Equals("condition"))
            {
                return GetUpTaskNodeModelIsNotCondition(taskNodeModelList, preTaskNodeModel);
            }
            else
            {
                return preTaskNodeModel;
            }
        }

        /// <summary>
        /// 条件判断
        /// </summary>
        /// <param name="formDataJson">表单填写数据</param>
        /// <param name="conditionPropertie">条件属性</param>
        /// <param name="hszKey"></param>
        /// <param name="keyList"></param>
        /// <returns></returns>
        private bool ConditionNodeJudge(string formDataJson, ConditionProperties conditionPropertie, Dictionary<string, string> hszKey, Dictionary<string, object> keyList)
        {
            try
            {
                bool flag = false;
                StringBuilder expression = new StringBuilder();
                var formData = formDataJson.Deserialize<JObject>();
                int i = 0;
                foreach (ConditionsModel flowNodeWhereModel in conditionPropertie.conditions)
                {
                    var logic = flowNodeWhereModel.logic;
                    var formValue = formData.ContainsKey(flowNodeWhereModel.field) ? formData[flowNodeWhereModel.field].ToString() : "";
                    var symbol = flowNodeWhereModel.symbol.Equals("==") ? "=" : flowNodeWhereModel.symbol;
                    var value = flowNodeWhereModel.filedValue;
                    var hszkey = hszKey.ContainsKey(flowNodeWhereModel.field) ? hszKey[flowNodeWhereModel.field].ToString() : null;
                    if (hszKey.IsNotEmptyOrNull())
                    {
                        var condFiledValue = value.Trim();
                        //下拉和单选
                        if ("select".Equals(hszkey) || "radio".Equals(hszkey))
                        {
                            List<Dictionary<string, string>> dataList = keyList[flowNodeWhereModel.field].Serialize().Deserialize<List<Dictionary<string, string>>>();
                            Dictionary<string, string> data = dataList.Find(x => x["id"].ToString().Equals(condFiledValue) || x["fullName"].ToString().Equals(condFiledValue));
                            if (data != null)
                            {
                                value = data["id"].ToString();
                            }
                        }
                        //公司和部门
                        if ("comSelect".Equals(hszkey) || "depSelect".Equals(hszkey))
                        {
                            List<OrganizeEntity> dataList = keyList[flowNodeWhereModel.field].Serialize().Deserialize<List<OrganizeEntity>>();
                            OrganizeEntity organize = dataList.Find(x => x.Id.Equals(condFiledValue) || x.FullName.Equals(condFiledValue));
                            if (organize != null)
                            {
                                value = organize.Id;
                            }
                        }
                        //岗位
                        if ("posSelect".Equals(hszkey))
                        {
                            List<PositionEntity> dataList = keyList[flowNodeWhereModel.field].Serialize().Deserialize<List<PositionEntity>>();
                            PositionEntity position = dataList.Find(x => x.Id.Equals(condFiledValue) || x.FullName.Equals(condFiledValue));
                            if (position != null)
                            {
                                value = position.Id;
                            }
                        }
                        //字典
                        if ("dicSelect".Equals(hszkey))
                        {
                            List<DictionaryTypeEntity> dictypeList = keyList[flowNodeWhereModel.field].Serialize().Deserialize<List<DictionaryTypeEntity>>();
                            DictionaryTypeEntity dic = dictypeList.Find(x => x.Id.Equals(formValue));
                            if (dic != null)
                            {
                                value = dic.Id;
                            }
                        }
                        //用户
                        if ("userSelect".Equals(hszkey))
                        {
                            List<UserEntity> dataList = keyList[flowNodeWhereModel.field].Serialize().Deserialize<List<UserEntity>>();
                            UserEntity user = dataList.Find(x => x.Id.Equals(condFiledValue) || x.RealName.Equals(condFiledValue) || x.Account.Equals(condFiledValue));
                            if (user != null)
                            {
                                value = user.Id;
                            }
                        }
                    }

                    if (symbol.Equals("=") || symbol.Equals("<>"))
                    {
                        expression.AppendFormat("select * from zjn_base_user where  '{0}'{1}'{2}'", formValue, symbol, value);
                    }
                    else if (symbol.Equals("like"))
                    {
                        expression.AppendFormat("select * from zjn_base_user where  '{0}' {1} '%{2}%'", formValue, symbol, value);
                    }
                    else if (symbol.Equals("notlike"))
                    {
                        expression.AppendFormat("select * from zjn_base_user where  '{0}' {1} '%{2}%'", formValue, "not like", value);
                    }
                    else
                    {
                        expression.AppendFormat("select * from zjn_base_user where {0}{1}{2}", formValue, symbol, value);
                    }

                    if (!string.IsNullOrEmpty(logic))
                    {
                        if (i != conditionPropertie.conditions.Count - 1)
                        {
                            expression.Append(" " + logic.Replace("&&", " and ").Replace("||", " or ") + " ");
                        }
                    }
                    i++;
                }
                flag = _changeDataBase.WhereDynamicFilter(null, expression.ToString());
                return flag;
            }
            catch (Exception e)
            {
                return false;
                throw new Exception(e.Message);

            }
        }

        /// <summary>
        /// 删除定时器和空节点
        /// </summary>
        /// <param name="flowTaskNodeEntityList"></param>
        private void DeleteEmptyOrTimerTaskNode(List<FlowTaskNodeEntity> flowTaskNodeEntityList)
        {
            var emptyTaskNodeList = flowTaskNodeEntityList.FindAll(x => "empty".Equals(x.NodeType));
            var timerTaskNodeList = flowTaskNodeEntityList.FindAll(x => "timer".Equals(x.NodeType));
            foreach (var item in emptyTaskNodeList)
            {
                //下-节点为empty类型节点的节点集合
                var taskNodeList = flowTaskNodeEntityList.FindAll(x => x.NodeNext.Contains(item.NodeCode));
                //替换节点
                foreach (var taskNode in taskNodeList)
                {
                    var flowTaskNodeEntity = flowTaskNodeEntityList.Where(x => x.NodeCode == taskNode.NodeCode).FirstOrDefault();
                    flowTaskNodeEntity.NodeNext = item.NodeNext;
                }
            }
            foreach (var item in timerTaskNodeList)
            {
                //下一节点为Timer类型节点的节点集合
                var taskNodeList = flowTaskNodeEntityList.FindAll(x => x.NodeNext.Contains(item.NodeCode));
                //Timer类型节点的下节点集合
                var nextTaskNodeList = flowTaskNodeEntityList.FindAll(x => item.NodeNext.Contains(x.NodeCode));
                //保存定时器节点的上节点编码到属性中
                var timerProperties = item.NodePropertyJson.Deserialize<TimerProperties>();
                timerProperties.upNodeCode = string.Join(",", taskNodeList.Select(x => x.NodeCode).ToArray());
                item.NodePropertyJson = timerProperties.Serialize();
                //上节点替换NodeNext
                foreach (var taskNode in taskNodeList)
                {
                    var flowTaskNodeEntity = flowTaskNodeEntityList.Where(x => x.NodeCode == taskNode.NodeCode).FirstOrDefault();
                    flowTaskNodeEntity.NodeNext = item.NodeNext;
                }
                //下节点添加定时器属性
                nextTaskNodeList.ForEach(nextNode =>
                {
                    var flowTaskNodeEntity = flowTaskNodeEntityList.Where(x => x.NodeCode == nextNode.NodeCode).FirstOrDefault();
                    if (flowTaskNodeEntity.NodeType.Equals("approver"))
                    {
                        var properties = flowTaskNodeEntity.NodePropertyJson.Deserialize<ApproversProperties>();
                        properties.timerList.Add(item.NodePropertyJson.Deserialize<TimerProperties>());
                        flowTaskNodeEntity.NodePropertyJson = properties.Serialize();
                    }
                });
            }
            flowTaskNodeEntityList.RemoveAll(x => "empty".Equals(x.NodeType));
            flowTaskNodeEntityList.RemoveAll(x => "timer".Equals(x.NodeType));
            UpdateNodeSort(flowTaskNodeEntityList);
        }

        /// <summary>
        /// 封装属性key和保存list
        /// </summary>
        /// <param name="fieLdsModelList">控件数据集合</param>
        /// <param name="hszKey"></param>
        /// <param name="keyList"></param>
        private async Task tempJson(List<FieldsModel> fieLdsModelList, Dictionary<string, string> hszKey, Dictionary<string, object> keyList)
        {
            List<DictionaryDataEntity> dictionaryDataList = await _dictionaryDataService.GetList();
            foreach (var fieLdsModel in fieLdsModelList)
            {
                string model = fieLdsModel.__vModel__;
                ConfigModel config = fieLdsModel.__config__;
                string key = config.hszKey;
                if (model.IsNotEmptyOrNull())
                {
                    hszKey.Add(model, key);
                }
                if ("select".Equals(key) || "checkbox".Equals(key) || "radio".Equals(key))
                {
                    string type = config.dataType;
                    List<Dictionary<string, string>> optionslList = new List<Dictionary<string, string>>();
                    string fullName = config.props.label;
                    string value = config.props.value;
                    if ("dictionary".Equals(type))
                    {
                        string dictionaryType = config.dictionaryType;
                        List<DictionaryDataEntity> dicList = dictionaryDataList.FindAll(x => x.DictionaryTypeId.Equals(dictionaryType));
                        foreach (DictionaryDataEntity dataEntity in dicList)
                        {
                            Dictionary<string, string> optionsModel = new Dictionary<string, string>();
                            optionsModel.Add("id", dataEntity.Id);
                            optionsModel.Add("fullName", dataEntity.FullName);
                            optionslList.Add(optionsModel);
                        }
                    }
                    else if ("static".Equals(type))
                    {
                        List<Dictionary<string, object>> staticList = fieLdsModel.__slot__.options;
                        foreach (Dictionary<string, object> options in staticList)
                        {
                            Dictionary<string, string> optionsModel = new Dictionary<string, string>();
                            optionsModel.Add("id", options["id"].ToString());
                            optionsModel.Add("fullName", options["fullName"].ToString());
                            optionslList.Add(optionsModel);
                        }
                    }
                    else if ("dynamic".Equals(type))
                    {
                        string dynId = config.propsUrl;
                        //查询外部接口
                        Dictionary<string, object> dynamicMap = new Dictionary<string, object>();
                        dynamicMap.Add("data", await _dataInterfaceService.GetData(dynId));
                        if (dynamicMap["data"] != null)
                        {
                            List<Dictionary<string, object>> dataList = dynamicMap["data"].Serialize().Deserialize<List<Dictionary<string, object>>>();
                            foreach (Dictionary<string, object> options in dataList)
                            {
                                Dictionary<string, string> optionsModel = new Dictionary<string, string>();
                                optionsModel.Add("id", options[value].ToString());
                                optionsModel.Add("fullName", options[fullName].ToString());
                                optionslList.Add(optionsModel);
                            }
                        }
                    }
                    keyList.Add(model, optionslList);
                }
            }
        }

        /// <summary>
        /// 处理并保存节点
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        private void UpdateNodeSort(List<FlowTaskNodeEntity> entitys)
        {
            var startNodes = entitys.FindAll(x => x.NodeType.Equals("start"));
            if (startNodes.Count > 0)
            {
                var startNode = startNodes[0].NodeCode;
                long num = 0L;
                long maxNum = 0L;
                var max = new List<long>();
                var _treeList = new List<FlowTaskNodeEntity>();
                NodeList(entitys, startNode, _treeList, num, max);
                max.Sort();
                if (max.Count > 0)
                {
                    maxNum = max[max.Count - 1];
                }
                var nodeNext = "end";
                foreach (var item in entitys)
                {
                    var type = item.NodeType;
                    var node = _treeList.Find(x => x.NodeCode.Equals(item.NodeCode));
                    if (item.NodeNext.IsEmpty())
                    {
                        item.NodeNext = nodeNext;
                    }
                    if (node.IsNotEmptyOrNull())
                    {
                        item.SortCode = node.SortCode;
                        item.State = "0";
                        if (item.NodeNext.IsEmpty())
                        {
                            item.NodeNext = nodeNext;
                        }
                    }
                }
                entitys.Add(new FlowTaskNodeEntity()
                {
                    Id = YitIdHelper.NextId().ToString(),
                    NodeCode = nodeNext,
                    NodeName = "结束",
                    Completion = 0,
                    CreatorTime = DateTime.Now,
                    SortCode = maxNum + 1,
                    TaskId = _treeList[0].TaskId,
                    NodePropertyJson = startNodes[0].NodePropertyJson,
                    NodeType = "endround",
                    State = "0"
                });
            }
        }

        /// <summary>
        /// 递归获取经过的节点
        /// </summary>
        /// <param name="dataAll"></param>
        /// <param name="nodeCode"></param>
        /// <param name="_treeList"></param>
        /// <param name="num"></param>
        /// <param name="max"></param>
        private void NodeList(List<FlowTaskNodeEntity> dataAll, string nodeCode, List<FlowTaskNodeEntity> _treeList, long num, List<long> max)
        {
            num++;
            max.Add(num);
            var thisEntity = dataAll.FindAll(x => x.NodeCode.Contains(nodeCode));
            foreach (var item in thisEntity)
            {
                item.SortCode = num;
                item.State = "0";
                _treeList.Add(item);
                foreach (var nodeNext in item.NodeNext.Split(","))
                {
                    long nums = _treeList.FindAll(x => x.NodeCode.Equals(nodeNext)).Count;
                    if (nodeNext.IsNotEmptyOrNull() && nums == 0)
                    {
                        NodeList(dataAll, nodeNext, _treeList, num, max);
                    }
                }
            }
        }
        #endregion
    }
}


