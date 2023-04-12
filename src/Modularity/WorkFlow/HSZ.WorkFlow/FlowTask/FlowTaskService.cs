using HSZ.ChangeDataBase;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.LinqBuilder;
using HSZ.RemoteRequest.Extensions;
using HSZ.System.Entitys.Entity.System;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.Permission;
using HSZ.System.Interfaces.System;
using HSZ.UnifyResult;
using HSZ.VisualDev.Entitys;
using HSZ.VisualDev.Entitys.Dto.VisualDevModelData;
using HSZ.VisualDev.Entitys.Model.VisualDevModelData;
using HSZ.VisualDev.Interfaces;
using HSZ.VisualDev.Run.Interfaces;
using HSZ.WorkFlow.Entitys;
using HSZ.WorkFlow.Entitys.Dto.FlowBefore;
using HSZ.WorkFlow.Entitys.Dto.FlowTask;
using HSZ.WorkFlow.Entitys.Enum;
using HSZ.WorkFlow.Entitys.Model;
using HSZ.WorkFlow.Entitys.Model.Properties;
using HSZ.WorkFlow.Interfaces.FLowDelegate;
using HSZ.WorkFlow.Interfaces.FlowEngine;
using HSZ.WorkFlow.Interfaces.FlowTask;
using HSZ.WorkFlow.Interfaces.FlowTask.Repository;
using HSZ.WorkFlow.Interfaces.WorkFlowForm;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.WorkFlow.Core.Service.FlowTask
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：流程任务
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowEngine", Name = "FlowTask", Order = 306)]
    [Route("api/workflow/Engine/[controller]")]
    public class FlowTaskService : IFlowTaskService, IDynamicApiController, ITransient
    {
        private readonly IFlowTaskRepository _flowTaskRepository;
        private readonly IDictionaryDataService _dictionaryDataService;
        private readonly IFlowDelegateService _flowDelegateService;
        private readonly IFlowEngineService _flowEngineService;
        private readonly IDataInterfaceService _dataInterfaceService;
        private readonly IUsersService _usersService;
        private readonly IOrganizeService _organizeService;
        private readonly IUserRelationService _userRelationService;
        private readonly IUserManager _userManager;
        private readonly IRunService _runService;
        private readonly IBillRullService _billRullService;
        private readonly IVisualDevService _visualDevServce;
        private readonly IDbLinkService _dbLinkService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly ILogger<FlowTaskService> _logger;
        private readonly SqlSugarScope Db;
        private readonly IChangeDataBase _changeDataBase;
        private string taskId = "";
        private string taskNodeId = "";

        /// <summary>
        ///
        /// </summary>
        public FlowTaskService(IFlowTaskRepository flowTaskRepository,
            IDictionaryDataService dictionaryDataService, IUsersService usersService,
            IFlowEngineService flowEngineService, IOrganizeService organizeService,
            IDataInterfaceService dataInterfaceService, IFlowDelegateService flowDelegateService,
            IUserRelationService userRelationService, IUserManager userManager,
            IRunService runService, IBillRullService billRullService,
            IVisualDevService visualDevServce, IChangeDataBase changeDataBase,
            IDbLinkService dbLinkService, IMessageTemplateService messageTemplateService,
            ILogger<FlowTaskService> logger)
        {
            _flowTaskRepository = flowTaskRepository;
            _dictionaryDataService = dictionaryDataService;
            _flowEngineService = flowEngineService;
            _usersService = usersService;
            _dataInterfaceService = dataInterfaceService;
            _organizeService = organizeService;
            _flowDelegateService = flowDelegateService;
            _userRelationService = userRelationService;
            _userManager = userManager;
            _runService = runService;
            _billRullService = billRullService;
            _visualDevServce = visualDevServce;
            _changeDataBase = changeDataBase;
            _dbLinkService = dbLinkService;
            _messageTemplateService = messageTemplateService;
            _logger = logger;
            Db = DbScoped.SugarScope;
        }

        #region Get
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var flowEntity = await _flowTaskRepository.GetTaskInfo(id);
            var output = await GetFlowDynamicDataManage(flowEntity);
            return output;
        }

        #endregion

        #region Post
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] FlowTaskCrInput input)
        {
            try
            {
                if (input.status == 1)
                {
                    await Save(null, input.flowId, null, null, 1, null, input.data.ToObject(), 1, 0, false);
                }
                else
                {
                    var flag = await Submit(null, input.flowId, null, null, 1, null, input.data.ToObject(), 0, 0, false, false, input.candidateList);
                    if (!flag)
                        throw HSZException.Oh(ErrorCode.WF0005);
                }
            }
            catch (Exception)
            {
                throw HSZException.Oh(ErrorCode.WF0005);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] FlowTaskUpInput input)
        {
            try
            {
                //if (_userManager.UserId.Equals("admin"))
                //    throw HSZException.Oh(ErrorCode.WF0004);
                if (input.status == 1)
                {
                    await Save(id, input.flowId, null, null, 1, null, input.data.ToObject(), 1, 0, false);
                }
                else
                {
                    var flag = await Submit(id, input.flowId, null, null, 1, null, input.data.ToObject(), 0, 0, false, false, input.candidateList);
                    if (!flag)
                        throw HSZException.Oh(ErrorCode.WF0005);
                }
            }
            catch (Exception)
            {
                throw HSZException.Oh(ErrorCode.WF0005);
            }
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// 获取任务详情
        /// </summary>
        /// <param name="id">任务id</param>
        /// <param name="taskNodeId">节点id</param>
        /// <param name="taskOperatorId">审批人id</param>
        /// <returns></returns>
        [NonAction]
        public async Task<FlowBeforeInfoOutput> GetFlowBeforeInfo(string id, string taskNodeId, string taskOperatorId = null)
        {
            try
            {
                var output = new FlowBeforeInfoOutput();
                var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(id);
                var flowEngineEntity = await _flowEngineService.GetInfo(flowTaskEntity.FlowId);
                var flowTaskNodeEntityList = (await _flowTaskRepository.GetTaskNodeList(flowTaskEntity.Id)).FindAll(x => "0".Equals(x.State));
                var flowTaskNodeList = flowTaskNodeEntityList.Adapt<List<FlowTaskNodeModel>>().OrderBy(x => x.sortCode).ToList();
                var flowTaskOperatorList = (await _flowTaskRepository.GetTaskOperatorList(flowTaskEntity.Id)).FindAll(t => "0".Equals(t.State));
                var flowTaskOperatorRecordList = (await _flowTaskRepository.GetTaskOperatorRecordList(flowTaskEntity.Id)).Adapt<List<FlowTaskOperatorRecordModel>>();
                var colorFlag = true;
                foreach (var item in flowTaskOperatorRecordList)
                {
                    item.userName = await _usersService.GetUserName(item.handleId);
                    item.operatorId = await _usersService.GetUserName(item.operatorId);
                }
                foreach (var item in flowTaskNodeList)
                {
                    #region 流程图节点颜色类型
                    if (colorFlag || item.completion == 1)
                    {
                        item.type = "0";
                    }
                    if (flowTaskEntity.ThisStepId.Contains(item.nodeCode))
                    {
                        item.type = "1";
                        colorFlag = false;
                    }
                    if (flowTaskEntity.ThisStepId == "end")
                    {
                        item.type = "0";
                    }
                    #endregion
                    item.userName = await GetApproverUserName(item, flowTaskEntity, flowTaskEntity.FlowFormContentJson, flowTaskNodeEntityList);
                }
                var thisNode = (await _flowTaskRepository.GetTaskNodeList(flowTaskEntity.Id)).Find(x => x.Id == taskNodeId);
                if (thisNode.IsNotEmptyOrNull())
                {
                    var thisNodeProperties = thisNode.NodePropertyJson.Deserialize<ApproversProperties>();
                    output.approversProperties = thisNodeProperties;
                    output.formOperates = thisNodeProperties.formOperates.Adapt<List<FormOperatesModel>>();
                }
                output.flowFormInfo = flowTaskEntity.FlowForm;
                output.flowTaskInfo = flowTaskEntity.Adapt<FlowTaskModel>();
                output.flowTaskInfo.appFormUrl = flowEngineEntity.AppFormUrl;
                output.flowTaskInfo.formUrl = flowEngineEntity.FormUrl;
                output.flowTaskInfo.type = flowEngineEntity.Type;
                output.flowTaskNodeList = flowTaskNodeList;
                output.flowTaskOperatorList = flowTaskOperatorList.Adapt<List<FlowTaskOperatorModel>>();
                output.flowTaskOperatorRecordList = flowTaskOperatorRecordList;
                if (taskOperatorId.IsNotEmptyOrNull())
                {
                    var flowTaskOperator = flowTaskOperatorList.Find(x => x.Id == taskOperatorId);
                    if (flowTaskOperator.IsNotEmptyOrNull() && flowTaskOperator.DraftData.IsNotEmptyOrNull())
                    {
                        output.draftData = flowTaskOperator.DraftData.ToObject();
                    }
                }
                return output;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="id">任务主键id（通过空值判断是修改还是新增）</param>
        /// <param name="flowId">引擎id</param>
        /// <param name="processId">关联id</param>
        /// <param name="flowTitle">任务名</param>
        /// <param name="flowUrgent">紧急程度（自定义默认为1）</param>
        /// <param name="billNo">单据规则</param>
        /// <param name="formData">表单数据</param>
        /// <param name="status">状态 1:保存，0提交</param>
        /// <param name="approvaUpType">审批修改权限1：可写，0：可读</param>
        /// <param name="isSysTable">true：系统表单，false：自定义表单</param>
        /// <param name="parentId">任务父id</param>
        /// <param name="crUser">子流程发起人</param>
        /// <param name="isDev">是否功能设计</param>
        /// <param name="isAsync">是否异步</param>
        /// <returns></returns>
        [NonAction]
        public async Task<FlowTaskEntity> Save(string id, string flowId, string processId, string flowTitle, int? flowUrgent, string billNo, object formData,
            int status, int? approvaUpType = 0, bool isSysTable = true, string parentId = "0", string crUser = null, bool isDev = false, bool isAsync = false)
        {
            try
            {
                var flowTaskEntity = new FlowTaskEntity();
                if (!isSysTable)
                {
                    var flowTaskEntityDynamic = await FlowDynamicDataManage(id, flowId, processId, flowTitle, flowUrgent, billNo, formData, crUser, isDev);
                    processId = flowTaskEntityDynamic.ProcessId;
                    flowTitle = flowTaskEntityDynamic.FlowName;
                    flowUrgent = flowTaskEntityDynamic.FlowUrgent;
                    billNo = flowTaskEntityDynamic.EnCode;
                    formData = flowTaskEntityDynamic.FlowFormContentJson.ToObject();
                }
                if (id.IsEmpty())
                {
                    FlowEngineEntity flowEngineEntity = await _flowEngineService.GetInfo(flowId);
                    flowTaskEntity.Id = processId;
                    flowTaskEntity.ProcessId = processId;
                    flowTaskEntity.EnCode = billNo;
                    flowTaskEntity.FullName = parentId.Equals("0") ? flowTitle : flowTitle + "(子流程)";
                    flowTaskEntity.FlowUrgent = flowUrgent;
                    flowTaskEntity.FlowId = flowEngineEntity.Id;
                    flowTaskEntity.FlowCode = flowEngineEntity.EnCode;
                    flowTaskEntity.FlowName = flowEngineEntity.FullName;
                    flowTaskEntity.FlowType = flowEngineEntity.Type;
                    flowTaskEntity.FlowCategory = flowEngineEntity.Category;
                    flowTaskEntity.FlowForm = flowEngineEntity.FormTemplateJson;
                    flowTaskEntity.FlowFormContentJson = formData == null ? "" : formData.ToJson();
                    flowTaskEntity.FlowTemplateJson = flowEngineEntity.FlowTemplateJson;
                    flowTaskEntity.FlowVersion = flowEngineEntity.Version;
                    flowTaskEntity.Status = FlowTaskStatusEnum.Draft;
                    flowTaskEntity.Completion = 0;
                    flowTaskEntity.ThisStep = "开始";
                    flowTaskEntity.CreatorTime = DateTime.Now;
                    flowTaskEntity.CreatorUserId = crUser.IsEmpty() ? _userManager.UserId : crUser;
                    flowTaskEntity.ParentId = parentId;
                    flowTaskEntity.IsAsync = isAsync ? 1 : 0;
                    if (status == 0)
                    {
                        flowTaskEntity.Status = FlowTaskStatusEnum.Handle;
                        flowTaskEntity.EnabledMark = FlowTaskStatusEnum.Handle;
                        flowTaskEntity.StartTime = DateTime.Now;
                        flowTaskEntity.CreatorTime = DateTime.Now;
                    }
                    await _flowTaskRepository.CreateTask(flowTaskEntity);
                }
                else
                {
                    flowTaskEntity = await _flowTaskRepository.GetTaskInfo(id);
                    if (!CheckStatus(flowTaskEntity.Status) && approvaUpType == 0)
                        throw new Exception("当前流程正在运行不能重复保存");
                    if (status == 0)
                    {
                        flowTaskEntity.Status = FlowTaskStatusEnum.Handle;
                        flowTaskEntity.StartTime = DateTime.Now;
                        flowTaskEntity.LastModifyTime = DateTime.Now;
                        flowTaskEntity.LastModifyUserId = _userManager.UserId;
                    }
                    if (approvaUpType == 0)
                    {
                        flowTaskEntity.FullName = parentId.Equals("0") ? flowTitle : flowTitle + "(子流程)";
                        flowTaskEntity.FlowUrgent = flowUrgent;
                    }
                    if (formData != null)
                    {
                        flowTaskEntity.FlowFormContentJson = formData.Serialize();
                    }
                    await _flowTaskRepository.UpdateTask(flowTaskEntity);
                }
                return flowTaskEntity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 自定义表单数据处理(保存或提交)
        /// </summary>
        /// <param name="id">主键id（通过空值判断是修改还是新增）</param>
        /// <param name="flowId">流程id</param>
        /// <param name="processId"></param>
        /// <param name="flowTitle">流程任务名</param>
        /// <param name="flowUrgent">紧急程度（自定义默认为1）</param>
        /// <param name="billNo"></param>
        /// <param name="formData">表单填写的数据</param>
        /// <param name="crUser">子流程发起人</param>
        /// <param name="isDev">是否功能设计</param>
        /// <returns></returns>
        [NonAction]
        public async Task<FlowTaskEntity> FlowDynamicDataManage(string id, string flowId, string processId, string flowTitle, int? flowUrgent, string billNo, object formData, string crUser, bool isDev)
        {
            try
            {
                FlowEngineEntity flowEngineEntity = await _flowEngineService.GetInfo(flowId);
                billNo = "单据规则不存在";
                flowTitle = _userManager.User.RealName + "的" + flowEngineEntity.FullName;
                if (crUser.IsNotEmptyOrNull())
                {
                    flowTitle = _usersService.GetInfoByUserId(crUser).RealName + "的" + flowEngineEntity.FullName;
                }
                processId = processId.IsNullOrEmpty() ? YitIdHelper.NextId().ToString() : processId;
                flowUrgent = 1;
                //表单模板list
                List<FieldsModel> fieldsModelList = flowEngineEntity.FormTemplateJson.Deserialize<FormDataModel>().fields;
                //剔除布局控件
                fieldsModelList = _runService.TemplateDataConversion(fieldsModelList);
                //待保存表单数据
                Dictionary<string, object> formDataDic = formData.ToObject<Dictionary<string, object>>();
                //有表无表
                bool isTable = flowEngineEntity.Tables.Equals("[]");
                //新增或修改
                bool type = id.IsEmpty();
                if (!type)
                {
                    var entity = await _flowTaskRepository.GetTaskInfo(id);
                    processId = id;
                    flowTitle = entity.FullName;
                    billNo = entity.EnCode;
                    flowUrgent = entity.FlowUrgent;
                }
                #region 待保存表单数据
                VisualDevEntity visualdevEntity = new VisualDevEntity() { Id = processId, FormData = flowEngineEntity.FormTemplateJson, Tables = flowEngineEntity.Tables, DbLinkId = flowEngineEntity.DbLinkId };
                VisualDevModelDataCrInput visualdevModelDataCrForm = new VisualDevModelDataCrInput() { data = formData.ToJson() };
                var dbLink = await _dbLinkService.GetInfo(flowEngineEntity.DbLinkId);
                if (dbLink == null)
                    dbLink = _runService.GetTenantDbLink();
                #endregion
                if (!isTable)
                {
                    var sql = new List<string>();
                    if (type)
                    {
                        sql = await _runService.CreateHaveTableSql(visualdevEntity, visualdevModelDataCrForm, processId);
                    }
                    else
                    {
                        sql = await _runService.UpdateHaveTableSql(visualdevEntity, visualdevModelDataCrForm.Adapt<VisualDevModelDataUpInput>(), id);
                    }
                    if (!isDev)
                    {
                        foreach (var item in sql)
                        {
                            await _changeDataBase.ExecuteSql(dbLink, item);
                        }
                    }
                }
                else
                {
                    //获取旧数据
                    if (!type)
                    {
                        var oldEntity = await _flowTaskRepository.GetTaskInfo(id);
                        var oldAllDataMap = oldEntity.FlowFormContentJson.Deserialize<Dictionary<string, object>>();
                        var curr = fieldsModelList.Where(x => x.__config__.hszKey == "currOrganize" || x.__config__.hszKey == "currPosition").Select(x => x.__vModel__).ToList();
                        foreach (var item in curr) formDataDic[item] = oldAllDataMap[item];//当前组织和当前岗位不做修改
                    }
                    //无表处理后待保存数据
                    formData = await _runService.GenerateFeilds(fieldsModelList, formDataDic, type);
                    //修改时单据规则不做修改
                    if (type)
                    {
                        var fieLdsModel = fieldsModelList.FindAll(x => "billRule".Equals(x.__config__.hszKey));
                        if (fieLdsModel.Count > 0)
                        {
                            string ruleKey = fieLdsModel.FirstOrDefault().__config__.rule;
                            billNo = await _billRullService.GetBillNumber(ruleKey, true);
                        }
                    }
                }
                var flowTaskEntity = new FlowTaskEntity();
                flowTaskEntity.Id = id;
                flowTaskEntity.FlowId = flowId;
                flowTaskEntity.ProcessId = processId;
                flowTaskEntity.FlowName = flowTitle;
                flowTaskEntity.FlowUrgent = flowUrgent;
                flowTaskEntity.EnCode = billNo;
                flowTaskEntity.FlowFormContentJson = formData.ToJson();
                return flowTaskEntity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="id">主键id（通过空值判断是修改还是新增）</param>
        /// <param name="flowId">引擎id</param>
        /// <param name="processId">关联id</param>
        /// <param name="flowTitle">任务名</param>
        /// <param name="flowUrgent">紧急程度（自定义默认为1）</param>
        /// <param name="billNo">单据规则</param>
        /// <param name="formData">表单数据</param>
        /// <param name="status">状态 1:保存，0提交</param>
        /// <param name="approvaUpType">审批修改权限1：可写，0：可读</param>
        /// <param name="isSysTable">true：系统表单，false：自定义表单</param>
        /// <param name="isDev">是否功能设计</param>
        /// <param name="candidateList">候选人</param>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> Submit(string id, string flowId, string processId, string flowTitle, int? flowUrgent, string billNo, object formData, int status, int? approvaUpType = 0, bool isSysTable = true, bool isDev = false, Dictionary<string, List<string>> candidateList = null)
        {
            try
            {
                Db.BeginTran();
                #region 流程引擎
                FlowEngineEntity flowEngineEntity = await _flowEngineService.GetInfo(flowId);
                //判断字段类型
                Dictionary<string, string> hszKey = new Dictionary<string, string>();
                //下拉、单选、高级控件的list
                Dictionary<string, object> keyList = new Dictionary<string, object>();
                if (flowEngineEntity.FormType == 2)
                {
                    List<FieldsModel> fieldsModelList = flowEngineEntity.FormTemplateJson.Deserialize<FormDataModel>().fields;
                    await tempJson(fieldsModelList, hszKey, keyList);
                }
                #endregion

                #region 流程任务
                FlowTaskEntity flowTaskEntity = await this.Save(id, flowId, processId, flowTitle, flowUrgent, billNo, formData, status, approvaUpType, isSysTable, "0", null, isDev);
                #endregion

                #region 流程节点
                List<FlowTaskNodeEntity> flowTaskNodeEntityList = ParsingTemplateGetNodeList(flowEngineEntity,formData.Serialize(), flowTaskEntity.Id);
                SaveNodeCandidates(flowTaskNodeEntityList, candidateList, "0");
                await _flowTaskRepository.CreateTaskNode(flowTaskNodeEntityList);
                #endregion

                List<FlowTaskOperatorEntity> flowTaskOperatorEntityList = new List<FlowTaskOperatorEntity>();

                //开始节点
                var startTaskNodeEntity = flowTaskNodeEntityList.Find(m => m.NodeType == "start");
                var startApproversProperties = startTaskNodeEntity.NodePropertyJson.Deserialize<StartProperties>();
                flowTaskEntity.IsBatch = startApproversProperties.isBatchApproval ? 1 : 0;
                //表单必填值判断
                if (IsRequiredParameters(startApproversProperties.formOperates, formData.ToObject<Dictionary<string, object>>()))
                    throw HSZException.Oh(ErrorCode.WF0023);
                var nextTaskNodeIdList = startTaskNodeEntity.NodeNext.Split(",");
                if (nextTaskNodeIdList.FirstOrDefault().Equals("end"))
                {
                    flowTaskEntity.Status = FlowTaskStatusEnum.Adopt;
                    flowTaskEntity.Completion = 100;
                    flowTaskEntity.EndTime = DateTime.Now;
                    flowTaskEntity.ThisStepId = "end";
                    flowTaskEntity.ThisStep = "结束";
                    await _flowTaskRepository.UpdateTask(flowTaskEntity);

                    #region 子流程结束回到主流程下一节点
                    if (flowTaskEntity.ParentId != "0" && flowTaskEntity.IsAsync == 0)
                    {
                        await InsertSubFlowNextNode(flowTaskEntity);
                    }
                    #endregion
                }
                else
                {
                    #region 流程经办
                    //任务流程当前节点名
                    var ThisStepList = new List<string>();
                    //任务流程当前完成度
                    var CompletionList = new List<int>();
                    var isAsync = false;
                    foreach (var item in nextTaskNodeIdList)
                    {
                        var nextTaskNodeEntity = flowTaskNodeEntityList.Find(m => m.NodeCode.Equals(item));
                        var approverPropertiers = nextTaskNodeEntity.NodePropertyJson.Deserialize<ApproversProperties>();
                        if (nextTaskNodeEntity.NodeType.Equals("subFlow"))
                        {
                            var childTaskPro = nextTaskNodeEntity.NodePropertyJson.Deserialize<ChildTaskProperties>();
                            var childTaskCrUserList = await GetSubFlowCreator(childTaskPro, flowTaskEntity.CreatorUserId, flowTaskNodeEntityList, nextTaskNodeEntity, formData.ToJson());
                            var childFormData = await GetSubFlowFormData(childTaskPro, formData.ToJson());
                            childTaskPro.childTaskId = await CreateSubProcesses(childTaskPro, childFormData, flowTaskEntity.Id, childTaskCrUserList);
                            childTaskPro.formData = formData.ToJson();
                            nextTaskNodeEntity.NodePropertyJson = childTaskPro.ToJson();
                            //将子流程id保存到主流程的子流程节点属性上
                            nextTaskNodeEntity.Completion = childTaskPro.isAsync ? 1 : 0;
                            await _flowTaskRepository.UpdateTaskNode(nextTaskNodeEntity);
                            await Alerts(childTaskPro.launchMsgConfig, childTaskCrUserList, formData.ToJson());
                            if (childTaskPro.isAsync)
                            {
                                isAsync = true;
                                flowTaskNodeEntityList.Remove(flowTaskNodeEntityList.Find(m => m.NodeCode.Equals(item)));
                                flowTaskNodeEntityList.Add(nextTaskNodeEntity);
                                flowTaskNodeEntityList = flowTaskNodeEntityList.FindAll(x => x.State == "0");
                                await CreateNextFlowTaskOperator(flowTaskNodeEntityList, nextTaskNodeEntity,
                                    nextTaskNodeEntity.NodePropertyJson.Deserialize<ApproversProperties>(), new List<FlowTaskOperatorEntity>(),
                                    1, flowTaskEntity, "", flowTaskOperatorEntityList, formData.ToJson(),
                                    new FlowHandleModel(), flowEngineEntity.FormType.ToInt());
                            }
                        }
                        else
                        {
                            await AddFlowTaskOperatorEntityByAssigneeType(flowTaskOperatorEntityList, flowTaskNodeEntityList, startTaskNodeEntity, nextTaskNodeEntity, flowTaskEntity.CreatorUserId, formData.Serialize(), 0);
                        }
                        ThisStepList.Add(nextTaskNodeEntity.NodeName);
                        CompletionList.Add(approverPropertiers.progress.ToInt());
                    }
                    if (!isAsync)
                    {
                        await _flowTaskRepository.CreateTaskOperator(flowTaskOperatorEntityList);
                    }
                    #endregion

                    #region 更新流程任务
                    if (isAsync)
                    {
                        await _flowTaskRepository.UpdateTask(flowTaskEntity);
                    }
                    else
                    {
                        flowTaskEntity.ThisStepId = startTaskNodeEntity.NodeNext;
                        flowTaskEntity.ThisStep = string.Join(",", ThisStepList);
                        flowTaskEntity.Completion = CompletionList.Min();
                        await _flowTaskRepository.UpdateTask(flowTaskEntity);
                    }
                    #endregion
                }

                #region 流程经办记录
                FlowTaskOperatorRecordEntity flowTaskOperatorRecordEntity = new FlowTaskOperatorRecordEntity();
                flowTaskOperatorRecordEntity.HandleId = _userManager.UserId;
                flowTaskOperatorRecordEntity.HandleTime = DateTime.Now;
                flowTaskOperatorRecordEntity.HandleStatus = 2;
                flowTaskOperatorRecordEntity.NodeName = "开始";
                flowTaskOperatorRecordEntity.TaskId = flowTaskEntity.Id;
                flowTaskOperatorRecordEntity.Status = 0;
                await _flowTaskRepository.CreateTaskOperatorRecord(flowTaskOperatorRecordEntity);
                #endregion

                Db.CommitTran();

                #region 开始事件
                taskId = startTaskNodeEntity.TaskId;
                taskNodeId = startTaskNodeEntity.Id;
                await RequestEvents(startApproversProperties.initFuncConfig, formData.Serialize());
                #endregion



                #region 消息提醒
                //审批消息
                var messageDic = GroupByOperator(flowTaskOperatorEntityList);
                var bodyDic = new Dictionary<string, object>();
                foreach (var item in messageDic.Keys)
                {
                    var userList = messageDic[item].Select(x => x.HandleId).ToList();
                    bodyDic = GetMesBodyText(flowEngineEntity, item, userList, messageDic[item], 2);
                    await StationLetterMsg(flowTaskEntity.FullName, userList, 0, bodyDic);
                    await Alerts(startApproversProperties.waitApproveMsgConfig, userList, formData.ToJson());
                }
                //结束消息
                if (flowTaskEntity.Status == FlowTaskStatusEnum.Adopt)
                {
                    #region 结束事件
                    await RequestEvents(startApproversProperties.endFuncConfig, formData.Serialize());
                    #endregion

                    bodyDic = GetMesBodyText(flowEngineEntity, "", new List<string>() { flowTaskEntity.CreatorUserId }, null, 1);
                    await StationLetterMsg(flowTaskEntity.FullName, new List<string>() { flowTaskEntity.CreatorUserId }, 5, bodyDic);
                    await Alerts(startApproversProperties.endMsgConfig, new List<string>() { flowTaskEntity.CreatorUserId }, formData.ToJson());
                }
                #endregion
                return true;
            }
            catch (Exception ex)
            {
                Db.RollbackTran();
                _logger.LogInformation("提交日志:" + ex.Message + ",错误详情:" + ex.StackTrace);
                return false;

            }
        }

        /// <summary>
        /// 审批(同意)
        /// </summary>
        /// <param name="flowTaskEntity"></param>
        /// <param name="flowTaskOperatorEntity"></param>
        /// <param name="flowHandleModel"></param>
        /// <param name="formType">表单类型</param>
        /// <returns></returns>
        [NonAction]
        public async Task Audit(FlowTaskEntity flowTaskEntity, FlowTaskOperatorEntity flowTaskOperatorEntity, FlowHandleModel flowHandleModel, int formType)
        {
            //流程所有节点
            List<FlowTaskNodeEntity> flowTaskNodeEntityList = (await _flowTaskRepository.GetTaskNodeList(flowTaskEntity.Id)).FindAll(x => x.State == "0");
            var candidates = SaveNodeCandidates(flowTaskNodeEntityList, flowHandleModel.candidateList, flowTaskOperatorEntity.Id);
            try
            {
                Db.BeginTran();
                //当前节点
                FlowTaskNodeEntity flowTaskNodeEntity = flowTaskNodeEntityList.Find(m => m.Id == flowTaskOperatorEntity.TaskNodeId);
                //当前节点属性
                ApproversProperties approversProperties = flowTaskNodeEntity.NodePropertyJson.Deserialize<ApproversProperties>();
                //当前节点所有审批人
                var thisFlowTaskOperatorEntityList = (await _flowTaskRepository.GetTaskOperatorList(flowTaskNodeEntity.TaskId))
                    .FindAll(x => x.TaskNodeId == flowTaskNodeEntity.Id && x.State == "0");
                //下一节点流程经办
                List<FlowTaskOperatorEntity> flowTaskOperatorEntityList = new List<FlowTaskOperatorEntity>();
                //流程抄送
                List<FlowTaskCirculateEntity> flowTaskCirculateEntityList = new List<FlowTaskCirculateEntity>();
                //表单数据
                var formData = formType == 2 ? flowHandleModel.formData.Serialize().Deserialize<JObject>()["data"].ToString() : flowHandleModel.formData.Serialize();
                //表单必填值判断
                if (flowTaskOperatorEntity.DraftData.IsNotEmptyOrNull())
                    formData = flowTaskOperatorEntity.DraftData;
                if (IsRequiredParameters(approversProperties.formOperates, formData.ToObject<Dictionary<string, object>>()))
                    throw HSZException.Oh(ErrorCode.WF0023);

                if (flowTaskOperatorEntity.Id.IsNotEmptyOrNull())
                {
                    #region 更新当前经办数据
                    await UpdateFlowTaskOperator(flowTaskOperatorEntity, thisFlowTaskOperatorEntityList, approversProperties, 1, flowHandleModel.freeApproverUserId, flowTaskEntity.FlowId);
                    #endregion

                    #region 更新当前抄送
                    GetflowTaskCirculateEntityList(approversProperties, flowTaskOperatorEntity, flowTaskCirculateEntityList, flowHandleModel.copyIds);
                    await _flowTaskRepository.CreateTaskCirculate(flowTaskCirculateEntityList);
                    #endregion
                }

                #region 更新经办记录
                await CreateOperatorRecode(flowTaskOperatorEntity, flowHandleModel, 1);
                #endregion

                #region 更新下一节点经办
                //加签审批人
                var freeApproverOperatorEntity = new FlowTaskOperatorEntity();
                if (flowHandleModel.freeApproverUserId.IsNotEmptyOrNull())
                {
                    //加签审批人
                    freeApproverOperatorEntity.Id = YitIdHelper.NextId().ToString();
                    freeApproverOperatorEntity.ParentId = flowTaskOperatorEntity.Id;
                    freeApproverOperatorEntity.HandleType = "6";
                    freeApproverOperatorEntity.HandleId = flowHandleModel.freeApproverUserId;
                    freeApproverOperatorEntity.NodeCode = flowTaskOperatorEntity.NodeCode;
                    freeApproverOperatorEntity.NodeName = flowTaskOperatorEntity.NodeName;
                    freeApproverOperatorEntity.Description = flowTaskOperatorEntity.Description;
                    freeApproverOperatorEntity.CreatorTime = DateTime.Now;
                    freeApproverOperatorEntity.TaskNodeId = flowTaskOperatorEntity.TaskNodeId;
                    freeApproverOperatorEntity.TaskId = flowTaskOperatorEntity.TaskId;
                    freeApproverOperatorEntity.Type = flowTaskOperatorEntity.Type;
                    freeApproverOperatorEntity.State = flowTaskOperatorEntity.State;
                    freeApproverOperatorEntity.Completion = 0;
                    await _flowTaskRepository.CreateTaskOperator(freeApproverOperatorEntity);
                    //当前审批人state改为1
                    flowTaskOperatorEntity.State = "1";
                    await _flowTaskRepository.UpdateTaskOperator(flowTaskOperatorEntity);

                    #region 流转记录
                    var flowTaskOperatorRecordEntity = new FlowTaskOperatorRecordEntity();
                    flowTaskOperatorRecordEntity.HandleOpinion = flowHandleModel.handleOpinion;
                    flowTaskOperatorRecordEntity.HandleId = _userManager.UserId;
                    flowTaskOperatorRecordEntity.HandleTime = DateTime.Now;
                    flowTaskOperatorRecordEntity.HandleStatus = 6;
                    flowTaskOperatorRecordEntity.NodeName = flowTaskOperatorEntity.NodeName;
                    flowTaskOperatorRecordEntity.TaskId = flowTaskOperatorEntity.TaskId;
                    flowTaskOperatorRecordEntity.TaskNodeId = flowTaskOperatorEntity.TaskNodeId;
                    flowTaskOperatorRecordEntity.TaskOperatorId = flowTaskOperatorEntity.Id;
                    flowTaskOperatorRecordEntity.Status = 0;
                    flowTaskOperatorRecordEntity.OperatorId = flowHandleModel.freeApproverUserId;
                    await _flowTaskRepository.CreateTaskOperatorRecord(flowTaskOperatorRecordEntity);
                    #endregion
                }
                else
                {
                    await CreateNextFlowTaskOperator(flowTaskNodeEntityList, flowTaskNodeEntity, approversProperties,
                                            thisFlowTaskOperatorEntityList, 1, flowTaskEntity, flowHandleModel.freeApproverUserId,
                                            flowTaskOperatorEntityList, formData, flowHandleModel, formType);
                }
                #endregion

                #region 更新节点
                await _flowTaskRepository.UpdateTaskNode(flowTaskNodeEntity);
                #endregion

                #region 更新任务
                taskId = flowTaskNodeEntity.TaskId;
                taskNodeId = flowTaskNodeEntity.Id;
                if (flowTaskNodeEntity.Completion > 0)
                {

                    if (flowTaskEntity.Status == FlowTaskStatusEnum.Adopt)
                    {
                        #region 子流程结束回到主流程下一节点
                        if (flowTaskEntity.ParentId != "0" && flowTaskEntity.IsAsync == 0)
                        {
                            await InsertSubFlowNextNode(flowTaskEntity);
                        }
                        #endregion
                    }
                    await _flowTaskRepository.UpdateTask(flowTaskEntity);
                }
                #endregion

                Db.CommitTran();

                #region 消息与事件
                var startApproversProperties = flowTaskNodeEntityList.Find(x => x.NodeType.Equals("start")).NodePropertyJson.Deserialize<StartProperties>();
                if (flowTaskNodeEntity.Completion > 0)
                {
                    #region 审批事件
                    await RequestEvents(approversProperties.approveFuncConfig, formData);
                    #endregion

                    #region 消息提醒
                    var flowEngineEntity = await _flowEngineService.GetInfo(flowTaskEntity.FlowId);
                    var bodyDic = new Dictionary<string, object>();
                    var messageDic = GroupByOperator(flowTaskOperatorEntityList);
                    //审批
                    foreach (var item in messageDic.Keys)
                    {
                        var userList = messageDic[item].Select(x => x.HandleId).ToList();
                        bodyDic = GetMesBodyText(flowEngineEntity, item, userList, messageDic[item], 2);
                        await StationLetterMsg(flowTaskEntity.FullName, userList, 0, bodyDic);
                        await StationLetterMsg(flowTaskEntity.FullName, userList, 1, bodyDic);
                        await Alerts(startApproversProperties.waitApproveMsgConfig, userList, formData);
                        approversProperties.approveMsgConfig = approversProperties.approveMsgConfig.on == 2 ? startApproversProperties.approveMsgConfig : approversProperties.approveMsgConfig;
                        await Alerts(approversProperties.approveMsgConfig, userList, formData);
                    }
                    //抄送
                    var userIdList = flowTaskCirculateEntityList.Select(x => x.ObjectId).ToList();
                    bodyDic = GetMesBodyText(flowEngineEntity, flowTaskNodeEntity.Id, userIdList, null, 3, flowTaskOperatorEntity.Id);
                    await StationLetterMsg(flowTaskEntity.FullName, userIdList, 3, bodyDic);
                    approversProperties.copyMsgConfig = approversProperties.copyMsgConfig.on == 2 ? startApproversProperties.copyMsgConfig : approversProperties.copyMsgConfig;
                    await Alerts(approversProperties.copyMsgConfig, userIdList, formData);
                    //加签
                    if (flowHandleModel.freeApproverUserId.IsNotEmptyOrNull())
                    {
                        bodyDic = GetMesBodyText(flowEngineEntity, freeApproverOperatorEntity.TaskNodeId, new List<string>() { flowHandleModel.freeApproverUserId }, new List<FlowTaskOperatorEntity>() { freeApproverOperatorEntity }, 2);
                        await StationLetterMsg(flowTaskEntity.FullName, new List<string>() { flowHandleModel.freeApproverUserId }, 0, bodyDic);
                        await Alerts(startApproversProperties.waitApproveMsgConfig, new List<string>() { flowHandleModel.freeApproverUserId }, formData);
                    }
                    #endregion

                    if (flowTaskEntity.Status == FlowTaskStatusEnum.Adopt)
                    {
                        #region 结束事件
                        await RequestEvents(startApproversProperties.endFuncConfig, formData);
                        #endregion
                        //结束
                        bodyDic = GetMesBodyText(flowEngineEntity, flowTaskNodeEntity.Id, new List<string>() { flowTaskEntity.CreatorUserId }, null, 1);
                        await StationLetterMsg(flowTaskEntity.FullName, new List<string>() { flowTaskEntity.CreatorUserId }, 5, bodyDic);
                        await Alerts(startApproversProperties.endMsgConfig, new List<string>() { flowTaskEntity.CreatorUserId }, formData);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                _flowTaskRepository.DeleteFlowCandidates(candidates.Select(x => x.Id).ToArray());
                Db.RollbackTran();
                throw HSZException.Oh(ex.Message);
            }


        }

        /// <summary>
        /// 审批(拒绝)
        /// </summary>
        /// <param name="flowTaskEntity"></param>
        /// <param name="flowTaskOperatorEntity"></param>
        /// <param name="flowHandleModel"></param>
        /// <param name="formType"></param>
        /// <returns></returns>
        [NonAction]
        public async Task Reject(FlowTaskEntity flowTaskEntity, FlowTaskOperatorEntity flowTaskOperatorEntity, FlowHandleModel flowHandleModel, int formType)
        {
            try
            {
                Db.BeginTran();
                //流程所有节点
                List<FlowTaskNodeEntity> flowTaskNodeEntityList = (await _flowTaskRepository.GetTaskNodeList(flowTaskEntity.Id)).FindAll(x => x.State == "0");
                //当前节点
                FlowTaskNodeEntity flowTaskNodeEntity = flowTaskNodeEntityList.Find(m => m.Id == flowTaskOperatorEntity.TaskNodeId);
                //当前节点属性
                ApproversProperties approversProperties = flowTaskNodeEntity.NodePropertyJson.Deserialize<ApproversProperties>();
                //当前节点所有审批人
                var thisFlowTaskOperatorEntityList = (await _flowTaskRepository.GetTaskOperatorList(flowTaskNodeEntity.TaskId)).FindAll(x => x.TaskNodeId == flowTaskNodeEntity.Id && x.State == "0");
                //表单数据
                var formData = formType == 2 ? (flowHandleModel.formData.Serialize().Deserialize<JObject>())["data"].ToString() : flowHandleModel.formData.Serialize();
                //驳回节点流程经办
                List<FlowTaskOperatorEntity> flowTaskOperatorEntityList = new List<FlowTaskOperatorEntity>();
                #region 更新当前经办数据
                await UpdateFlowTaskOperator(flowTaskOperatorEntity, thisFlowTaskOperatorEntityList, approversProperties, 0, flowHandleModel.freeApproverUserId, flowTaskEntity.FlowId);
                #endregion

                #region 自定义抄送
                var flowTaskCirculateEntityList = new List<FlowTaskCirculateEntity>();
                GetflowTaskCirculateEntityList(approversProperties, flowTaskOperatorEntity, flowTaskCirculateEntityList, flowHandleModel.copyIds, 0);
                await _flowTaskRepository.CreateTaskCirculate(flowTaskCirculateEntityList);
                #endregion

                #region 更新驳回经办
                await CreateNextFlowTaskOperator(flowTaskNodeEntityList, flowTaskNodeEntity, approversProperties,
                    thisFlowTaskOperatorEntityList, 0, flowTaskEntity, flowHandleModel.freeApproverUserId,
                    flowTaskOperatorEntityList, formData, flowHandleModel, formType);
                #endregion

                #region 更新流程任务
                if (flowTaskEntity.Status == FlowTaskStatusEnum.Reject)
                {
                    await _flowTaskRepository.UpdateTask(flowTaskEntity);
                    await _flowTaskRepository.DeleteFlowTaskAllData(flowTaskEntity.Id);
                }
                else
                {
                    await _flowTaskRepository.UpdateTask(flowTaskEntity);
                    await _flowTaskRepository.CreateTaskOperator(flowTaskOperatorEntityList);
                }
                #endregion

                #region 更新经办记录
                await CreateOperatorRecode(flowTaskOperatorEntity, flowHandleModel, 0);
                #endregion

                Db.CommitTran();

                #region 消息与事件
                taskId = flowTaskNodeEntity.TaskId;
                taskNodeId = flowTaskNodeEntity.Id;
                await RequestEvents(approversProperties.rejectFuncConfig, formData);

                var startApproversProperties = flowTaskNodeEntityList.Find(x => x.NodeType.Equals("start")).NodePropertyJson.Deserialize<StartProperties>();
                if (flowTaskOperatorEntityList.Count > 0)
                {
                    #region 审批事件
                    await RequestEvents(approversProperties.approveFuncConfig, formData);
                    #endregion

                    #region 消息提醒
                    var flowEngineEntity = await _flowEngineService.GetInfo(flowTaskEntity.FlowId);
                    var bodyDic = new Dictionary<string, object>();
                    var messageDic = GroupByOperator(flowTaskOperatorEntityList);
                    //审批
                    foreach (var item in messageDic.Keys)
                    {
                        var userList = messageDic[item].Select(x => x.HandleId).ToList();
                        bodyDic = GetMesBodyText(flowEngineEntity, item, userList, messageDic[item], 2);
                        await StationLetterMsg(flowTaskEntity.FullName, userList, 0, bodyDic);
                        await StationLetterMsg(flowTaskEntity.FullName, userList, 2, bodyDic);
                        await Alerts(startApproversProperties.waitApproveMsgConfig, userList, formData);
                        approversProperties.rejectMsgConfig = approversProperties.rejectMsgConfig.on == 2 ? startApproversProperties.rejectMsgConfig : approversProperties.rejectMsgConfig;
                        await Alerts(approversProperties.rejectMsgConfig, userList, formData);
                    }
                    //抄送
                    var userIdList = flowTaskCirculateEntityList.Select(x => x.ObjectId).ToList();
                    bodyDic = GetMesBodyText(flowEngineEntity, flowTaskNodeEntity.Id, userIdList, null, 3, flowTaskOperatorEntity.Id);
                    await StationLetterMsg(flowTaskEntity.FullName, userIdList, 3, bodyDic);
                    approversProperties.copyMsgConfig = approversProperties.copyMsgConfig.on == 2 ? startApproversProperties.copyMsgConfig : approversProperties.copyMsgConfig;
                    await Alerts(approversProperties.copyMsgConfig, userIdList, formData);
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                Db.RollbackTran();
            }
        }

        /// <summary>
        /// 审批(撤回)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        [NonAction]
        public async Task Recall(string id, FlowHandleModel flowHandleModel)
        {
            try
            {
                Db.BeginTran();
                //撤回经办记录
                var flowTaskOperatorRecordEntity = await _flowTaskRepository.GetTaskOperatorRecordInfo(id);
                //撤回经办
                var flowTaskOperatorEntity = await _flowTaskRepository.GetTaskOperatorInfo(flowTaskOperatorRecordEntity.TaskOperatorId);
                //撤回节点
                var flowTaskNodeEntity = await _flowTaskRepository.GetTaskNodeInfo(flowTaskOperatorRecordEntity.TaskNodeId);
                //撤回任务
                var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(flowTaskOperatorRecordEntity.TaskId);
                //所有节点
                var flowTaskNodeEntityList = (await _flowTaskRepository.GetTaskNodeList(flowTaskOperatorRecordEntity.TaskId)).FindAll(x => x.State == "0");
                //所有经办
                var flowTaskOperatorEntityList = (await _flowTaskRepository.GetTaskOperatorList(flowTaskOperatorRecordEntity.TaskId)).FindAll(x => x.State == "0");
                //撤回节点属性
                var recallNodeProperties = flowTaskNodeEntity.NodePropertyJson.Deserialize<ApproversProperties>();
                #region 撤回判断
                //拒绝不撤回
                if (flowTaskOperatorEntity.HandleStatus == 0)
                    throw HSZException.Oh(ErrorCode.WF0010);
                //任务待审状态才能撤回
                if (!(flowTaskEntity.EnabledMark == 1 && flowTaskEntity.Status == 1))
                    throw HSZException.Oh(ErrorCode.WF0011);
                //撤回节点下一节点已操作
                var recallNextOperatorList = flowTaskOperatorEntityList.FindAll(x => flowTaskNodeEntity.NodeNext.Contains(x.NodeCode));
                if (recallNextOperatorList.FindAll(x => x.Completion == 1 && x.HandleStatus == 1).Count > 0)
                    throw HSZException.Oh(ErrorCode.WF0011);
                #endregion

                #region 经办修改
                var delOperatorRecordIds = new List<string>();
                //加签人
                var upOperatorList = await GetOperator(flowTaskOperatorEntity.Id, new List<FlowTaskOperatorEntity>());

                flowTaskOperatorEntity.HandleStatus = null;
                flowTaskOperatorEntity.HandleTime = null;
                flowTaskOperatorEntity.Completion = 0;
                flowTaskOperatorEntity.State = "0";
                upOperatorList.Add(flowTaskOperatorEntity);

                foreach (var item in upOperatorList)
                {
                    var operatorRecord = await _flowTaskRepository.GetTaskOperatorRecordInfo(item.TaskId, item.TaskNodeId, item.Id);
                    if (operatorRecord.IsNotEmptyOrNull())
                    {
                        delOperatorRecordIds.Add(operatorRecord.Id);
                    }
                }
                //撤回节点是否完成
                if (flowTaskNodeEntity.Completion == 1)
                {
                    //撤回节点下一节点经办删除
                    await _flowTaskRepository.DeleteTaskOperator(recallNextOperatorList.Select(x => x.Id).ToList());
                    //或签经办全部撤回，会签撤回未处理的经办
                    //撤回节点未审批的经办
                    var notHanleOperatorList = flowTaskOperatorEntityList.FindAll(x => x.TaskNodeId == flowTaskOperatorRecordEntity.TaskNodeId && x.HandleStatus == null
                     && x.HandleTime == null);
                    foreach (var item in notHanleOperatorList)
                    {
                        item.Completion = 0;
                    }
                    upOperatorList = upOperatorList.Union(notHanleOperatorList).ToList();

                    #region 更新撤回节点
                    flowTaskNodeEntity.Completion = 0;
                    await _flowTaskRepository.UpdateTaskNode(flowTaskNodeEntity);
                    var thisNodeEntityList = flowTaskNodeEntityList.FindAll(x => flowTaskNodeEntity.NodeNext.Contains(x.NodeCode));
                    foreach (var item in thisNodeEntityList)
                    {
                        _flowTaskRepository.DeleteFlowCandidates(item.Id, _userManager.UserId, flowTaskOperatorEntity.Id);
                    }
                    #endregion

                    #region 更新任务流程
                    flowTaskEntity.ThisStepId = GetRecallThisStepId(flowTaskNodeEntityList, flowTaskNodeEntity, flowTaskEntity.ThisStepId);
                    flowTaskEntity.ThisStep = GetThisStep(flowTaskNodeEntityList, flowTaskEntity.ThisStepId);
                    flowTaskEntity.Completion = flowTaskNodeEntity.NodePropertyJson.Deserialize<ApproversProperties>().progress.ToInt();
                    flowTaskEntity.Status = FlowTaskStatusEnum.Handle;
                    await _flowTaskRepository.UpdateTask(flowTaskEntity);
                    #endregion
                }
                await _flowTaskRepository.UpdateTaskOperator(upOperatorList);
                #endregion


                #region 删除经办记录
                delOperatorRecordIds.Add(flowTaskOperatorRecordEntity.Id);
                await _flowTaskRepository.DeleteTaskOperatorRecord(delOperatorRecordIds);
                #endregion

                #region 撤回记录
                flowTaskOperatorRecordEntity.HandleId = _userManager.UserId;
                flowTaskOperatorRecordEntity.HandleOpinion = flowHandleModel.handleOpinion;
                flowTaskOperatorRecordEntity.HandleTime = DateTime.Now;
                flowTaskOperatorRecordEntity.HandleStatus = 3;
                flowTaskOperatorRecordEntity.NodeName = flowTaskNodeEntity.NodeName;
                flowTaskOperatorRecordEntity.TaskId = flowTaskEntity.Id;
                flowTaskOperatorRecordEntity.TaskNodeId = flowTaskOperatorRecordEntity.TaskNodeId;
                flowTaskOperatorRecordEntity.TaskOperatorId = flowTaskOperatorRecordEntity.Id;
                flowTaskOperatorRecordEntity.Status = 0;
                await _flowTaskRepository.CreateTaskOperatorRecord(flowTaskOperatorRecordEntity);
                #endregion

                Db.CommitTran();
                #region 撤回事件
                taskId = flowTaskNodeEntity.TaskId;
                taskNodeId = flowTaskNodeEntity.Id;
                await RequestEvents(recallNodeProperties.recallFuncConfig, flowTaskEntity.FlowFormContentJson);
                #endregion
            }
            catch (Exception ex)
            {
                Db.RollbackTran();
                throw HSZException.Oh(ErrorCode.WF0005);
            }
        }

        /// <summary>
        /// 流程撤回
        /// </summary>
        /// <param name="flowTaskEntity">流程实例</param>
        /// <param name="flowHandleModel">流程经办</param>
        [NonAction]
        public async Task Revoke(FlowTaskEntity flowTaskEntity, string flowHandleModel)
        {
            try
            {
                Db.BeginTran();
                var starProperty = (await _flowTaskRepository.GetTaskNodeList(flowTaskEntity.Id))
                    .Find(x => x.NodeType == "start" && x.State == "0").NodePropertyJson.Deserialize<StartProperties>();
                #region 撤回数据
                await _flowTaskRepository.DeleteFlowTaskAllData(flowTaskEntity.Id);
                #endregion

                #region 更新实例
                flowTaskEntity.ThisStepId = "";
                flowTaskEntity.ThisStep = "开始";
                flowTaskEntity.Completion = 0;
                flowTaskEntity.Status = FlowTaskStatusEnum.Revoke;
                flowTaskEntity.StartTime = null;
                flowTaskEntity.EndTime = null;
                await _flowTaskRepository.UpdateTask(flowTaskEntity);
                #endregion

                #region 撤回记录
                FlowTaskOperatorRecordEntity flowTaskOperatorRecordEntity = new FlowTaskOperatorRecordEntity();
                flowTaskOperatorRecordEntity.HandleOpinion = flowHandleModel;
                flowTaskOperatorRecordEntity.HandleId = _userManager.UserId;
                flowTaskOperatorRecordEntity.HandleTime = DateTime.Now;
                flowTaskOperatorRecordEntity.HandleStatus = 3;
                flowTaskOperatorRecordEntity.NodeName = "开始";
                flowTaskOperatorRecordEntity.TaskId = flowTaskEntity.Id;
                flowTaskOperatorRecordEntity.Status = 0;
                await _flowTaskRepository.CreateTaskOperatorRecord(flowTaskOperatorRecordEntity);
                #endregion

                #region 撤回子流程任务
                var childTask = (await _flowTaskRepository.GetTaskList()).FindAll(x => flowTaskEntity.Id == x.ParentId);
                foreach (var item in childTask)
                {
                    if (item.Status == 1)
                    {
                        await this.Revoke(item, flowHandleModel);
                    }
                    await _flowTaskRepository.DeleteTask(item);
                }
                #endregion

                Db.CommitTran();

                #region 撤回事件
                taskId = flowTaskEntity.Id;
                taskNodeId = "";
                await RequestEvents(starProperty.flowRecallFuncConfig, flowTaskEntity.FlowFormContentJson);
                #endregion
            }
            catch (Exception ex)
            {
                Db.RollbackTran();
            }
        }

        /// <summary>
        /// 终止
        /// </summary>
        /// <param name="flowTaskEntity">流程实例</param>
        /// <param name="flowHandleModel">流程经办</param>
        [NonAction]
        public async Task Cancel(FlowTaskEntity flowTaskEntity, FlowHandleModel flowHandleModel)
        {
            try
            {
                Db.BeginTran();
                #region 更新实例
                flowTaskEntity.Status = FlowTaskStatusEnum.Cancel;
                flowTaskEntity.EndTime = DateTime.Now;
                await _flowTaskRepository.UpdateTask(flowTaskEntity);
                #endregion

                #region 作废记录
                FlowTaskOperatorRecordEntity flowTaskOperatorRecordEntity = new FlowTaskOperatorRecordEntity();
                flowTaskOperatorRecordEntity.HandleOpinion = flowHandleModel.handleOpinion;
                flowTaskOperatorRecordEntity.HandleId = _userManager.UserId;
                flowTaskOperatorRecordEntity.HandleTime = DateTime.Now;
                flowTaskOperatorRecordEntity.HandleStatus = 4;
                flowTaskOperatorRecordEntity.NodeName = flowTaskEntity.ThisStep;
                flowTaskOperatorRecordEntity.TaskId = flowTaskEntity.Id;
                flowTaskOperatorRecordEntity.Status = 0;
                await _flowTaskRepository.CreateTaskOperatorRecord(flowTaskOperatorRecordEntity);
                #endregion
                Db.CommitTran();

            }
            catch (Exception ex)
            {
                Db.RollbackTran();
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 指派
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        [NonAction]
        public async Task Assigned(string id, FlowHandleModel flowHandleModel)
        {
            try
            {
                Db.BeginTran();
                var flowOperatorEntityList = (await _flowTaskRepository.GetTaskOperatorList(id)).FindAll(x => x.State == "0" && x.NodeCode == flowHandleModel.nodeCode);
                await _flowTaskRepository.DeleteTaskOperator(flowOperatorEntityList.Select(x => x.Id).ToList());
                var entity = new FlowTaskOperatorEntity()
                {
                    Id = YitIdHelper.NextId().ToString(),
                    HandleId = flowHandleModel.freeApproverUserId,
                    HandleType = flowOperatorEntityList.FirstOrDefault().HandleType,
                    NodeCode = flowOperatorEntityList.FirstOrDefault().NodeCode,
                    NodeName = flowOperatorEntityList.FirstOrDefault().NodeName,
                    CreatorTime = DateTime.Now,
                    TaskId = flowOperatorEntityList.FirstOrDefault().TaskId,
                    TaskNodeId = flowOperatorEntityList.FirstOrDefault().TaskNodeId,
                    Type = flowOperatorEntityList.FirstOrDefault().Type,
                    Completion = 0,
                    State = "0"
                };
                var isOk = await _flowTaskRepository.CreateTaskOperator(entity);
                if (isOk < 1)
                    throw HSZException.Oh(ErrorCode.WF0008);

                #region 流转记录
                var flowTaskOperatorRecordEntity = new FlowTaskOperatorRecordEntity();
                flowTaskOperatorRecordEntity.HandleOpinion = flowHandleModel.handleOpinion;
                flowTaskOperatorRecordEntity.HandleId = _userManager.UserId;
                flowTaskOperatorRecordEntity.HandleTime = DateTime.Now;
                flowTaskOperatorRecordEntity.HandleStatus = 5;
                flowTaskOperatorRecordEntity.NodeName = entity.NodeName;
                flowTaskOperatorRecordEntity.TaskId = entity.TaskId;
                flowTaskOperatorRecordEntity.Status = 0;
                flowTaskOperatorRecordEntity.OperatorId = flowHandleModel.freeApproverUserId;
                await _flowTaskRepository.CreateTaskOperatorRecord(flowTaskOperatorRecordEntity);
                #endregion
                Db.CommitTran();

                var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(entity.TaskId);
                var flowEngineEntity = await _flowEngineService.GetInfo(flowTaskEntity.FlowId);
                var startApproversProperties = (await _flowTaskRepository.GetTaskNodeList(entity.TaskId)).Find(x => x.NodeType.Equals("start")).NodePropertyJson.ToObject<StartProperties>();
                var bodyDic = GetMesBodyText(flowEngineEntity, entity.TaskNodeId, new List<string>() { flowHandleModel.freeApproverUserId }, new List<FlowTaskOperatorEntity>() { entity }, 2);
                await StationLetterMsg(flowTaskEntity.FullName, new List<string>() { flowHandleModel.freeApproverUserId }, 0, bodyDic);
                await Alerts(startApproversProperties.waitApproveMsgConfig, new List<string>() { flowHandleModel.freeApproverUserId }, flowTaskEntity.FlowFormContentJson);
            }
            catch (Exception ex)
            {
                Db.RollbackTran();
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 转办
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        [NonAction]
        public async Task Transfer(string id, FlowHandleModel flowHandleModel)
        {
            try
            {
                Db.BeginTran();
                var flowOperatorEntity = await _flowTaskRepository.GetTaskOperatorInfo(id);
                if (flowOperatorEntity == null)
                    throw HSZException.Oh(ErrorCode.COM1005);
                flowOperatorEntity.HandleId = flowHandleModel.freeApproverUserId;
                var isOk = await _flowTaskRepository.UpdateTaskOperator(flowOperatorEntity);
                if (isOk < 1)
                    throw HSZException.Oh(ErrorCode.WF0007);

                #region 流转记录
                var flowTaskOperatorRecordEntity = new FlowTaskOperatorRecordEntity();
                flowTaskOperatorRecordEntity.HandleOpinion = flowHandleModel.handleOpinion;
                flowTaskOperatorRecordEntity.HandleId = _userManager.UserId;
                flowTaskOperatorRecordEntity.HandleTime = DateTime.Now;
                flowTaskOperatorRecordEntity.HandleStatus = 7;
                flowTaskOperatorRecordEntity.NodeName = flowOperatorEntity.NodeName;
                flowTaskOperatorRecordEntity.TaskId = flowOperatorEntity.TaskId;
                flowTaskOperatorRecordEntity.Status = 0;
                flowTaskOperatorRecordEntity.OperatorId = flowHandleModel.freeApproverUserId;
                await _flowTaskRepository.CreateTaskOperatorRecord(flowTaskOperatorRecordEntity);
                #endregion
                Db.CommitTran();

                var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(flowOperatorEntity.TaskId);
                var flowEngineEntity = await _flowEngineService.GetInfo(flowTaskEntity.FlowId);
                var startApproversProperties = (await _flowTaskRepository.GetTaskNodeList(flowOperatorEntity.TaskId)).Find(x => x.NodeType.Equals("start")).NodePropertyJson.ToObject<StartProperties>();
                var bodyDic = GetMesBodyText(flowEngineEntity, flowOperatorEntity.TaskNodeId, new List<string>() { flowHandleModel.freeApproverUserId }, new List<FlowTaskOperatorEntity>() { flowOperatorEntity }, 2);
                await StationLetterMsg(flowTaskEntity.FullName, new List<string>() { flowHandleModel.freeApproverUserId }, 0, bodyDic);
                await Alerts(startApproversProperties.waitApproveMsgConfig, new List<string>() { flowHandleModel.freeApproverUserId }, flowTaskEntity.FlowFormContentJson);
            }
            catch (Exception ex)
            {
                Db.RollbackTran();
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 催办
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task Press(string id)
        {
            try
            {
                Db.BeginTran();
                var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(id);
                var flowTaskOperatorEntityList = (await _flowTaskRepository.GetTaskOperatorList(id)).FindAll(x => x.Completion == 0 && x.State == "0");
                var processId = flowTaskOperatorEntityList.Select(x => x.HandleId).ToList();
                if (processId.Count == 0)
                    throw HSZException.Oh(ErrorCode.WF0009);
                Db.CommitTran();
                var flowEngineEntity = await _flowEngineService.GetInfo(flowTaskEntity.FlowId);
                var bodyDic = new Dictionary<string, object>();
                var messageDic = GroupByOperator(flowTaskOperatorEntityList);
                var startApproversProperties = (await _flowTaskRepository.GetTaskNodeList(flowTaskEntity.Id)).Find(x => x.NodeType.Equals("start")).NodePropertyJson.ToObject<StartProperties>();
                foreach (var item in messageDic.Keys)
                {
                    var node = await _flowTaskRepository.GetTaskNodeInfo(item);
                    var nodeProperties = node.NodePropertyJson.Deserialize<ApproversProperties>();
                    var userList = messageDic[item].Select(x => x.HandleId).ToList();
                    bodyDic = GetMesBodyText(flowEngineEntity, node.Id, userList, messageDic[item], 2);
                    await StationLetterMsg(flowTaskEntity.FullName, userList, 0, bodyDic);
                    await Alerts(startApproversProperties.waitApproveMsgConfig, userList, flowTaskEntity.FlowFormContentJson);
                }
            }
            catch (Exception ex)
            {
                Db.RollbackTran();
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 审批事前操作
        /// </summary>
        /// <param name="flowEngineEntity"></param>
        /// <param name="flowTaskEntity"></param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        [NonAction]
        public async Task ApproveBefore(FlowEngineEntity flowEngineEntity, FlowTaskEntity flowTaskEntity, FlowHandleModel flowHandleModel)
        {
            try
            {
                if (flowEngineEntity.FormType == 2)
                {
                    var data = (flowHandleModel.formData.Serialize().Deserialize<JObject>())["data"].ToString().Deserialize<JObject>();
                    var devData = (flowHandleModel.formData.Serialize().Deserialize<JObject>())["data"].ToString();
                    var devEntity = await _visualDevServce.GetInfoById(flowEngineEntity.Id);
                    var upInput = new VisualDevModelDataUpInput() { id = flowTaskEntity.Id, data = devData, status = 1 };
                    await Save(flowTaskEntity.Id, flowTaskEntity.FlowId, flowTaskEntity.ProcessId, flowTaskEntity.FullName, flowTaskEntity.FlowUrgent, flowTaskEntity.EnCode, data, 1, 1, false, "0", null, devEntity.IsNotEmptyOrNull());
                    if (devEntity.IsNotEmptyOrNull())
                    {
                        await _runService.Update(flowTaskEntity.Id, devEntity, upInput);
                    }
                }
                else
                {
                    flowTaskEntity.FlowFormContentJson = flowHandleModel.formData.ToJson();
                    await _flowTaskRepository.UpdateTask(flowTaskEntity);
                    GetSysTableFromService(flowHandleModel.enCode, flowHandleModel.formData, flowTaskEntity.Id, 0);
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 获取候选人
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flowHandleModel"></param>
        /// <param name="type">0:候选节点编码，1：候选人</param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetCandidateModelList(string id, FlowHandleModel flowHandleModel, int type = 0)
        {
            var output = new List<FlowTaskCandidateModel>();
            //所有节点
            List<FlowTaskNodeEntity> flowTaskNodeEntityList = new List<FlowTaskNodeEntity>();
            //下个节点集合
            List<FlowTaskNodeEntity> nextNodeEntityList = new List<FlowTaskNodeEntity>();
            //指定下个节点
            FlowTaskNodeEntity nextNodeEntity = new FlowTaskNodeEntity();
            var jobj = flowHandleModel.formData.Serialize().Deserialize<JObject>();
            if (!jobj.ContainsKey("flowId"))
            {
                return output;
            }
            var flowId = jobj["flowId"].ToString();
            var flowEngineEntity = await _flowEngineService.GetInfo(flowId);
            var formData = flowEngineEntity.FormType == 2 ? jobj["data"].ToString().ToObject() : flowHandleModel.formData;
            if (id == "0")
            {
                //所有节点
                flowTaskNodeEntityList = ParsingTemplateGetNodeList(flowEngineEntity, formData.Serialize(), "");
                var startTaskNodeEntity = flowTaskNodeEntityList.Find(m => m.NodeType == "start");
                nextNodeEntityList = flowTaskNodeEntityList.FindAll(m => startTaskNodeEntity.NodeNext.Contains(m.NodeCode));
            }
            else
            {
                var flowTaskOperator = await _flowTaskRepository.GetTaskOperatorInfo(id);
                if (flowTaskOperator.ParentId.IsNotEmptyOrNull() && type == 0)
                {
                    return output;
                }
                var flowTaskNodeEntity = await _flowTaskRepository.GetTaskNodeInfo(flowTaskOperator.TaskNodeId);
                flowTaskNodeEntityList = (await _flowTaskRepository.GetTaskNodeList(flowTaskOperator.TaskId)).FindAll(x => x.State == "0");
                nextNodeEntityList = flowTaskNodeEntityList.FindAll(m => flowTaskNodeEntity.NodeNext.Contains(m.NodeCode));
            }
            nextNodeEntity = flowTaskNodeEntityList.Find(x => x.NodeCode.Equals(flowHandleModel.nodeCode));
            if (type == 1)
            {
                return await GetCandidateItems(nextNodeEntity, flowHandleModel);
            }
            await GetCandidates(output, nextNodeEntityList, flowTaskNodeEntityList);
            return output;
        }

        /// <summary>
        /// 批量审批节点列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> NodeSelector(string flowId)
        {
            var flowEngineEntity = await _flowEngineService.GetInfo(flowId);
            var taskNodeList = ParsingTemplateGetNodeList(flowEngineEntity, null, "");
            var nodeList = taskNodeList.FindAll(x => x.NodeType.Equals("approver")).Select(x => new { id = x.NodeCode, fullName = x.NodePropertyJson.Deserialize<ApproversProperties>().title }).ToList();
            return nodeList;
        }

        /// <summary>
        /// 获取批量审批候选人
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="flowTaskOperatorId"></param>
        /// <returns></returns>
        public async Task<dynamic> GetBatchCandidate(string flowId, string flowTaskOperatorId)
        {
            //所有节点
            var flowEngineEntity = await _flowEngineService.GetInfo(flowId);
            var taskNodeList = ParsingTemplateGetNodeList(flowEngineEntity, null, "");
            var flowTaskOperator = await _flowTaskRepository.GetTaskOperatorInfo(flowTaskOperatorId);
            var node = await _flowTaskRepository.GetTaskNodeInfo(flowTaskOperator.TaskNodeId);
            var ids = node.NodeNext.Split(",").ToList();
            var nodeList = taskNodeList.FindAll(x => x.NodeType.Equals("condition") && ids.Intersect(x.NodeNext.Split(",").ToList()).ToList().Count > 0);
            var flag = taskNodeList.FindAll(x => ids.Contains(x.NodeCode)).Where(x => x.NodePropertyJson.Deserialize<ApproversProperties>().assigneeType == 7).Count() > 0;
            if (nodeList.Count > 0 && flag)
            {
                throw HSZException.Oh(ErrorCode.WF0022);
            }
            var model = new FlowHandleModel
            {
                nodeCode = flowTaskOperator.NodeCode,
                formData = new { flowId = flowId, data = "{}", id = flowTaskOperator.TaskId }
            };
            return await GetCandidateModelList(flowTaskOperatorId, model);
        }

        /// <summary>
        /// 审批根据条件变更节点
        /// </summary>
        /// <param name="flowEngineEntity"></param>
        /// <param name="formData"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task AdjustNodeByCon(FlowEngineEntity flowEngineEntity, object formData, string taskId)
        {
            var data = flowEngineEntity.FormType == 2 ? formData.Serialize().Deserialize<JObject>()["data"].ToString() : formData.Serialize();
            var nodeList = await _flowTaskRepository.GetTaskNodeList(taskId);
            var newNodeList = ParsingTemplateGetNodeList(flowEngineEntity, data, taskId);
            foreach (var item in nodeList)
            {
                var node = newNodeList.FirstOrDefault(x => x.NodeCode == item.NodeCode);
                item.SortCode = node.SortCode;
                item.State = node.State;
            }
            await _flowTaskRepository.UpdateTaskNode(nodeList);
        }

        /// <summary>
        /// 判断驳回节点是否存在子流程
        /// </summary>
        /// <param name="flowTaskOperatorEntity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> IsSubFlowUpNode(FlowTaskOperatorEntity flowTaskOperatorEntity)
        {
            var nodeList = await _flowTaskRepository.GetTaskNodeList(flowTaskOperatorEntity.TaskId);
            var nodeInfo = await _flowTaskRepository.GetTaskNodeInfo(flowTaskOperatorEntity.TaskNodeId);
            if (nodeInfo.NodeUp == "0")
            {
                return false;
            }
            else
            {
                var rejectNodeList = GetRejectFlowTaskOperatorEntity(nodeList, nodeInfo, nodeInfo.NodePropertyJson.Deserialize<ApproversProperties>());
                return rejectNodeList.Any(x => x.NodeType.Equals("subFlow"));
            }
        }
        #endregion

        #region PrivateMethod
        #region 流程模板解析
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
        private void GetFlowTemplateAll(FlowTemplateJsonModel template, List<TaskNodeModel> nodeList, List<FlowTemplateJsonModel> templateList, List<string> childNodeIdList,string taskId="")
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
        #endregion

        #region 审批人员
        /// <summary>
        /// 根据类型获取审批人
        /// </summary>
        /// <param name="flowTaskOperatorEntityList">审批人集合</param>
        /// <param name="flowTaskNodeEntitieList">所有节点</param>
        /// <param name="nextFlowTaskNodeEntity">下个审批节点数据</param>
        /// <param name="flowTaskNodeEntity">当前审批节点数据</param>
        /// <param name="creatorUserId">发起人</param>
        /// <param name="fromData">表单数据</param>
        /// <param name="type">操作标识（0：提交，1：审批）</param>
        /// <param name="candidateList">候选人</param>
        private async Task AddFlowTaskOperatorEntityByAssigneeType(List<FlowTaskOperatorEntity> flowTaskOperatorEntityList,
            List<FlowTaskNodeEntity> flowTaskNodeEntitieList, FlowTaskNodeEntity flowTaskNodeEntity,
            FlowTaskNodeEntity nextFlowTaskNodeEntity, string creatorUserId, string fromData, int type = 1)
        {
            try
            {
                if (!nextFlowTaskNodeEntity.NodeType.Equals("subFlow"))
                {
                    var approverPropertiers = nextFlowTaskNodeEntity.NodePropertyJson.Deserialize<ApproversProperties>();
                    FlowTaskOperatorEntity flowTaskOperatorEntity = new FlowTaskOperatorEntity();
                    flowTaskOperatorEntity.Id = YitIdHelper.NextId().ToString();
                    flowTaskOperatorEntity.HandleType = approverPropertiers.assigneeType.ToString();
                    flowTaskOperatorEntity.NodeCode = nextFlowTaskNodeEntity.NodeCode;
                    flowTaskOperatorEntity.NodeName = nextFlowTaskNodeEntity.NodeName;
                    flowTaskOperatorEntity.TaskNodeId = nextFlowTaskNodeEntity.Id;
                    flowTaskOperatorEntity.TaskId = nextFlowTaskNodeEntity.TaskId;
                    flowTaskOperatorEntity.CreatorTime = DateTime.Now;
                    flowTaskOperatorEntity.Completion = 0;
                    flowTaskOperatorEntity.State = "0";
                    flowTaskOperatorEntity.Description = GetTimerDate(approverPropertiers, flowTaskNodeEntity.NodeCode);
                    flowTaskOperatorEntity.Type = approverPropertiers.assigneeType.ToString();
                    //创建人信息
                    var creatorUser = _usersService.GetInfoByUserId(creatorUserId);
                    var userList = (await _usersService.GetList()).Select(x => x.Id).ToList();
                    if (nextFlowTaskNodeEntity.NodeCode != "end")
                    {
                        switch (approverPropertiers.assigneeType)
                        {
                            //发起者主管
                            case (int)FlowTaskOperatorEnum.LaunchCharge:
                                flowTaskOperatorEntity.HandleId = type == 0 ? await GetManagerByLevel(_userManager.User.ManagerId, (int)approverPropertiers.managerLevel) : await GetManagerByLevel(creatorUser.ManagerId, (int)approverPropertiers.managerLevel);
                                if (flowTaskOperatorEntity.HandleId.IsNullOrEmpty())
                                {
                                    flowTaskOperatorEntity.HandleId = "admin";
                                }
                                flowTaskOperatorEntityList.Add(flowTaskOperatorEntity);
                                break;
                            //部门主管
                            case (int)FlowTaskOperatorEnum.DepartmentCharge:
                                var organizeEntity = type == 0 ? await _organizeService.GetInfoById(_userManager.User.OrganizeId) : await _organizeService.GetInfoById(creatorUser.OrganizeId);
                                if (organizeEntity.ManagerId.IsNullOrEmpty())
                                {
                                    organizeEntity.ManagerId = "admin";
                                }
                                flowTaskOperatorEntity.HandleId = organizeEntity.ManagerId;
                                flowTaskOperatorEntityList.Add(flowTaskOperatorEntity);
                                break;
                            //发起者本人
                            case (int)FlowTaskOperatorEnum.InitiatorMe:
                                flowTaskOperatorEntity.HandleId = creatorUserId;
                                flowTaskOperatorEntityList.Add(flowTaskOperatorEntity);
                                break;
                            //之前某个节点的审批人(提交时下个节点是环节就跳过，审批则看环节节点是否是当前节点的上级)
                            case (int)FlowTaskOperatorEnum.LinkApprover:
                                if (type == 1 && !IsUpNode(approverPropertiers.nodeId, flowTaskNodeEntitieList, (long)nextFlowTaskNodeEntity.SortCode))
                                {
                                    //环节节点所有经办人(过滤掉加签人)
                                    var handleIds = (await _flowTaskRepository.GetTaskOperatorRecordList(flowTaskNodeEntity.TaskId))
                                        .FindAll(x => x.NodeCode.IsNotEmptyOrNull() && x.NodeCode.Equals(approverPropertiers.nodeId)
                                        && x.HandleStatus == 1 && 0 == x.Status).Where(x => HasFreeApprover(x.TaskOperatorId).Result)
                                        .Select(x => x.HandleId).Distinct().ToList();
                                    foreach (var item in handleIds)
                                    {
                                        var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                                        entity.Id = YitIdHelper.NextId().ToString();
                                        entity.HandleId = item;
                                        entity.HandleType = "5";
                                        entity.Type = "5";
                                        flowTaskOperatorEntityList.Add(entity);
                                    }
                                }
                                else
                                {
                                    var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                                    entity.HandleId = "admin";
                                    entity.HandleType = "5";
                                    entity.Type = "5";
                                    flowTaskOperatorEntityList.Add(entity);
                                }
                                break;
                            //表单值审批人
                            case (int)FlowTaskOperatorEnum.VariableApprover:
                                var jObj = fromData.Deserialize<JObject>();
                                var field = jObj.ContainsKey(approverPropertiers.formField) ? jObj[approverPropertiers.formField].ToString() : "admin";
                                var filedList = field.Split(",").ToList();
                                filedList = userList.Intersect(filedList).ToList();
                                if (filedList.Count == 0)
                                {
                                    flowTaskOperatorEntity.HandleId = "admin";
                                    flowTaskOperatorEntityList.Add(flowTaskOperatorEntity);
                                }
                                else
                                {
                                    foreach (var item in filedList)
                                    {
                                        var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                                        entity.Id = YitIdHelper.NextId().ToString();
                                        entity.HandleId = item;
                                        flowTaskOperatorEntityList.Add(entity);
                                    }
                                }
                                break;
                            //接口审批人(接口结构为{"code":200,"data":{"handleId":"admin"},"msg":""})
                            case (int)FlowTaskOperatorEnum.ServiceApprover:
                                var data = await approverPropertiers.getUserUrl.SetHeaders(new { Authorization = _userManager.ToKen }).GetAsStringAsync();
                                var result = JSON.Deserialize<RESTfulResult<object>>(data);
                                if (result.IsNotEmptyOrNull())
                                {
                                    var resultJobj = result.data.ToObject<JObject>();
                                    if (result.code == 200)
                                    {
                                        var handleId = resultJobj["handleId"].IsNotEmptyOrNull() ? resultJobj["handleId"].ToString() : "admin";
                                        var handleIdList = userList.Intersect(handleId.Split(",").ToList()).ToList();
                                        if (handleIdList.Count == 0)
                                        {
                                            var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                                            entity.Id = YitIdHelper.NextId().ToString();
                                            entity.HandleId = "admin";
                                            flowTaskOperatorEntityList.Add(entity);
                                        }
                                        else
                                        {
                                            foreach (var item in handleIdList)
                                            {
                                                var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                                                entity.Id = YitIdHelper.NextId().ToString();
                                                entity.HandleId = item;
                                                flowTaskOperatorEntityList.Add(entity);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                                    entity.Id = YitIdHelper.NextId().ToString();
                                    flowTaskOperatorEntity.HandleId = "admin";
                                    flowTaskOperatorEntityList.Add(flowTaskOperatorEntity);
                                }
                                break;
                            //候选人
                            case (int)FlowTaskOperatorEnum.CandidateApprover:
                                var userIds = new List<string>();
                                if (type == 0 && nextFlowTaskNodeEntity.Candidates.IsNotEmptyOrNull())
                                {
                                    var candidateDic = nextFlowTaskNodeEntity.Candidates.ToObject<Dictionary<string, List<string>>>();

                                    foreach (var candidate in candidateDic.Keys)
                                    {
                                        userIds = userIds.Union(candidateDic[candidate]).ToList();
                                    }
                                }
                                else
                                {
                                    userIds = _flowTaskRepository.GetFlowCandidates(nextFlowTaskNodeEntity.Id);
                                }
                                foreach (var item in userIds)
                                {
                                    var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                                    entity.Id = YitIdHelper.NextId().ToString();
                                    entity.HandleId = item;
                                    flowTaskOperatorEntityList.Add(entity);
                                }
                                break;
                            default:
                                GetAppointApprover(flowTaskOperatorEntityList, flowTaskOperatorEntity, approverPropertiers);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw HSZException.Oh(ErrorCode.WF0013);
            }
        }

        /// <summary>
        /// 判断经办记录人是否加签且加签是否完成
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<bool> HasFreeApprover(string id)
        {
            var entityList = await GetOperator(id, new List<FlowTaskOperatorEntity>());
            if (entityList.Count == 0)
            {
                return true;
            }
            else
            {
                return entityList.FindAll(x => x.HandleStatus.IsEmpty() || x.HandleStatus == 0).Count() == 0;
            }
        }

        /// <summary>
        /// 指定用户或岗位(审批人)
        /// </summary>
        /// <param name="flowTaskOperatorEntityList">当前节点所有经办</param>
        /// <param name="flowTaskOperatorEntity">当前经办</param>
        /// <param name="approverPropertiers">当前节点属性</param>
        private void GetAppointApprover(List<FlowTaskOperatorEntity> flowTaskOperatorEntityList, FlowTaskOperatorEntity flowTaskOperatorEntity, ApproversProperties approverPropertiers)
        {
            var approverUserList = new List<string>();
            var userList = GetUserDefined(approverPropertiers.approvers, approverPropertiers.approverRole, approverPropertiers.approverPos);
            approverUserList = approverUserList.Union(userList).ToList();
            if (approverUserList.Count == 0)
            {
                var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                entity.Id = YitIdHelper.NextId().ToString();
                entity.HandleId = "admin";
                flowTaskOperatorEntityList.Add(entity);
            }
            else
            {
                foreach (var item in approverUserList.Distinct())
                {
                    var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                    entity.Id = YitIdHelper.NextId().ToString();
                    entity.HandleId = item;
                    flowTaskOperatorEntityList.Add(entity);
                }
            }
        }

        /// <summary>
        /// 获取抄送人
        /// </summary>
        /// <param name="approverPropertiers">节点属性</param>
        /// <param name="flowTaskOperatorEntity">经办</param>
        /// <param name="flowTaskCirculateEntityList">抄送list</param>
        /// <param name="copyIds">自定义抄送</param>
        /// <param name="hanlderState">审批状态</param>
        private void GetflowTaskCirculateEntityList(ApproversProperties approverPropertiers, FlowTaskOperatorEntity flowTaskOperatorEntity, List<FlowTaskCirculateEntity> flowTaskCirculateEntityList, string copyIds, int hanlderState = 1)
        {
            var circulateUserList = copyIds.Split(",").ToList();
            #region 抄送人
            if (hanlderState == 1)
            {
                var userList = GetUserDefined(approverPropertiers.circulateUser, approverPropertiers.circulateRole, approverPropertiers.circulatePosition);
                circulateUserList = circulateUserList.Union(userList).ToList();
            }

            foreach (var item in circulateUserList.Distinct())
            {
                flowTaskCirculateEntityList.Add(new FlowTaskCirculateEntity()
                {
                    Id = YitIdHelper.NextId().ToString(),
                    ObjectType = flowTaskOperatorEntity.HandleType,
                    ObjectId = item,
                    NodeCode = flowTaskOperatorEntity.NodeCode,
                    NodeName = flowTaskOperatorEntity.NodeName,
                    TaskNodeId = flowTaskOperatorEntity.TaskNodeId,
                    TaskId = flowTaskOperatorEntity.TaskId,
                    CreatorTime = DateTime.Now,
                });
            }
            #endregion
        }

        /// <summary>
        /// 获取自定义人员
        /// </summary>
        /// <param name="userList">指定人</param>
        /// <param name="roleList">指定角色</param>
        /// <param name="posList">指定岗位</param>
        /// <returns></returns>
        private List<string> GetUserDefined(List<string> userList, List<string> roleList, List<string> posList)
        {
            if (posList.IsNotEmptyOrNull() && posList.Count > 0)
            {
                foreach (var item in posList)
                {
                    var userPosition = _userRelationService.GetUserId("Position", item);
                    userList = userList.Union(userPosition).ToList();
                }
            }
            if (roleList.IsNotEmptyOrNull() && roleList.Count > 0)
            {
                foreach (var item in roleList)
                {
                    var userRole = _userRelationService.GetUserId("Role", item);
                    userList = userList.Union(userRole).ToList();
                }
            }
            return userList;
        }

        /// <summary>
        /// 获取自定义人员名称
        /// </summary>
        /// <param name="userList"></param>
        /// <param name="roleList"></param>
        /// <param name="posList"></param>
        /// <param name="userNameList"></param>
        /// <returns></returns>
        private async Task GetUserNameDefined(List<string> userList, List<string> roleList, List<string> posList, List<string> userNameList)
        {
            if (userList.IsNotEmptyOrNull() && userList.Count > 0)
            {
                foreach (var item in userList)
                {
                    var approversUser = await _usersService.GetUserName(item);
                    if (approversUser.IsNotEmptyOrNull() && !userNameList.Contains(approversUser))
                    {
                        userNameList.Add(approversUser);
                    }
                }
            }
            if (posList.IsNotEmptyOrNull() && posList.Count > 0)
            {
                foreach (var item in posList)
                {
                    var userPosition = _userRelationService.GetUserId("Position", item);
                    foreach (var item1 in userPosition)
                    {
                        userNameList.Add(await _usersService.GetUserName(item1));
                    }
                }
            }
            if (roleList.IsNotEmptyOrNull() && roleList.Count > 0)
            {
                foreach (var item in roleList)
                {
                    var userRole = _userRelationService.GetUserId("Role", item);
                    foreach (var item1 in userRole)
                    {
                        userNameList.Add(await _usersService.GetUserName(item1));
                    }
                }
            }
        }

        /// <summary>
        /// 获取子流程发起人
        /// </summary>
        /// <param name="childTaskProperties"></param>
        /// <param name="creatorUser"></param>
        /// <param name="flowTaskNodeEntitieList"></param>
        /// <param name="nextFlowTaskNodeEntity"></param>
        /// <param name="fromData"></param>
        /// <returns></returns>
        private async Task<List<string>> GetSubFlowCreator(ChildTaskProperties childTaskProperties, string creatorUser,
            List<FlowTaskNodeEntity> flowTaskNodeEntitieList, FlowTaskNodeEntity nextFlowTaskNodeEntity, string fromData)
        {
            var crUserList = new List<string>();
            var userEntity = _usersService.GetInfoByUserId(creatorUser);
            var userList = (await _usersService.GetList()).Select(x => x.Id).ToList();
            switch (childTaskProperties.initiateType)
            {
                //发起者主管
                case (int)FlowTaskOperatorEnum.LaunchCharge:
                    var crDirector = await GetManagerByLevel(userEntity.ManagerId, childTaskProperties.managerLevel);
                    if (crDirector.IsNullOrEmpty())
                    {
                        crDirector = "admin";
                    }
                    crUserList.Add(crDirector);
                    break;
                //部门主管
                case (int)FlowTaskOperatorEnum.DepartmentCharge:
                    var organizeEntity = await _organizeService.GetInfoById(userEntity.OrganizeId);
                    if (organizeEntity.ManagerId.IsNullOrEmpty())
                    {
                        organizeEntity.ManagerId = "admin";
                    }
                    crUserList.Add(organizeEntity.ManagerId);
                    break;
                //发起者本人
                case (int)FlowTaskOperatorEnum.InitiatorMe:
                    crUserList.Add(creatorUser);
                    break;
                //之前某个节点的审批人(提交时下个节点是环节就跳过，审批则看环节节点是否是当前节点的上级)
                case (int)FlowTaskOperatorEnum.LinkApprover:
                    if (!IsUpNode(childTaskProperties.nodeId, flowTaskNodeEntitieList, (long)nextFlowTaskNodeEntity.SortCode))
                    {
                        //环节节点所有经办人(过滤掉加签人)
                        crUserList = (await _flowTaskRepository.GetTaskOperatorRecordList(nextFlowTaskNodeEntity.TaskId))
                            .FindAll(x => x.NodeCode.IsNotEmptyOrNull() && x.NodeCode.Equals(childTaskProperties.nodeId)
                            && x.HandleStatus == 1 && 0 == x.Status).Where(x => HasFreeApprover(x.TaskOperatorId).Result)
                            .Select(x => x.HandleId).Distinct().ToList();
                    }
                    break;
                ////表单值审批人
                case (int)FlowTaskOperatorEnum.VariableApprover:
                    var jObj = fromData.Deserialize<JObject>();
                    var field = jObj.ContainsKey(childTaskProperties.formField) ? jObj[childTaskProperties.formField].ToString() : "admin";
                    var filedList = field.Split(",").ToList();
                    crUserList = userList.Intersect(filedList).ToList();
                    if (crUserList.Count == 0)
                    {
                        crUserList.Add("admin");
                    }
                    break;
                //接口审批人(接口结构为{"code":200,"data":{"handleId":"admin"},"msg":""})
                case (int)FlowTaskOperatorEnum.ServiceApprover:
                    var data = await childTaskProperties.getUserUrl.SetHeaders(new { Authorization = _userManager.ToKen }).GetAsStringAsync();
                    var result = JSON.Deserialize<RESTfulResult<object>>(data);
                    if (result.IsNotEmptyOrNull())
                    {
                        var resultJobj = result.data.ToObject<JObject>();
                        if (result.code == 200)
                        {
                            var handleId = resultJobj["handleId"].IsNotEmptyOrNull() ? resultJobj["handleId"].ToString() : "admin";
                            crUserList = userList.Intersect(handleId.Split(",").ToList()).ToList();
                            if (crUserList.Count == 0)
                            {
                                crUserList.Add("admin");
                            }
                        }
                    }
                    else
                    {
                        crUserList.Add("admin");
                    }
                    break;
                default:
                    var userIdList = GetUserDefined(childTaskProperties.initiator, childTaskProperties.initiateRole, childTaskProperties.initiatePos);
                    crUserList = crUserList.Union(userIdList).ToList();
                    break;
            }
            if (crUserList.Count == 0)
            {
                crUserList.Add("admin");
            }
            return crUserList.Distinct().ToList();
        }

        /// <summary>
        /// 获取候选人信息
        /// </summary>
        /// <param name="flowTaskCandidateModels"></param>
        /// <param name="nextNodeEntities"></param>
        /// <param name="nodeEntities"></param>
        /// <returns></returns>
        private async Task GetCandidates(List<FlowTaskCandidateModel> flowTaskCandidateModels, List<FlowTaskNodeEntity> nextNodeEntities,
            List<FlowTaskNodeEntity> nodeEntities)
        {
            foreach (var item in nextNodeEntities)
            {
                if (item.NodeType.Equals("approver"))
                {
                    var candidateItem = new FlowTaskCandidateModel();
                    var approverPropertiers = item.NodePropertyJson.Deserialize<ApproversProperties>();
                    if (approverPropertiers.assigneeType == 7)
                    {
                        candidateItem.nodeId = item.NodeCode;
                        candidateItem.nodeName = item.NodeName;
                        flowTaskCandidateModels.Add(candidateItem);
                    }
                }
                else if (item.NodeType.Equals("subFlow"))
                {
                    var subFlowNextNodes = nodeEntities.FindAll(m => item.NodeNext.Contains(m.NodeCode));
                    await GetCandidates(flowTaskCandidateModels, subFlowNextNodes, nodeEntities);
                }
            }
        }

        /// <summary>
        /// 候选人员列表
        /// </summary>
        /// <param name="nextNodeEntity"></param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        private async Task<dynamic> GetCandidateItems(FlowTaskNodeEntity nextNodeEntity, FlowHandleModel flowHandleModel)
        {
            var approverPropertiers = nextNodeEntity.NodePropertyJson.Deserialize<ApproversProperties>();
            var objIds = approverPropertiers.approverPos.Union(approverPropertiers.approverRole).ToList();
            return await _userRelationService.GetUserIdPage(approverPropertiers.approvers, objIds, flowHandleModel);
        }

        /// <summary>
        /// 获取级别主管
        /// </summary>
        /// <param name="managerId">主管id</param>
        /// <param name="level">级别</param>
        /// <returns></returns>
        private async Task<string> GetManagerByLevel(string managerId, int level)
        {
            --level;
            if (level == 0)
            {
                return managerId;
            }
            else
            {
                var manager = (await _usersService.GetList()).Find(x => x.Id.Equals(managerId));
                return manager.IsNullOrEmpty() ? "" : await GetManagerByLevel(manager.ManagerId, level);
            }
        }

        /// <summary>
        /// 获取审批人名称
        /// </summary>
        /// <param name="flowTaskNodeModel">当前节点</param>
        /// <param name="flowTaskEntity">任务</param>
        /// <param name="formData">表单数据</param>
        /// <param name="flowTaskNodeEntities">所有节点</param>
        /// <returns></returns>
        private async Task<string> GetApproverUserName(FlowTaskNodeModel flowTaskNodeModel, FlowTaskEntity flowTaskEntity, string formData, List<FlowTaskNodeEntity> flowTaskNodeEntities)
        {
            var userNameList = new List<string>();
            var creatorUser = (await _usersService.GetList()).Find(x => x.Id == flowTaskEntity.CreatorUserId);
            if (flowTaskNodeModel.nodeType.Equals("start"))
            {
                var userName = await _usersService.GetUserName(creatorUser.Id);
                userNameList.Add(userName);
            }
            else if (flowTaskNodeModel.nodeType.Equals("subFlow"))
            {
                var subFlowProperties = flowTaskNodeModel.nodePropertyJson.Deserialize<ChildTaskProperties>();
                var userIdList = await GetSubFlowCreator(subFlowProperties, creatorUser.Id, flowTaskNodeEntities, flowTaskNodeModel.Adapt<FlowTaskNodeEntity>(), formData);
                await GetUserNameDefined(userIdList, null, null, userNameList);
            }
            else
            {
                var approverProperties = flowTaskNodeModel.nodePropertyJson.Deserialize<ApproversProperties>();
                switch (approverProperties.assigneeType)
                {
                    //发起者主管
                    case (int)FlowTaskOperatorEnum.LaunchCharge:
                        var managerId = await GetManagerByLevel(creatorUser.ManagerId, (int)approverProperties.managerLevel);
                        var manager = (await _usersService.GetList()).Find(x => x.Id == managerId);
                        if (manager != null)
                        {
                            userNameList.Add(await _usersService.GetUserName(manager.Id));
                        }
                        break;
                    //部门主管
                    case (int)FlowTaskOperatorEnum.DepartmentCharge:
                        var organize = await _organizeService.GetInfoById(creatorUser.OrganizeId);
                        if (organize != null && organize.ManagerId.IsNotEmptyOrNull())
                        {
                            userNameList.Add(await _usersService.GetUserName(organize.ManagerId));
                        }
                        break;
                    //发起者本人
                    case (int)FlowTaskOperatorEnum.InitiatorMe:
                        var userName = await _usersService.GetUserName(creatorUser.Id);
                        userNameList.Add(userName);
                        break;
                    //环节
                    case (int)FlowTaskOperatorEnum.LinkApprover:
                        if (!IsUpNode(approverProperties.nodeId, flowTaskNodeEntities, (long)flowTaskNodeModel.sortCode))
                        {
                            //环节节点所有经办人
                            var handleIds = (await _flowTaskRepository.GetTaskOperatorRecordList(flowTaskNodeModel.taskId)).
                                FindAll(x => x.NodeCode.IsNotEmptyOrNull() && x.NodeCode.Equals(approverProperties.nodeId)
                                && x.HandleStatus == 1 && x.Status == 0).Where(x => HasFreeApprover(x.TaskOperatorId).Result).Select(x => x.HandleId).Distinct().ToList();
                            foreach (var item in handleIds)
                            {
                                var linkUserName = await _usersService.GetUserName(item);
                                userNameList.Add(linkUserName);
                            }
                        }
                        break;
                    //变量
                    case (int)FlowTaskOperatorEnum.VariableApprover:
                        var jObj = formData.Deserialize<JObject>();
                        var fieldList = jObj[approverProperties.formField].ToString().Split(",");
                        foreach (var item in fieldList)
                        {
                            var variableUserName = await _usersService.GetUserName(item);
                            if (variableUserName.IsEmpty())
                            {
                                userNameList.Add(await _usersService.GetUserName("admin"));
                            }
                            else
                            {
                                userNameList.Add(variableUserName);
                            }
                        }
                        break;
                    //服务
                    case (int)FlowTaskOperatorEnum.ServiceApprover:
                        try
                        {
                            var data = await approverProperties.getUserUrl.SetHeaders(new { Authorization = _userManager.ToKen }).GetAsStringAsync();
                            var result = JSON.Deserialize<RESTfulResult<object>>(data);
                            if (result.IsNotEmptyOrNull())
                            {
                                var resultJobj = result.data.ToObject<JObject>();
                                if (result.code == 200)
                                {
                                    var handleId = resultJobj["handleId"].IsNotEmptyOrNull() ? resultJobj["handleId"].ToString() : "";
                                    foreach (var item in handleId.Split(","))
                                    {
                                        var serviceUserName = await _usersService.GetUserName(item);
                                        if (serviceUserName.IsNotEmptyOrNull())
                                        {
                                            userNameList.Add(serviceUserName);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {

                            break;
                        }
                        break;
                    //候选人
                    case (int)FlowTaskOperatorEnum.CandidateApprover:
                        var ids = _flowTaskRepository.GetFlowCandidates(flowTaskNodeModel.id);
                        foreach (var item in ids)
                        {
                            userNameList.Add(await _usersService.GetUserName(item));
                        }
                        break;
                    default:
                        await GetUserNameDefined(approverProperties.approvers, approverProperties.approverRole, approverProperties.approverPos, userNameList);
                        break;
                }
                if (userNameList.Count == 0 && approverProperties.assigneeType != 5)
                {
                    userNameList.Add(await _usersService.GetUserName("admin"));
                }
            }
            return string.Join(",", userNameList.Distinct());
        }

        /// <summary>
        /// 获取定时器节点定时结束时间
        /// </summary>
        /// <param name="approverPropertiers">定时器节点属性</param>
        /// <param name="nodeCode">定时器节点编码</param>
        /// <returns></returns>
        private string GetTimerDate(ApproversProperties approverPropertiers, string nodeCode)
        {
            var nowTime = DateTime.Now;
            if (approverPropertiers.timerList.Count > 0)
            {
                string upNodeStr = string.Join(",", approverPropertiers.timerList.Select(x => x.upNodeCode).ToArray());
                if (upNodeStr.Contains(nodeCode))
                {
                    foreach (var item in approverPropertiers.timerList)
                    {
                        var result = DateTime.Now.AddDays(item.day).AddHours(item.hour).AddMinutes(item.minute).AddSeconds(item.second);
                        if (result > nowTime)
                        {
                            nowTime = result;
                        }
                    }
                    return nowTime.ToString();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 节点处理
        /// <summary>
        /// 判断分流节点是否完成
        /// (因为分流节点最终只能是一个 所以只需判断下一节点中的其中一个的上节点完成情况)
        /// </summary>
        /// <param name="flowTaskNodeEntityList">所有节点</param>
        /// <param name="nextNodeCode">下一个节点编码</param>
        /// <param name="flowTaskNodeEntity">当前节点</param>
        /// <returns></returns>
        private bool IsShuntNodeCompletion(List<FlowTaskNodeEntity> flowTaskNodeEntityList, string nextNodeCode, FlowTaskNodeEntity flowTaskNodeEntity)
        {
            var shuntNodeCodeList = flowTaskNodeEntityList.FindAll(x => x.NodeNext.IsNotEmptyOrNull() &&
            x.NodeCode != flowTaskNodeEntity.NodeCode && x.NodeNext.Contains(nextNodeCode) && x.Completion == 0);
            return shuntNodeCodeList.Count == 0;
        }

        /// <summary>
        /// 替换任务当前节点
        /// </summary>
        /// <param name="flowTaskNodeEntityList">所有节点</param>
        /// <param name="nextNodeCodeList">替换数据</param>
        /// <param name="thisStepId">源数据</param>
        /// <returns></returns>
        private string GetThisStepId(List<FlowTaskNodeEntity> flowTaskNodeEntityList, List<string> nextNodeCodeList, string thisStepId)
        {
            var replaceNodeCodeList = new List<string>();
            nextNodeCodeList.ForEach(item =>
            {
                var nodeCode = new List<string>();
                var nodeEntityList = flowTaskNodeEntityList.FindAll(x => x.NodeNext.IsNotEmptyOrNull() && x.NodeNext.Contains(item));
                nodeCode = nodeEntityList.Select(x => x.NodeCode).ToList();
                replaceNodeCodeList = replaceNodeCodeList.Union(nodeCode).ToList();
            });
            var thisNodeList = new List<string>();
            if (thisStepId.IsNotEmptyOrNull())
            {
                thisNodeList = thisStepId.Split(",").ToList();
            }
            //去除当前审批节点并添加下个节点
            var list = thisNodeList.Except(replaceNodeCodeList).Union(nextNodeCodeList);
            return string.Join(",", list.ToArray());
        }

        /// <summary>
        /// 替换驳回当前任务节点
        /// </summary>
        /// <param name="flowTaskNodeEntityList"></param>
        /// <param name="nextNodeCodeList"></param>
        /// <param name="thisStepId"></param>
        /// <returns></returns>
        private string GetThisStepIdReject(List<FlowTaskNodeEntity> flowTaskNodeEntityList, List<FlowTaskNodeEntity> nextNodeCodeList, string thisStepId)
        {
            var replaceNodeCodeList = new List<string>();
            foreach (var item in nextNodeCodeList)
            {
                var nodeCode = item.NodeNext.Split(",").ToList();
                replaceNodeCodeList = replaceNodeCodeList.Union(nodeCode).ToList();
            }

            var thisNodeList = new List<string>();
            if (thisStepId.IsNotEmptyOrNull())
            {
                thisNodeList = thisStepId.Split(",").ToList();
            }
            //去除当前审批节点并添加下个节点
            var list = thisNodeList.Except(replaceNodeCodeList).Union(nextNodeCodeList.Select(x => x.NodeCode));
            return string.Join(",", list.ToArray());
        }

        /// <summary>
        /// 撤回替换任务当前节点
        /// </summary>
        /// <param name="flowTaskNodeEntityList"></param>
        /// <param name="flowTaskNodeEntity"></param>
        /// <param name="thisStepId"></param>
        /// <returns></returns>
        private string GetRecallThisStepId(List<FlowTaskNodeEntity> flowTaskNodeEntityList, FlowTaskNodeEntity flowTaskNodeEntity, string thisStepId)
        {
            //撤回节点下一节点
            var nextNode = flowTaskNodeEntityList.FindAll(x => flowTaskNodeEntity.NodeNext.Contains(x.NodeCode));

            if (nextNode.Count == 1)
            {
                var recallNodeList = flowTaskNodeEntityList.FindAll(x => x.NodeNext.IsNotEmptyOrNull() && x.NodeNext.Contains(nextNode.First().NodeCode));
                if (recallNodeList.Count > 1)
                {
                    var nodeCodes = recallNodeList.FindAll(x => x.Completion == 0).Select(x => x.NodeCode).ToList();
                    nodeCodes.Add(flowTaskNodeEntity.NodeCode);
                    return string.Join(",", nodeCodes);
                }
                else
                {
                    return flowTaskNodeEntity.NodeCode;
                }
            }
            else
            {
                var ids = thisStepId.Split(",").ToList();
                var nodes = nextNode.Select(x => x.NodeCode).ToList();
                var thisNodes = ids.Except(nodes).ToList();
                thisNodes.Add(flowTaskNodeEntity.NodeCode);
                return string.Join(",", thisNodes);
            }
        }

        /// <summary>
        /// 根据当前节点编码获取节点名称
        /// </summary>
        /// <param name="flowTaskNodeEntityList"></param>
        /// <param name="thisStepId"></param>
        /// <returns></returns>
        private string GetThisStep(List<FlowTaskNodeEntity> flowTaskNodeEntityList, string thisStepId)
        {
            var ids = thisStepId.Split(",").ToList();
            var nextNodeNameList = new List<string>();
            foreach (var item in ids)
            {
                var name = flowTaskNodeEntityList.Find(x => x.NodeCode.Equals(item)).NodeName;
                nextNodeNameList.Add(name);
            }
            return string.Join(",", nextNodeNameList);
        }

        /// <summary>
        /// 获取驳回节点
        /// </summary>
        /// <param name="flowTaskNodeEntityList">所有节点</param>
        /// <param name="flowTaskNodeEntity">当前节点</param>
        /// <param name="approversProperties">当前节点属性</param>
        /// <returns></returns>
        private List<FlowTaskNodeEntity> GetRejectFlowTaskOperatorEntity(List<FlowTaskNodeEntity> flowTaskNodeEntityList, FlowTaskNodeEntity flowTaskNodeEntity, ApproversProperties approversProperties)
        {
            //驳回节点
            var upflowTaskNodeEntityList = new List<FlowTaskNodeEntity>();
            if (flowTaskNodeEntity.NodeUp == "1")
            {
                upflowTaskNodeEntityList = flowTaskNodeEntityList.FindAll(x => x.NodeNext.IsNotEmptyOrNull() && x.NodeNext.Contains(flowTaskNodeEntity.NodeCode));
            }
            else
            {
                var upflowTaskNodeEntity = flowTaskNodeEntityList.Find(x => x.NodeCode == approversProperties.rejectStep);
                upflowTaskNodeEntityList = flowTaskNodeEntityList.FindAll(x => x.SortCode == upflowTaskNodeEntity.SortCode);
            }
            return upflowTaskNodeEntityList;
        }

        /// <summary>
        /// 修改节点完成状态
        /// </summary>
        /// <param name="taskNodeList"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private async Task RejectUpdateTaskNode(List<FlowTaskNodeEntity> taskNodeList, int state)
        {
            foreach (var item in taskNodeList)
            {
                item.Completion = state;
                await _flowTaskRepository.UpdateTaskNode(item);
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

        /// <summary>
        /// 判断表单参数是否必填
        /// </summary>
        /// <returns></returns>
        private bool IsRequiredParameters(List<FormOperatesModels> formOperatesModels, Dictionary<string, object> dic)
        {
            foreach (var item in formOperatesModels)
            {
                if (item.required)
                {
                    if (dic[item.id] is string && dic[item.id].IsNullOrEmpty())
                    {
                        return true;
                    }
                    if (dic[item.id] is JArray && dic[item.id].ToObject<List<object>>().Count == 0)
                    {
                        return true; ;
                    }
                    if (dic[item.id] is object && dic[item.id].IsNullOrEmpty())
                    {
                        return true;
                    }
                    if (dic[item.id] == null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否为上级节点
        /// </summary>
        /// <param name="nodeId">上级节点编码</param>
        /// <param name="flowTaskNodeEntitieList">所有节点</param>
        /// <param name="sortCode">当前节点排序码</param>
        /// <returns></returns>
        private bool IsUpNode(string nodeId, List<FlowTaskNodeEntity> flowTaskNodeEntitieList, long sortCode)
        {
            var upflowTaskNodeEntity = flowTaskNodeEntitieList.Find(x => x.NodeCode.Equals(nodeId) && x.SortCode < sortCode);
            return upflowTaskNodeEntity.IsNullOrEmpty();
        }

        /// <summary>
        /// 递归节点下所有节点
        /// </summary>
        /// <param name="flowTaskNodeList"></param>
        /// <param name="nodeNext"></param>
        /// <param name="rejectTaskNodeEntityList"></param>
        private void GetAllNextNode(List<FlowTaskNodeEntity> flowTaskNodeList, string nodeNext, List<FlowTaskNodeEntity> rejectTaskNodeEntityList)
        {
            var nextNodes = nodeNext.Split(",").ToList();
            var flowTaskNodeEntityList = flowTaskNodeList.FindAll(x => nextNodes.Contains(x.NodeCode));
            rejectTaskNodeEntityList = rejectTaskNodeEntityList.Union(flowTaskNodeEntityList).ToList();
            foreach (var item in flowTaskNodeEntityList)
            {
                if (!item.NodeCode.Equals("end"))
                {
                    GetAllNextNode(flowTaskNodeList, item.NodeNext, rejectTaskNodeEntityList);
                }
            }
        }

        /// <summary>
        /// 根据表单数据解析模板获取流程节点
        /// </summary>
        /// <param name="flowEngineEntity"></param>
        /// <param name="fromData"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private List<FlowTaskNodeEntity> ParsingTemplateGetNodeList(FlowEngineEntity flowEngineEntity,string fromData,string taskId)
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
        #endregion

        #region 经办处理
        /// <summary>
        /// 根据不同节点类型修改经办数据
        /// </summary>
        /// <param name="flowTaskOperatorEntity">当前经办</param>
        /// <param name="thisFlowTaskOperatorEntityList">当前节点所有经办</param>
        /// <param name="aspproversProperties">当前节点属性</param>
        /// <param name="handleStatus">审批类型（0：拒绝，1：同意）</param>
        /// <param name="freeApprover">加签人</param>
        /// <param name="flowId">流程id</param>
        /// <returns></returns>
        private async Task UpdateFlowTaskOperator(FlowTaskOperatorEntity flowTaskOperatorEntity,
            List<FlowTaskOperatorEntity> thisFlowTaskOperatorEntityList, ApproversProperties aspproversProperties,
            int handleStatus, string freeApprover, string flowId)
        {
            //当前用户委托人id
            List<string> delegateUserIdList = await _flowDelegateService.GetDelegateUserId(_userManager.UserId, flowId);
            if (aspproversProperties.counterSign == 0)
            {
                if (freeApprover.IsNullOrEmpty())
                {
                    //未审批经办
                    var notCompletion = GetNotCompletion(thisFlowTaskOperatorEntityList);
                    await _flowTaskRepository.UpdateTaskOperator(notCompletion);
                }
            }
            else
            {
                var deleIndex = (await GetDelegateOperator(flowTaskOperatorEntity.TaskId, flowTaskOperatorEntity.TaskNodeId, delegateUserIdList, handleStatus)).Count;
                if (IsAchievebilProportion(thisFlowTaskOperatorEntityList, handleStatus, (int)aspproversProperties.countersignRatio, deleIndex, freeApprover.IsEmpty()))
                {
                    //未审批经办
                    var notCompletion = GetNotCompletion(thisFlowTaskOperatorEntityList);
                    await _flowTaskRepository.UpdateTaskOperator(notCompletion);
                }
            }
            await UpdateThisOperator(flowTaskOperatorEntity, handleStatus);
        }

        /// <summary>
        /// 根据当前审批节点插入下一节点经办
        /// </summary>
        /// <param name="flowTaskNodeEntityList">所有节点</param>
        /// <param name="flowTaskNodeEntity">当前节点</param>
        /// <param name="approversProperties">当前节点属性</param>
        /// <param name="thisFlowTaskOperatorEntityList">当前节点所有经办</param>
        /// <param name="handleStatus">审批状态</param>
        /// <param name="flowTaskEntity">流程任务</param>
        /// <param name="freeApproverUserId">加签人</param>
        /// <param name="nextFlowTaskOperatorEntityList">经办数据</param>
        /// <param name="fromData">表单数据</param>
        /// <param name="flowHandleModel">审批详情</param>
        /// <param name="formType">单据类型</param>
        /// <returns></returns>
        private async Task CreateNextFlowTaskOperator(List<FlowTaskNodeEntity> flowTaskNodeEntityList, FlowTaskNodeEntity flowTaskNodeEntity,
            ApproversProperties approversProperties, List<FlowTaskOperatorEntity> thisFlowTaskOperatorEntityList, int handleStatus, FlowTaskEntity flowTaskEntity, string freeApproverUserId,
            List<FlowTaskOperatorEntity> nextFlowTaskOperatorEntityList, string fromData, FlowHandleModel flowHandleModel, int formType)
        {
            try
            {
                var nextNodeCodeList = new List<string>();
                var nextNodeCompletion = new List<int>();
                var isInsert = false;
                //下个节点集合
                List<FlowTaskNodeEntity> nextNodeEntity = flowTaskNodeEntityList.FindAll(m => flowTaskNodeEntity.NodeNext.Contains(m.NodeCode));
                //当前用户委托人id
                List<string> delegateUserIdList = await _flowDelegateService.GetDelegateUserId(_userManager.UserId, flowTaskEntity.FlowId);
                var deleIndex = (await GetDelegateOperator(flowTaskNodeEntity.TaskId, flowTaskNodeEntity.Id, delegateUserIdList, handleStatus)).Count;
                if (handleStatus == 0)
                {
                    if (approversProperties.counterSign == 0)
                    {
                        await GetNextOperatorByNo(flowTaskNodeEntity, flowTaskEntity, flowTaskNodeEntityList, approversProperties, nextFlowTaskOperatorEntityList, fromData);
                    }
                    else
                    {
                        if (IsAchievebilProportion(thisFlowTaskOperatorEntityList, handleStatus, (int)approversProperties.countersignRatio, deleIndex, freeApproverUserId.IsEmpty()))
                        {
                            await GetNextOperatorByNo(flowTaskNodeEntity, flowTaskEntity, flowTaskNodeEntityList, approversProperties, nextFlowTaskOperatorEntityList, fromData);
                        }
                    }
                }
                else
                {
                    foreach (var item in nextNodeEntity)
                    {
                        if (approversProperties.counterSign == 0)
                        {
                            isInsert = true;
                            await GetNextOperatorByYes(flowTaskNodeEntity, item, nextNodeCodeList, nextNodeCompletion, nextFlowTaskOperatorEntityList, flowTaskNodeEntityList, flowTaskEntity, fromData);
                        }
                        else
                        {
                            if (IsAchievebilProportion(thisFlowTaskOperatorEntityList, handleStatus, (int)approversProperties.countersignRatio, deleIndex, freeApproverUserId.IsEmpty()))
                            {
                                isInsert = true;
                                await GetNextOperatorByYes(flowTaskNodeEntity, item, nextNodeCodeList, nextNodeCompletion, nextFlowTaskOperatorEntityList, flowTaskNodeEntityList, flowTaskEntity, fromData);
                            }
                        }

                    }
                    #region 判断是否插入下个节点数据
                    //下一节点是分流必定有审批人
                    if (nextNodeEntity.Count > 1)
                    {
                        flowTaskEntity.ThisStepId = GetThisStepId(flowTaskNodeEntityList, nextNodeCodeList, flowTaskEntity.ThisStepId);
                        flowTaskEntity.ThisStep = GetThisStep(flowTaskNodeEntityList, flowTaskEntity.ThisStepId);
                        flowTaskEntity.Completion = nextNodeCompletion.Count == 0 ? flowTaskEntity.Completion : nextNodeCompletion.Min();
                        await _flowTaskRepository.CreateTaskOperator(nextFlowTaskOperatorEntityList);
                    }
                    else
                    {
                        //判断当前节点在不在分流当中且是否为分流的最后审批节点
                        var nextNode = nextNodeEntity.FirstOrDefault();
                        var isShuntNodeCompletion = IsShuntNodeCompletion(flowTaskNodeEntityList, nextNode.NodeCode, flowTaskNodeEntity);
                        if (nextFlowTaskOperatorEntityList.Count > 0)
                        {
                            if (isShuntNodeCompletion)
                            {
                                flowTaskEntity.ThisStepId = GetThisStepId(flowTaskNodeEntityList, nextNodeCodeList, flowTaskEntity.ThisStepId);
                                flowTaskEntity.ThisStep = GetThisStep(flowTaskNodeEntityList, flowTaskEntity.ThisStepId);
                                flowTaskEntity.Completion = nextNodeCompletion.Count == 0 ? flowTaskEntity.Completion : nextNodeCompletion.Min();
                                await _flowTaskRepository.CreateTaskOperator(nextFlowTaskOperatorEntityList);
                            }
                            else
                            {
                                if (nextNode.NodeCode.Equals("end"))
                                {
                                    flowTaskEntity.Status = FlowTaskStatusEnum.Adopt;
                                    flowTaskEntity.Completion = 100;
                                    flowTaskEntity.EndTime = DateTime.Now;
                                    flowTaskEntity.ThisStepId = "end";
                                    flowTaskEntity.ThisStep = "结束";
                                }
                                else
                                {
                                    var thisStepIds = flowTaskEntity.ThisStepId.Split(",").ToList();
                                    thisStepIds.Remove(flowTaskNodeEntity.NodeCode);

                                    flowTaskEntity.ThisStepId = string.Join(",", thisStepIds.ToArray());
                                    flowTaskEntity.ThisStep = GetThisStep(flowTaskNodeEntityList, flowTaskEntity.ThisStepId);
                                }
                            }

                        }
                        else
                        {
                            //下一节点没有审批人(1.当前会签节点没结束，2.结束节点，3.子流程)
                            if (isShuntNodeCompletion)
                            {
                                var isLastEndNode = flowTaskNodeEntityList.FindAll(x =>
                            x.NodeNext.IsNotEmptyOrNull() && x.NodeNext.Equals("end")
                            && !x.NodeCode.Equals(flowTaskNodeEntity.NodeCode) && x.Completion == 0).Count == 0;
                                //下一节点是子流程
                                if (nextNode.NodeType.Equals("subFlow") && isInsert)
                                {
                                    flowTaskEntity.ThisStepId = GetThisStepId(flowTaskNodeEntityList, nextNodeCodeList, flowTaskEntity.ThisStepId);
                                    flowTaskEntity.ThisStep = GetThisStep(flowTaskNodeEntityList, flowTaskEntity.ThisStepId);
                                    flowTaskEntity.Completion = nextNodeCompletion.Count == 0 ? flowTaskEntity.Completion : nextNodeCompletion.Min();
                                    var childTaskPro = nextNode.NodePropertyJson.Deserialize<ChildTaskProperties>();
                                    var childTaskCrUserList = await GetSubFlowCreator(childTaskPro, flowTaskEntity.CreatorUserId, flowTaskNodeEntityList, nextNode, fromData);
                                    var childFLowEngine = await _flowEngineService.GetInfo(childTaskPro.flowId);
                                    var isSysTable = childFLowEngine.FormType == 1;
                                    var childFormData = await GetSubFlowFormData(childTaskPro, fromData);
                                    childTaskPro.childTaskId = await CreateSubProcesses(childTaskPro, childFormData, flowTaskEntity.Id, childTaskCrUserList);
                                    childTaskPro.formData = fromData;
                                    nextNode.NodePropertyJson = childTaskPro.ToJson();
                                    //将子流程id保存到主流程的子流程节点属性上
                                    nextNode.Completion = childTaskPro.isAsync ? 1 : 0;
                                    await _flowTaskRepository.UpdateTaskNode(nextNode);
                                    await Alerts(childTaskPro.launchMsgConfig, childTaskCrUserList, fromData);
                                    if (childTaskPro.isAsync)
                                    {
                                        flowTaskNodeEntityList.Remove(nextNodeEntity.FirstOrDefault());
                                        flowTaskNodeEntityList.Add(nextNode);
                                        await CreateNextFlowTaskOperator(flowTaskNodeEntityList, nextNode,
                                            nextNode.NodePropertyJson.Deserialize<ApproversProperties>(), new List<FlowTaskOperatorEntity>(),
                                            handleStatus, flowTaskEntity, freeApproverUserId, nextFlowTaskOperatorEntityList, fromData,
                                            flowHandleModel, formType);
                                    }
                                }
                                else if (nextNode.NodeCode.Equals("end") && isLastEndNode)
                                {
                                    flowTaskEntity.Status = FlowTaskStatusEnum.Adopt;
                                    flowTaskEntity.Completion = 100;
                                    flowTaskEntity.EndTime = DateTime.Now;
                                    flowTaskEntity.ThisStepId = "end";
                                    flowTaskEntity.ThisStep = "结束";
                                }
                                else
                                {
                                    flowTaskEntity.ThisStepId = GetThisStepId(flowTaskNodeEntityList, nextNodeCodeList, flowTaskEntity.ThisStepId);
                                    flowTaskEntity.ThisStep = GetThisStep(flowTaskNodeEntityList, flowTaskEntity.ThisStepId);
                                    flowTaskEntity.Completion = nextNodeCompletion.Count == 0 ? flowTaskEntity.Completion : nextNodeCompletion.Min();
                                }
                            }
                            else
                            {
                                if (nextNode.NodeCode.Equals("end") && isShuntNodeCompletion)
                                {
                                    flowTaskEntity.Status = FlowTaskStatusEnum.Adopt;
                                    flowTaskEntity.Completion = 100;
                                    flowTaskEntity.EndTime = DateTime.Now;
                                    flowTaskEntity.ThisStepId = "end";
                                    flowTaskEntity.ThisStep = "结束";
                                }
                                else
                                {
                                    var thisStepIds = flowTaskEntity.ThisStepId.Split(",").ToList();
                                    thisStepIds.Remove(flowTaskNodeEntity.NodeCode);

                                    flowTaskEntity.ThisStepId = string.Join(",", thisStepIds.ToArray());
                                    flowTaskEntity.ThisStep = GetThisStep(flowTaskNodeEntityList, flowTaskEntity.ThisStepId);
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取委托经办数据
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskNodeId"></param>
        /// <param name="userId"></param>
        /// <param name="handleStatus"></param>
        /// <returns></returns>
        private async Task<List<FlowTaskOperatorEntity>> GetDelegateOperator(string taskId, string taskNodeId, List<string> userId, int handleStatus)
        {
            try
            {
                var entityList = (await _flowTaskRepository.GetTaskOperatorList(taskId)).FindAll(x => x.TaskNodeId == taskNodeId);
                var upEntityList = new List<FlowTaskOperatorEntity>();
                foreach (var item in userId)
                {
                    var upEntity = entityList.Find(x => x.HandleId == item);
                    if (upEntity.IsNotEmptyOrNull())
                    {
                        upEntity.HandleStatus = handleStatus;
                        upEntity.Completion = 1;
                        upEntity.HandleTime = DateTime.Now;
                        upEntityList.Add(upEntity);
                    }
                }
                return upEntityList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 修改当前经办以及所属委托经办
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="handleStatus"></param>
        /// <param name="delegateUserIdList"></param>
        /// <returns></returns>
        private async Task UpdateThisOperator(FlowTaskOperatorEntity entity, int handleStatus)
        {
            entity.HandleStatus = handleStatus;
            entity.Completion = 1;
            entity.HandleTime = DateTime.Now;
            await _flowTaskRepository.UpdateTaskOperator(entity);
        }

        /// <summary>
        /// 获取未审经办并修改完成状态
        /// </summary>
        /// <param name="thisFlowTaskOperatorEntityList"></param>
        /// <returns></returns>
        private List<FlowTaskOperatorEntity> GetNotCompletion(List<FlowTaskOperatorEntity> thisFlowTaskOperatorEntityList)
        {
            var notCompletion = thisFlowTaskOperatorEntityList.FindAll(x => x.Completion == 0);
            notCompletion.ForEach(item =>
            {
                item.Completion = 1;
            });
            return notCompletion;
        }

        /// <summary>
        /// 获取驳回节点经办
        /// </summary>
        /// <param name="flowTaskNodeEntity">当前节点</param>
        /// <param name="flowTaskEntity">当前任务</param>
        /// <param name="flowTaskNodeEntityList">所有节点</param>
        /// <param name="approversProperties">当前节点属性</param>
        /// <param name="nextFlowTaskOperatorEntityList">下个节点存储list</param>
        /// <param name="fromData">表单数据</param>
        /// <returns></returns>
        private async Task GetNextOperatorByNo(FlowTaskNodeEntity flowTaskNodeEntity, FlowTaskEntity flowTaskEntity, List<FlowTaskNodeEntity> flowTaskNodeEntityList, ApproversProperties approversProperties, List<FlowTaskOperatorEntity> nextFlowTaskOperatorEntityList, string fromData)
        {
            if (flowTaskNodeEntity.NodeUp == "0")
            {
                flowTaskEntity.ThisStepId = flowTaskNodeEntityList.Find(x => x.NodeType.Equals("start")).NodeCode;
                flowTaskEntity.ThisStep = "开始";
                flowTaskEntity.Completion = 0;
                flowTaskEntity.Status = FlowTaskStatusEnum.Reject;
            }
            else
            {
                //当上节点为开始节点，当前审批节点NodeUp为0；所以不需做判断
                var upflowTaskNodeEntityList = GetRejectFlowTaskOperatorEntity(flowTaskNodeEntityList, flowTaskNodeEntity, approversProperties);
                if (upflowTaskNodeEntityList.Count == 0)
                {
                    throw new Exception("当前流程不经过该驳回节点!");
                }
                foreach (var item in upflowTaskNodeEntityList)
                {
                    await AddFlowTaskOperatorEntityByAssigneeType(nextFlowTaskOperatorEntityList, flowTaskNodeEntityList, flowTaskNodeEntity, item, flowTaskEntity.CreatorUserId, fromData);
                }

                flowTaskEntity.Completion = upflowTaskNodeEntityList.Select(x => x.NodePropertyJson.Deserialize<ApproversProperties>().progress.ToInt()).ToList().Min();
                //修改驳回节点完成状态
                if (flowTaskNodeEntity.NodeUp == "1")
                {
                    flowTaskEntity.ThisStepId = GetThisStepIdReject(flowTaskNodeEntityList, upflowTaskNodeEntityList, flowTaskEntity.ThisStepId);
                    flowTaskEntity.ThisStep = GetThisStep(flowTaskNodeEntityList, flowTaskEntity.ThisStepId);
                    if (flowTaskEntity.ThisStep.Equals("开始"))
                    {
                        flowTaskEntity.Completion = 0;
                        flowTaskEntity.Status = FlowTaskStatusEnum.Reject;
                    }
                    //清空当前节点的候选人
                    _flowTaskRepository.DeleteFlowCandidates(new List<string>() { flowTaskNodeEntity.Id });
                }
                else
                {
                    flowTaskEntity.ThisStep = string.Join(",", upflowTaskNodeEntityList.Select(x => x.NodeName).ToArray());
                    flowTaskEntity.ThisStepId = string.Join(",", upflowTaskNodeEntityList.Select(x => x.NodeCode).ToArray());
                    GetAllNextNode(flowTaskNodeEntityList, upflowTaskNodeEntityList[0].NodeNext, upflowTaskNodeEntityList);
                    //清空驳回指定节点下所有节点的候选人
                    var candidateDelNodeIds = upflowTaskNodeEntityList.FindAll(x => !x.NodeCode.Equals(approversProperties.rejectStep)).Select(x => x.Id).ToList();
                    _flowTaskRepository.DeleteFlowCandidates(candidateDelNodeIds);
                }
                await RejectUpdateTaskNode(upflowTaskNodeEntityList, 0);
                var rejectNodeIds = upflowTaskNodeEntityList.Select(x => x.Id).ToArray();
                //删除驳回节点下所有经办
                var rejectList = _flowTaskRepository.GetTaskOperatorList(flowTaskEntity.Id, rejectNodeIds).Select(x => x.Id).ToList();
                await _flowTaskRepository.DeleteTaskOperator(rejectList);
                //删除驳回节点经办记录
                var rejectRecodeList = _flowTaskRepository.GetTaskOperatorRecordList(flowTaskEntity.Id, rejectNodeIds).Select(x => x.Id).ToList();
                await _flowTaskRepository.DeleteTaskOperatorRecord(rejectRecodeList);
            }
        }

        /// <summary>
        /// 获取同意节点经办
        /// </summary>
        /// <param name="flowTaskNodeEntity">当前节点</param>
        /// <param name="nextNode">下个节点</param>
        /// <param name="nextNodeCodeList">下个节点编码list</param>
        /// <param name="nextNodeCompletion">下个节点完成度list</param>
        /// <param name="nextFlowTaskOperatorEntityList">下个节点经办list</param>
        /// <param name="flowTaskNodeEntityList">所有接地那</param>
        /// <param name="flowTaskEntity">当前任务</param>
        /// <param name="fromData">表单数据</param>
        /// <returns></returns>
        private async Task GetNextOperatorByYes(FlowTaskNodeEntity flowTaskNodeEntity, FlowTaskNodeEntity nextNode, List<string> nextNodeCodeList, List<int> nextNodeCompletion, List<FlowTaskOperatorEntity> nextFlowTaskOperatorEntityList, List<FlowTaskNodeEntity> flowTaskNodeEntityList, FlowTaskEntity flowTaskEntity, string fromData)
        {
            flowTaskNodeEntity.Completion = 1;
            nextNodeCodeList.Add(nextNode.NodeCode);
            nextNodeCompletion.Add(nextNode.NodePropertyJson.Deserialize<ApproversProperties>().progress.ToInt());
            await AddFlowTaskOperatorEntityByAssigneeType(nextFlowTaskOperatorEntityList, flowTaskNodeEntityList, flowTaskNodeEntity, nextNode, flowTaskEntity.CreatorUserId, fromData);
        }

        /// <summary>
        /// 递归获取加签人
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flowTaskOperatorEntities"></param>
        /// <returns></returns>
        private async Task<List<FlowTaskOperatorEntity>> GetOperator(string id, List<FlowTaskOperatorEntity> flowTaskOperatorEntities)
        {
            var childEntity = await _flowTaskRepository.GetTaskOperatorInfoByParentId(id);
            if (childEntity.IsNotEmptyOrNull())
            {
                childEntity.State = "-1";
                flowTaskOperatorEntities.Add(childEntity);
                return await GetOperator(childEntity.Id, flowTaskOperatorEntities);
            }
            else
            {
                return flowTaskOperatorEntities;
            }
        }

        /// <summary>
        /// 对审批人节点分组
        /// </summary>
        /// <param name="flowTaskOperatorEntities"></param>
        /// <returns></returns>
        private Dictionary<string, List<FlowTaskOperatorEntity>> GroupByOperator(List<FlowTaskOperatorEntity> flowTaskOperatorEntities)
        {
            var dic = new Dictionary<string, List<FlowTaskOperatorEntity>>();
            var modelsGroup = flowTaskOperatorEntities.GroupBy(x => x.TaskNodeId).ToList();
            foreach (var item in modelsGroup)
            {
                dic.Add(item.Key, flowTaskOperatorEntities.FindAll(x => x.TaskNodeId == item.Key));
            }
            return dic;
        }

        /// <summary>
        /// 保存当前未完成节点下个候选人节点的候选人
        /// </summary>
        /// <param name="nodeEntityList"></param>
        /// <param name="candidateList"></param>
        /// <param name="taskOperatorId">0:发起节点，其他：审批节点</param>
        private List<FlowCandidatesEntity> SaveNodeCandidates(List<FlowTaskNodeEntity> nodeEntityList, Dictionary<string, List<string>> candidateList, string taskOperatorId)
        {
            var flowCandidateList = new List<FlowCandidatesEntity>();
            if (candidateList.IsNotEmptyOrNull())
            {
                foreach (var item in candidateList.Keys)
                {
                    var node = nodeEntityList.Find(x => x.NodeCode == item);
                    if (node != null)
                    {
                        flowCandidateList.Add(new FlowCandidatesEntity()
                        {
                            Id = YitIdHelper.NextId().ToString(),
                            TaskId = node.TaskId,
                            TaskNodeId = node.Id,
                            HandleId = _userManager.UserId,
                            Account = _userManager.Account,
                            Candidates = string.Join(",", candidateList[item]),
                            TaskOperatorId = taskOperatorId
                        });
                    }
                    var dic = new Dictionary<string, List<string>>();
                    if (node.Candidates.IsNotEmptyOrNull())
                    {
                        dic = node.Candidates.ToObject<Dictionary<string, List<string>>>();
                        dic[_userManager.Account] = candidateList[item];
                    }
                    else
                    {
                        dic[_userManager.Account] = candidateList[item];
                    }
                    node.Candidates = dic.ToJson();
                }
                _flowTaskRepository.CreateFlowCandidates(flowCandidateList);
            }
            return flowCandidateList;
        }
        #endregion

        #region 子流程处理
        /// <summary>
        /// 创建子流程任务
        /// </summary>
        /// <param name="childTaskPro">子流程节点属性</param>
        /// <param name="formData">表单数据</param>
        /// <param name="parentId">子任务父id</param>
        /// <param name="childTaskCrUsers">子任务创建人</param>
        private async Task<List<string>> CreateSubProcesses(ChildTaskProperties childTaskPro, object formData, string parentId, List<string> childTaskCrUsers)
        {
            var childFLowEngine = await _flowEngineService.GetInfo(childTaskPro.flowId);
            var isSysTable = childFLowEngine.FormType == 1;
            var childTaskId = new List<string>();
            var bodyDic = new Dictionary<string, object>();
            foreach (var item in childTaskCrUsers)
            {
                var prossId = isSysTable ? YitIdHelper.NextId().ToString() : null;
                var title = isSysTable ? _usersService.GetInfoByUserId(item).RealName + "的" + childFLowEngine.FullName : null;
                var childTaskEntity = await this.Save(null, childTaskPro.flowId, prossId, title, 0, null, formData, 1, 0, isSysTable, parentId, item, false, childTaskPro.isAsync);
                childTaskId.Add(childTaskEntity.Id);
                if (isSysTable)
                {
                    GetSysTableFromService(childFLowEngine.EnCode, formData, childTaskEntity.Id, 1);
                }

                bodyDic.Add(item, new
                {
                    enCode = childFLowEngine.EnCode,
                    flowId = childFLowEngine.Id,
                    formType = childFLowEngine.FormType,
                    status = 0,
                    processId = childTaskEntity.Id,
                    taskNodeId = "",
                    taskOperatorId = "",
                    type = 1
                });
                await StationLetterMsg(title, new List<string>() { item }, 4, bodyDic);
            }
            return childTaskId;
        }

        /// <summary>
        /// 获取子流程继承父流程的表单数据
        /// </summary>
        /// <param name="childTaskProperties"></param>
        /// <param name="formData"></param>
        /// <param name="isSysTable"></param>
        /// <returns></returns>
        private async Task<object> GetSubFlowFormData(ChildTaskProperties childTaskProperties, string formData)
        {
            var childFlowEngine = await _flowEngineService.GetInfo(childTaskProperties.flowId);
            var parentDic = formData.ToObject().ToObject<Dictionary<string, object>>();
            var isSysTable = childFlowEngine.FormType == 1;
            var childDic = new Dictionary<string, object>();
            if (isSysTable)
            {
                childDic["flowId"] = childTaskProperties.flowId;
            }
            else
            {
                //表单模板list
                List<FieldsModel> fieldsModelList = childFlowEngine.FormTemplateJson.Deserialize<FormDataModel>().fields;
                //剔除布局控件
                fieldsModelList = _runService.TemplateDataConversion(fieldsModelList);
                foreach (var item in fieldsModelList)
                {
                    childDic[item.__vModel__] = "";
                }
            }
            foreach (var item in childTaskProperties.assignList)
            {
                childDic[item.childField] = parentDic.ContainsKey(item.parentField) ? parentDic[item.parentField] : null;
            }
            return childDic;
        }

        /// <summary>
        /// 插入子流程
        /// </summary>
        /// <param name="childFlowTaskEntity">子流程</param>
        /// <returns></returns>
        private async Task InsertSubFlowNextNode(FlowTaskEntity childFlowTaskEntity)
        {
            try
            {
                //所有子流程(不包括当前流程)
                var childFlowTaskAll = (await _flowTaskRepository.GetTaskList()).FindAll(x => x.ParentId == childFlowTaskEntity.ParentId && x.Id != childFlowTaskEntity.Id && x.IsAsync == 0);
                //已完成的子流程
                var completeChildFlow = childFlowTaskAll.FindAll(x => x.Status == FlowTaskStatusEnum.Adopt);
                //父流程
                var parentFlowTask = await _flowTaskRepository.GetTaskInfo(childFlowTaskEntity.ParentId);
                if (childFlowTaskAll.Count == completeChildFlow.Count)
                {
                    var parentSubFlowNode = (await _flowTaskRepository.GetTaskNodeList(parentFlowTask.Id)).Find(x => x.NodeType.Equals("subFlow") && x.Completion == 0 && x.NodePropertyJson.Deserialize<ChildTaskProperties>().childTaskId.Contains(childFlowTaskEntity.Id));
                    var subFlowOperator = parentSubFlowNode.Adapt<FlowTaskOperatorEntity>();
                    subFlowOperator.Id = null;
                    subFlowOperator.TaskNodeId = parentSubFlowNode.Id;
                    var formType = (await _flowEngineService.GetInfo(parentFlowTask.FlowId)).FormType;
                    var childData = parentSubFlowNode.NodePropertyJson.ToObject<ChildTaskProperties>().formData;
                    var handleModel = new FlowHandleModel();
                    if (formType == 2)
                    {
                        var dic = new Dictionary<string, object>();
                        dic.Add("data", childData);
                        handleModel.formData = dic;
                    }
                    else
                    {
                        handleModel.formData = childData.ToObject();
                    }
                    await this.Audit(parentFlowTask, subFlowOperator, handleModel, (int)formType);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        #region 其他
        /// <summary>
        /// 处理填写数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task<FlowTaskInfoOutput> GetFlowDynamicDataManage(FlowTaskEntity entity)
        {
            try
            {
                var flowEngineEntity = await _flowEngineService.GetInfo(entity.FlowId);
                var flowEngineTablesModelList = flowEngineEntity.Tables.Deserialize<List<FlowEngineTablesModel>>();
                FlowTaskInfoOutput output = entity.Adapt<FlowTaskInfoOutput>();
                var visualDevEntity = flowEngineEntity.Adapt<VisualDevEntity>();
                visualDevEntity.FormData = flowEngineEntity.FormTemplateJson;
                if (flowEngineTablesModelList.Count > 0)
                {
                    var outData = await _runService.GetHaveTableInfo(entity.Id, visualDevEntity);
                    output.data = outData;
                }
                else
                {
                    //真实表单数据
                    Dictionary<string, object> formDataDic = entity.FlowFormContentJson.ToObject<Dictionary<string, object>>();
                    var data = await _runService.GetIsNoTableInfo(visualDevEntity, entity.FlowFormContentJson);
                    output.data = data;
                }
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 添加经办记录
        /// </summary>
        /// <param name="flowTaskOperatorEntity">当前经办</param>
        /// <param name="flowHandleModel">审批数据</param>
        /// <param name="hanldState"></param>
        /// <returns></returns>
        private async Task CreateOperatorRecode(FlowTaskOperatorEntity flowTaskOperatorEntity, FlowHandleModel flowHandleModel, int hanldState)
        {
            FlowTaskOperatorRecordEntity flowTaskOperatorRecordEntity = new FlowTaskOperatorRecordEntity();
            flowTaskOperatorRecordEntity.HandleOpinion = flowHandleModel.handleOpinion;
            flowTaskOperatorRecordEntity.HandleId = _userManager.UserId;
            flowTaskOperatorRecordEntity.HandleTime = DateTime.Now;
            flowTaskOperatorRecordEntity.HandleStatus = hanldState;
            flowTaskOperatorRecordEntity.NodeCode = flowTaskOperatorEntity.NodeCode;
            flowTaskOperatorRecordEntity.NodeName = flowTaskOperatorEntity.NodeName;
            flowTaskOperatorRecordEntity.TaskOperatorId = flowTaskOperatorEntity.Id;
            flowTaskOperatorRecordEntity.TaskNodeId = flowTaskOperatorEntity.TaskNodeId;
            flowTaskOperatorRecordEntity.TaskId = flowTaskOperatorEntity.TaskId;
            flowTaskOperatorRecordEntity.SignImg = flowHandleModel.signImg;
            flowTaskOperatorRecordEntity.Status = flowTaskOperatorEntity.ParentId.IsNotEmptyOrNull() ? 1 : 0;
            await _flowTaskRepository.CreateTaskOperatorRecord(flowTaskOperatorRecordEntity);
        }

        /// <summary>
        /// 验证有效状态
        /// </summary>
        /// <param name="status">状态编码</param>
        /// <returns></returns>
        private bool CheckStatus(int? status)
        {
            if (status == FlowTaskStatusEnum.Draft || status == FlowTaskStatusEnum.Reject || status == FlowTaskStatusEnum.Revoke)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断会签人数是否达到会签比例
        /// </summary>
        /// <param name="thisFlowTaskOperatorEntityList">当前节点所有审批人(已剔除加签人)</param>
        /// <param name="handStatus">审批状态</param>
        /// <param name="index">比例</param>
        /// <param name="delCount">委托人数</param>
        /// <param name="hasFreeApprover">是否有加签(true：没有，flase：有)</param>
        /// <returns></returns>
        private bool IsAchievebilProportion(List<FlowTaskOperatorEntity> thisFlowTaskOperatorEntityList, int handStatus, int index, int delCount, bool hasFreeApprover)
        {
            if (handStatus == 0)
                index = 100 - index;
            //完成人数（加上当前审批人）
            var comIndex = thisFlowTaskOperatorEntityList.FindAll(x => x.HandleStatus == handStatus && x.Completion == 1 && x.State == "0").Count.ToDouble();
            if (hasFreeApprover)
            {
                comIndex = comIndex + 1;
            }
            //完成比例
            var comProportion = (comIndex / thisFlowTaskOperatorEntityList.Count.ToDouble() * 100).ToInt();
            return comProportion >= index;
        }

        /// <summary>
        /// 事件请求
        /// </summary>
        /// <param name="funcConfig"></param>
        /// <param name="formdata"></param>
        /// <returns></returns>
        private async Task RequestEvents(FuncConfig funcConfig, string formdata)
        {
            if (funcConfig.IsNotEmptyOrNull() && funcConfig.on && funcConfig.interfaceId.IsNotEmptyOrNull())
            {
                var parameters = GetMsgContent(funcConfig.templateJson, formdata, new MessageTemplateEntity());
                await _dataInterfaceService.GetResponseByType(funcConfig.interfaceId, 3, _userManager.TenantId, null, parameters);
            }
        }

        #region 消息推送
        /// <summary>
        /// 站内消息推送
        /// </summary>
        /// <param name="fullName">任务名</param>
        /// <param name="users">通知人员</param>
        /// <param name="msgType">消息类型【0.审批，1.同意，2.拒绝，3.抄送，4.子流程，5.结束】</param>
        /// <param name="pairs">详情跳转json</param>
        /// <returns></returns>
        private async Task StationLetterMsg(string fullName, List<string> users, int msgType, Dictionary<string, object> pairs)
        {
            var msgTemplateEntity = new MessageTemplateEntity();
            switch (msgType)
            {
                case 1:
                    msgTemplateEntity.Title = fullName + "已被【同意】";
                    break;
                case 2:
                    msgTemplateEntity.Title = fullName + "已被【拒绝】";
                    break;
                case 3:
                    msgTemplateEntity.Title = fullName + "已被【抄送】";
                    break;
                case 4:
                    msgTemplateEntity.Title = fullName + "【子流程】已被【发起】";
                    break;
                case 5:
                    msgTemplateEntity.Title = fullName + "已【结束】";
                    break;
                default:
                    msgTemplateEntity.Title = fullName;
                    break;
            }
            await _messageTemplateService.SendNodeMessage(new List<string>() { "1" }, msgTemplateEntity, users, null, pairs);
        }


        /// <summary>
        /// 通过消息模板获取消息通知
        /// </summary>
        /// <param name="msgConfig"></param>
        /// <param name="users"></param>
        /// <param name="formdata"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        private async Task Alerts(MsgConfig msgConfig, List<string> users, string formdata)
        {
            if (msgConfig.IsNotEmptyOrNull() && msgConfig.on != 0 && msgConfig.msgId.IsNotEmptyOrNull())
            {
                var msgTemplateEntity = await _messageTemplateService.GetInfo(msgConfig.msgId);
                var typeList = new List<string>();
                var parameters = GetMsgContent(msgConfig.templateJson, formdata, msgTemplateEntity);
                if (msgTemplateEntity.IsEmail == 1)
                    typeList.Add("2");
                if (msgTemplateEntity.IsSms == 1)
                    typeList.Add("3");
                if (msgTemplateEntity.IsDingTalk == 1)
                    typeList.Add("4");
                if (msgTemplateEntity.IsWeCom == 1)
                    typeList.Add("5");
                await _messageTemplateService.SendNodeMessage(typeList, msgTemplateEntity, users, parameters, null);
            }
        }

        /// <summary>
        /// 获取消息模板内容
        /// </summary>
        /// <param name="templateJsonItems">消息模板json</param>
        /// <param name="formData">表单数据</param>
        /// <param name="messageTemplateEntity">消息模板</param>
        /// <returns></returns>
        private Dictionary<string, string> GetMsgContent(List<TemplateJsonItem> templateJsonItems, string formData, MessageTemplateEntity messageTemplateEntity)
        {
            var jObj = formData.Deserialize<JObject>();
            var dic = new Dictionary<string, string>();
            var taskEntity = _flowTaskRepository.GetTaskInfoNoAsync(taskId);
            foreach (var item in templateJsonItems)
            {
                var value = "";
                if (item.relationField.Equals("hszFlowOperatorId"))
                {
                    value = _userManager.UserId;
                }
                else if (item.relationField.Equals("hszTaskId"))
                {
                    value = taskId;
                }
                else if (item.relationField.Equals("hszTaskNodeId"))
                {
                    value = taskNodeId;
                }
                else if (item.relationField.Equals("hszTaskFullName"))
                {
                    value = taskEntity.FullName;
                }
                else if (item.relationField.Equals("hszLaunchUserId"))
                {
                    value = taskEntity.CreatorUserId;
                }
                else if (item.relationField.Equals("hszLaunchUserName"))
                {
                    value = _usersService.GetInfoByUserId(taskEntity.CreatorUserId).RealName;
                }
                else if (item.relationField.Equals("hszFlowOperatorUserName"))
                {
                    value = _userManager.User.RealName;
                }
                else
                {
                    value = jObj.ContainsKey(item.relationField) ? jObj[item.relationField].ToString() : "";
                }
                messageTemplateEntity.Title = messageTemplateEntity.Title.Replace("{" + item.field + "}", value);
                messageTemplateEntity.Content = messageTemplateEntity.Content.Replace("{" + item.field + "}", value);
                dic.Add(item.field, value);
            }
            return dic;
        }

        /// <summary>
        /// 组装消息跳转详情参数
        /// </summary>
        /// <param name="flowEngineEntity"></param>
        /// <param name="taskNodeId"></param>
        /// <param name="userList"></param>
        /// <param name="flowTaskOperatorEntities"></param>
        /// <param name="type">1:发起，2：待办，3：抄送</param>
        /// <param name="taskOperatorId"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetMesBodyText(FlowEngineEntity flowEngineEntity, string taskNodeId, List<string> userList, List<FlowTaskOperatorEntity> flowTaskOperatorEntities, int type, string taskOperatorId = "")
        {
            var dic = new Dictionary<string, object>();
            if (flowTaskOperatorEntities.IsNotEmptyOrNull() && flowTaskOperatorEntities.Count > 0)
            {
                foreach (var item in flowTaskOperatorEntities)
                {
                    var value = new
                    {
                        enCode = flowEngineEntity.EnCode,
                        flowId = flowEngineEntity.Id,
                        formType = flowEngineEntity.FormType,
                        status = type == 1 ? 0 : 1,
                        processId = item.TaskId,
                        taskNodeId = item.TaskNodeId,
                        taskOperatorId = item.Id,
                        type = type
                    };
                    dic.Add(item.HandleId, value);
                }
            }
            else
            {
                var value = new
                {
                    enCode = flowEngineEntity.EnCode,
                    flowId = flowEngineEntity.Id,
                    formType = flowEngineEntity.FormType,
                    status = type == 1 ? 0 : 1,
                    processId = taskId,
                    taskNodeId = taskNodeId,
                    taskOperatorId = taskOperatorId,
                    type = type
                };
                userList.ForEach(u => dic.Add(u, value));
            }
            return dic;
        }
        #endregion

        #region 系统表单
        /// <summary>
        /// 系统表单操作
        /// </summary>
        /// <param name="enCode"></param>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        private void GetSysTableFromService(string enCode, object data, string id, int type)
        {
            Scoped.Create((_, scope) =>
            {
                switch (enCode.ToLower())
                {
                    case "applybanquet":
                        var ApplyBanquet = App.GetService<IApplyBanquetService>();
                        ApplyBanquet.Save(id, data, type);
                        break;
                    case "quotationapproval":
                        var QuotationApproval = App.GetService<IQuotationApprovalService>();
                        QuotationApproval.Save(id, data, type);
                        break;
                    case "paydistribution":
                        var PayDistribution = App.GetService<IPayDistributionService>();
                        PayDistribution.Save(id, data, type);
                        break;
                    case "contractapprovalsheet":
                        var ContractApprovalSheet = App.GetService<IContractApprovalSheetService>();
                        ContractApprovalSheet.Save(id, data, type);
                        break;
                    case "salesorder":
                        var SalesOrder = App.GetService<ISalesOrderService>();
                        SalesOrder.Save(id, data, type);
                        break;
                    case "paymentapply":
                        var PaymentApply = App.GetService<IPaymentApplyService>();
                        PaymentApply.Save(id, data, type);
                        break;
                    case "finishedproduct":
                        var FinishedProduct = App.GetService<IFinishedProductService>();
                        FinishedProduct.Save(id, data, type);
                        break;
                    case "archivalborrow":
                        var ArchivalBorrow = App.GetService<IArchivalBorrowService>();
                        ArchivalBorrow.Save(id, data, type);
                        break;
                    case "applydelivergoods":
                        var ApplyDeliverGoods = App.GetService<IApplyDeliverGoodsService>();
                        ApplyDeliverGoods.Save(id, data, type);
                        break;
                    case "applymeeting":
                        var ApplyMeeting = App.GetService<IApplyMeetingService>();
                        ApplyMeeting.Save(id, data, type);
                        break;
                    case "articleswarehous":
                        var ArticlesWarehous = App.GetService<IArticlesWarehousService>();
                        ArticlesWarehous.Save(id, data, type);
                        break;
                    case "batchpack":
                        var BatchPack = App.GetService<IBatchPackService>();
                        BatchPack.Save(id, data, type);
                        break;
                    case "batchtable":
                        var BatchTable = App.GetService<IBatchTableService>();
                        BatchTable.Save(id, data, type);
                        break;
                    case "conbilling":
                        var ConBilling = App.GetService<IConBillingService>();
                        ConBilling.Save(id, data, type);
                        break;
                    case "contractapproval":
                        var ContractApproval = App.GetService<IContractApprovalService>();
                        ContractApproval.Save(id, data, type);
                        break;
                    case "debitbill":
                        var DebitBill = App.GetService<IDebitBillService>();
                        DebitBill.Save(id, data, type);
                        break;
                    case "documentapproval":
                        var DocumentApproval = App.GetService<IDocumentApprovalService>();
                        DocumentApproval.Save(id, data, type);
                        break;
                    case "documentsigning":
                        var DocumentSigning = App.GetService<IDocumentSigningService>();
                        DocumentSigning.Save(id, data, type);
                        break;
                    case "expenseexpenditure":
                        var ExpenseExpenditure = App.GetService<IExpenseExpenditureService>();
                        ExpenseExpenditure.Save(id, data, type);
                        break;
                    case "incomerecognition":
                        var IncomeRecognition = App.GetService<IIncomeRecognitionService>();
                        IncomeRecognition.Save(id, data, type);
                        break;
                    case "leaveapply":
                        var LeaveApply = App.GetService<ILeaveApplyService>();
                        LeaveApply.Save(id, data, type);
                        break;
                    case "letterservice":
                        var LetterService = App.GetService<ILetterServiceService>();
                        LetterService.Save(id, data, type);
                        break;
                    case "materialrequisition":
                        var MaterialRequisition = App.GetService<IMaterialRequisitionService>();
                        MaterialRequisition.Save(id, data, type);
                        break;
                    case "monthlyreport":
                        var MonthlyReport = App.GetService<IMonthlyReportService>();
                        MonthlyReport.Save(id, data, type);
                        break;
                    case "officesupplies":
                        var OfficeSupplies = App.GetService<IOfficeSuppliesService>();
                        OfficeSupplies.Save(id, data, type);
                        break;
                    case "outboundorder":
                        var OutboundOrder = App.GetService<IOutboundOrderService>();
                        OutboundOrder.Save(id, data, type);
                        break;
                    case "outgoingapply":
                        var OutgoingApply = App.GetService<IOutgoingApplyService>();
                        OutgoingApply.Save(id, data, type);
                        break;
                    case "postbatchtab":
                        var PostBatchTab = App.GetService<IPostBatchTabService>();
                        PostBatchTab.Save(id, data, type);
                        break;
                    case "procurementmaterial":
                        var ProcurementMaterial = App.GetService<IProcurementMaterialService>();
                        ProcurementMaterial.Save(id, data, type);
                        break;
                    case "purchaselist":
                        var PurchaseList = App.GetService<IPurchaseListService>();
                        PurchaseList.Save(id, data, type);
                        break;
                    case "receiptprocessing":
                        var ReceiptProcessing = App.GetService<IReceiptProcessingService>();
                        ReceiptProcessing.Save(id, data, type);
                        break;
                    case "receiptsign":
                        var ReceiptSign = App.GetService<IReceiptSignService>();
                        ReceiptSign.Save(id, data, type);
                        break;
                    case "rewardpunishment":
                        var RewardPunishment = App.GetService<IRewardPunishmentService>();
                        RewardPunishment.Save(id, data, type);
                        break;
                    case "salessupport":
                        var SalesSupport = App.GetService<ISalesSupportService>();
                        SalesSupport.Save(id, data, type);
                        break;
                    case "staffovertime":
                        var StaffOvertime = App.GetService<IStaffOvertimeService>();
                        StaffOvertime.Save(id, data, type);
                        break;
                    case "supplementcard":
                        var SupplementCard = App.GetService<ISupplementCardService>();
                        SupplementCard.Save(id, data, type);
                        break;
                    case "travelapply":
                        var TravelApply = App.GetService<ITravelApplyService>();
                        TravelApply.Save(id, data, type);
                        break;
                    case "travelreimbursement":
                        var TravelReimbursement = App.GetService<ITravelReimbursementService>();
                        TravelReimbursement.Save(id, data, type);
                        break;
                    case "vehicleapply":
                        var VehicleApply = App.GetService<IVehicleApplyService>();
                        VehicleApply.Save(id, data, type);
                        break;
                    case "violationhandling":
                        var ViolationHandling = App.GetService<IViolationHandlingService>();
                        ViolationHandling.Save(id, data, type);
                        break;
                    case "warehousereceipt":
                        var WarehouseReceipt = App.GetService<IWarehouseReceiptService>();
                        WarehouseReceipt.Save(id, data, type);
                        break;
                    case "workcontactsheet":
                        var WorkContactSheet = App.GetService<IWorkContactSheetService>();
                        WorkContactSheet.Save(id, data, type);
                        break;
                    default:
                        break;
                }
            });
        }
        #endregion
        #endregion
        #endregion
    }
}