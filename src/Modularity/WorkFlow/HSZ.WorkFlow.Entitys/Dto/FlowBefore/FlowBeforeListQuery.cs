using HSZ.Common.Filter;
using HSZ.Dependency;

namespace HSZ.WorkFlow.Entitys.Dto.FlowBefore
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class FlowBeforeListQuery : PageInputBase
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public long? startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public long? endTime { get; set; }
        /// <summary>
        /// 引擎id
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 引擎分类
        /// </summary>
        public string flowCategory { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string creatorUserId { get; set; }
        /// <summary>
        /// 节点id
        /// </summary>
        public string nodeCode { get; set; }
    }
}
