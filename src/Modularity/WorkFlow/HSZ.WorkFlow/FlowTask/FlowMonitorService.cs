using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.WorkFlow.Entitys.Dto.FlowMonitor;
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
    /// 描 述：流程监控
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowEngine", Name = "FlowMonitor", Order = 304)]
    [Route("api/workflow/Engine/[controller]")]
    public class FlowMonitorService : IDynamicApiController, ITransient
    {
        private readonly IFlowTaskRepository _flowTaskRepository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowTaskRepository"></param>
        public FlowMonitorService(IFlowTaskRepository flowTaskRepository)
        {
            _flowTaskRepository = flowTaskRepository;
        }

        #region GET
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] FlowMonitorListQuery input)
        {
            return await _flowTaskRepository.GetMonitorList(input);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task Delete([FromBody] FlowMonitorDeleteInput input)
        {
            var ids = new List<string>(input.ids.Split(","));
            var entityList = ids.Select(x => _flowTaskRepository.GetTaskInfo(x).Result).ToList();
            foreach (var item in entityList)
            {
                if (item == null)
                    throw HSZException.Oh(ErrorCode.COM1005);
                if (!item.ParentId.Equals("0") && item.ParentId.IsNotEmptyOrNull())
                    throw HSZException.Oh(ErrorCode.WF0003);
                if (item.FlowType == 1)
                    throw HSZException.Oh(ErrorCode.WF0012);
                await _flowTaskRepository.DeleteTask(item);
            }
        }
        #endregion
    }
}
