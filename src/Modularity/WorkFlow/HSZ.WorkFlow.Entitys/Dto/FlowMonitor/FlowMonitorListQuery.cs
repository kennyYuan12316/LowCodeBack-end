using HSZ.Common.Filter;
using HSZ.Dependency;

namespace HSZ.WorkFlow.Entitys.Dto.FlowMonitor
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class FlowMonitorListQuery : PageInputBase
    {

        /// <summary>
        /// 发起人员id
        /// </summary>
        public string creatorUserId { get; set; }

        /// <summary>
        /// 所属分类
        /// </summary>
        public string flowCategory { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public long? startTime { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public long? endTime { get; set; }

        /// <summary>
        /// 流程主键
        /// </summary>
        public string flowId { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        public int? status { get; set; }


    }
}
