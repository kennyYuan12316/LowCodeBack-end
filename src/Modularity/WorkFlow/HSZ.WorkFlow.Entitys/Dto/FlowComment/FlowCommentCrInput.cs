using HSZ.Dependency;

namespace HSZ.WorkFlow.Entitys.Dto.FlowComment
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class FlowCommentCrInput
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string file { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string image { get; set; }
        /// <summary>
        /// 流程分类
        /// </summary>
        public string taskId { get; set; }
        /// <summary>
        /// 委托流程
        /// </summary>
        public string text { get; set; }
    }
}
