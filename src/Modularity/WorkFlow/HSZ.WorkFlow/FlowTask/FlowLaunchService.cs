using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.Message.Interfaces.Message;
using HSZ.WorkFlow.Entitys.Dto.FlowLaunch;
using HSZ.WorkFlow.Entitys.Enum;
using HSZ.WorkFlow.Entitys.Model.Properties;
using HSZ.WorkFlow.Interfaces.FlowTask;
using HSZ.WorkFlow.Interfaces.FlowTask.Repository;
using Microsoft.AspNetCore.Mvc;
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
    /// 描 述：流程发起
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowEngine", Name = "FlowLaunch", Order = 305)]
    [Route("api/workflow/Engine/[controller]")]
    public class FlowLaunchService : IDynamicApiController, ITransient
    {
        private readonly IFlowTaskRepository _flowTaskRepository;
        private readonly IFlowTaskService _flowTaskService;
        private readonly IMessageService _messageService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowTaskRepository"></param>
        public FlowLaunchService(IFlowTaskRepository flowTaskRepository, IFlowTaskService flowTaskService, IMessageService messageService)
        {
            _flowTaskRepository = flowTaskRepository;
            _flowTaskService = flowTaskService;
            _messageService = messageService;
        }

        #region GET
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] FlowLaunchListQuery input)
        {
            return await _flowTaskRepository.GetLaunchList(input);
        }
        #endregion

        #region POST
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _flowTaskRepository.GetTaskInfo(id);
            if (entity == null)
                throw HSZException.Oh(ErrorCode.COM1005);
            if (!entity.ParentId.Equals("0") && entity.ParentId.IsNotEmptyOrNull())
                throw HSZException.Oh(ErrorCode.WF0003);
            if (entity.FlowType == 1)
                throw HSZException.Oh(ErrorCode.WF0012);
            var isOk = await _flowTaskRepository.DeleteTask(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 撤回
        /// 注意：在撤回流程时要保证你的下一节点没有处理这条记录；如已处理则无法撤销流程。
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">流程经办</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/Withdraw")]
        public async Task Revoke(string id, [FromBody] FlowLaunchActionWithdrawInput input)
        {
            var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(id);
            var flowTaskNodeEntityList = await _flowTaskRepository.GetTaskNodeList(flowTaskEntity.Id);
            var flowTaskNodeEntity = flowTaskNodeEntityList.Find(m => m.SortCode == 2);
            await _flowTaskService.Revoke(flowTaskEntity, input.handleOpinion);
        }

        /// <summary>
        /// 催办
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Press/{id}")]
        public async Task Press(string id)
        {
            await _flowTaskService.Press(id);
        }

        #endregion
    }
}
