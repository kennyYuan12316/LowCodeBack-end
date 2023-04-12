using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.WorkFlow.Entitys.Dto.FlowBefore;
using HSZ.WorkFlow.Entitys.Model;
using HSZ.WorkFlow.Interfaces.FlowEngine;
using HSZ.WorkFlow.Interfaces.FlowTask;
using HSZ.WorkFlow.Interfaces.FlowTask.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSZ.WorkFlow.Core.Service.FlowTask
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：流程审批
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowEngine", Name = "FlowBefore", Order = 303)]
    [Route("api/workflow/Engine/[controller]")]
    public class FlowBeforeService : IDynamicApiController, ITransient
    {
        private readonly IFlowTaskRepository _flowTaskRepository;
        private readonly IFlowTaskService _flowTaskService;
        private readonly IFlowEngineService _flowEngineService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowTaskRepository"></param>
        /// <param name="flowTaskService"></param>
        /// <param name="flowEngineService"></param>
        public FlowBeforeService(IFlowTaskRepository flowTaskRepository, IFlowTaskService flowTaskService, IFlowEngineService flowEngineService)
        {
            _flowTaskRepository = flowTaskRepository;
            _flowTaskService = flowTaskService;
            _flowEngineService = flowEngineService;
        }

        #region Get
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <param name="category">分类</param>
        /// <returns></returns>
        [HttpGet("List/{category}")]
        public async Task<dynamic> GetList([FromQuery] FlowBeforeListQuery input, string category)
        {
            try
            {
                switch (category)
                {
                    case "1":
                        return await _flowTaskRepository.GetWaitList(input);
                    case "2":
                        return await _flowTaskRepository.GetTrialList(input);
                    case "3":
                        return await _flowTaskRepository.GetCirculateList(input);
                    case "4":
                        return await _flowTaskRepository.GetBatchWaitList(input);
                    default:
                        break;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taskNodeId"></param>
        /// <param name="taskOperatorId"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id, [FromQuery] string taskNodeId, [FromQuery] string taskOperatorId)
        {
            try
            {
                var output = await _flowTaskService.GetFlowBeforeInfo(id, taskNodeId, taskOperatorId);
                return output;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 审批汇总
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="category">分类（1：部门，2：角色，3：岗位）</param>
        /// <returns></returns>
        [HttpGet("RecordList/{id}")]
        public async Task<dynamic> GetRecordList(string id,[FromQuery] string category, [FromQuery] string type)
        {
            var recordList = await _flowTaskRepository.GetRecordListByCategory(id,category, type);
            var categoryId = recordList.Select(x=>x.category).Distinct().ToList();
            var list = new List<FlowBeforeRecordListOutput>();
            foreach (var item in categoryId)
            {
                var categoryList = recordList.FindAll(x=>x.category==item).ToList();
                var output = new FlowBeforeRecordListOutput();
                output.fullName = categoryList.FirstOrDefault().categoryName;
                output.list = categoryList.OrderByDescending(x=>x.handleTime).ToList();
                list.Add(output);
            }
            return list;
        }

        /// <summary>
        /// 获取候选人编码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        [HttpPost("Candidates/{id}")]
        public async Task<dynamic> Candidates(string id, [FromBody] FlowHandleModel flowHandleModel)
        {
            return await _flowTaskService.GetCandidateModelList(id,flowHandleModel);
        }

        /// <summary>
        /// 获取候选人
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        [HttpPost("CandidateUser/{id}")]
        public async Task<dynamic> CandidateUser(string id, [FromBody] FlowHandleModel flowHandleModel)
        {
            return await _flowTaskService.GetCandidateModelList(id, flowHandleModel,1);
        }

        /// <summary>
        /// 批量流程列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("BatchFlowSelector")]
        public async Task<dynamic> BatchFlowSelector()
        {
            return await _flowTaskRepository.BatchFlowSelector();
        }

        /// <summary>
        /// 节点列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("NodeSelector/{id}")]
        public async Task<dynamic> NodeSelector(string id)
        {
            return await _flowTaskService.NodeSelector(id);
        }

        /// <summary>
        /// 批量审批候选人
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="taskOperatorId"></param>
        /// <returns></returns>
        [HttpGet("BatchCandidate")]
        public async Task<dynamic> GetBatchCandidate([FromQuery]string flowId,[FromQuery]string taskOperatorId)
        {
            return await _flowTaskService.GetBatchCandidate(flowId, taskOperatorId);
        }
        #endregion

        #region POST
        /// <summary>
        /// 审核同意
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="flowHandleModel">流程经办</param>
        /// <returns></returns>
        [HttpPost("Audit/{id}")]
        [SqlSugarUnitOfWork]
        public async Task Audit(string id, [FromBody] FlowHandleModel flowHandleModel)
        {
            var flowTaskOperatorEntity = await _flowTaskRepository.GetTaskOperatorInfo(id);
            var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(flowTaskOperatorEntity.TaskId);
            var flowEngine = await _flowEngineService.GetInfo(flowTaskEntity.FlowId);
            if (flowTaskOperatorEntity == null)
                throw HSZException.Oh(ErrorCode.COM1005);
            if (flowTaskOperatorEntity.Completion != 0)
                throw HSZException.Oh(ErrorCode.WF0006);
            await _flowTaskService.AdjustNodeByCon(flowEngine, flowHandleModel.formData, flowTaskEntity.Id);
            await _flowTaskService.Audit(flowTaskEntity, flowTaskOperatorEntity, flowHandleModel, (int)flowEngine.FormType);
            await _flowTaskService.ApproveBefore(flowEngine, flowTaskEntity, flowHandleModel);
        }

        /// <summary>
        /// 审核拒绝
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="flowHandleModel">流程经办</param>
        /// <returns></returns>
        [HttpPost("Reject/{id}")]
        public async Task Reject(string id, [FromBody] FlowHandleModel flowHandleModel)
        {
            var flowTaskOperatorEntity = await _flowTaskRepository.GetTaskOperatorInfo(id);
            var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(flowTaskOperatorEntity.TaskId);
            var flowEngine = await _flowEngineService.GetInfo(flowTaskEntity.FlowId);
            if (flowTaskOperatorEntity == null)
                throw HSZException.Oh(ErrorCode.COM1005);
            if (flowTaskOperatorEntity.Completion != 0)
                throw HSZException.Oh(ErrorCode.WF0006);
            if (_flowTaskRepository.AnySubFlowTask(flowTaskOperatorEntity.TaskId))
                throw HSZException.Oh(ErrorCode.WF0019);
            if (await _flowTaskService.IsSubFlowUpNode(flowTaskOperatorEntity))
                throw HSZException.Oh(ErrorCode.WF0019);
            flowTaskEntity = await _flowTaskRepository.GetTaskInfo(flowTaskOperatorEntity.TaskId);
            await _flowTaskService.Reject(flowTaskEntity, flowTaskOperatorEntity, flowHandleModel, (int)flowEngine.FormType);
        }
        /// <summary>
        /// 撤回审核
        /// 注意：在撤销流程时要保证你的下一节点没有处理这条记录；如已处理则无法撤销流程。
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="flowHandleModel">流程经办</param>
        /// <returns></returns>
        [HttpPost("Recall/{id}")]
        public async Task Recall(string id, [FromBody] FlowHandleModel flowHandleModel)
        {
            var flowTaskOperatorRecord = await _flowTaskRepository.GetTaskOperatorRecordInfo(id);
            if (_flowTaskRepository.AnySubFlowTask(flowTaskOperatorRecord.TaskId))
                throw HSZException.Oh(ErrorCode.WF0018);
            await _flowTaskService.Recall(id, flowHandleModel);
        }

        /// <summary>
        /// 终止审核
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="flowHandleModel">流程经办</param>
        /// <returns></returns>
        [HttpPost("Cancel/{id}")]
        public async Task Cancel(string id, [FromBody] FlowHandleModel flowHandleModel)
        {
            if (_flowTaskRepository.AnySubFlowTask(id))
                throw HSZException.Oh(ErrorCode.WF0017);
            var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(id);
            if (!flowTaskEntity.ParentId.Equals("0") && flowTaskEntity.ParentId.IsNotEmptyOrNull())
                throw HSZException.Oh(ErrorCode.WF0015);
            if (flowTaskEntity.FlowType == 1)
                throw HSZException.Oh(ErrorCode.WF0016);
            await _flowTaskService.Cancel(flowTaskEntity, flowHandleModel);
        }

        /// <summary>
        /// 转办
        /// </summary>
        /// <param name="id">流程经办主键id</param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        [HttpPost("Transfer/{id}")]
        public async Task Transfer(string id, [FromBody] FlowHandleModel flowHandleModel)
        {
            await _flowTaskService.Transfer(id, flowHandleModel);
        }

        /// <summary>
        /// 指派
        /// </summary>
        /// <param name="id">流程经办主键id</param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        [HttpPost("Assign/{id}")]
        public async Task Assigned(string id, [FromBody] FlowHandleModel flowHandleModel)
        {
            var nodeEntity = (await _flowTaskRepository.GetTaskNodeList(id)).Find(x => x.State.Equals("0") && x.NodeType.Equals("subFlow")&&x.NodeCode.Equals(flowHandleModel.nodeCode));
            if (nodeEntity.IsNotEmptyOrNull())
                throw HSZException.Oh(ErrorCode.WF0014);
            await _flowTaskService.Assigned(id, flowHandleModel);
        }

        /// <summary>
        /// 保存审批草稿数据
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="flowHandleModel">流程经办</param>
        /// <returns></returns>
        [HttpPost("SaveAudit/{id}")]
        [SqlSugarUnitOfWork]
        public async Task SaveAudit(string id, [FromBody] FlowHandleModel flowHandleModel)
        {
            var flowTaskOperatorEntity = await _flowTaskRepository.GetTaskOperatorInfo(id);
            var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(flowTaskOperatorEntity.TaskId);
            var flowEngine = await _flowEngineService.GetInfo(flowTaskEntity.FlowId);
            if (flowTaskOperatorEntity == null)
                throw HSZException.Oh(ErrorCode.COM1005);
            flowTaskOperatorEntity.DraftData = flowEngine.FormType == 2 ? flowHandleModel.formData.Serialize().Deserialize<JObject>()["data"].ToString() : flowHandleModel.formData.Serialize();
            await _flowTaskRepository.UpdateTaskOperator(flowTaskOperatorEntity);
        }

        /// <summary>
        /// 批量审批
        /// </summary>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        [HttpPost("BatchOperation")]
        public async Task BatchOperation([FromBody] FlowHandleModel flowHandleModel)
        {
            foreach (var item in flowHandleModel.ids)
            {
                flowHandleModel.formData = await GetBatchOperationData(item);
                switch (flowHandleModel.batchType)
                {
                    case 0:
                        var flowTaskOperatorEntity = await _flowTaskRepository.GetTaskOperatorInfo(item);
                        var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(flowTaskOperatorEntity.TaskId);
                        var flowEngine = await _flowEngineService.GetInfo(flowTaskEntity.FlowId);
                        if (flowTaskOperatorEntity == null)
                            throw HSZException.Oh(ErrorCode.COM1005);
                        if (flowTaskOperatorEntity.Completion != 0)
                            throw HSZException.Oh(ErrorCode.WF0006);
                        await _flowTaskService.Audit(flowTaskEntity, flowTaskOperatorEntity, flowHandleModel, (int)flowEngine.FormType);
                        break;
                    case 1:
                        await Reject(item, flowHandleModel);
                        break;
                    case 2:
                        await Transfer(item, flowHandleModel);
                        break;
                    default:
                        break;
                }
            }
        }


        #endregion

        #region PrivateMethod
        /// <summary>
        /// 获取批量任务的表单数据
        /// </summary>
        /// <param name="taskOperatorId"></param>
        /// <returns></returns>
        private async Task<object> GetBatchOperationData(string taskOperatorId)
        {
            var taskOperatorEntity = await _flowTaskRepository.GetTaskOperatorInfo(taskOperatorId);
            var taskEntity=await _flowTaskRepository.GetTaskInfo(taskOperatorEntity.TaskId);
            var flowEngine = await _flowEngineService.GetInfo(taskEntity.FlowId);
            if (flowEngine.FormType==1)
            {
                return taskEntity.FlowFormContentJson.Deserialize<JObject>();
            }
            else
            {
                return new { flowId = taskEntity.Id, data = taskEntity.FlowFormContentJson, id = taskEntity.Id };
            }
        }
        #endregion
    }
}
