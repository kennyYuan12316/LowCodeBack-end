using HSZ.Dependency;
using System;

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
    public class FlowBeforeRecordListModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 审批人
        /// </summary>
        public string handleId { get; set; }
        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? handleTime { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>
        public string handleOpinion { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        public int? handleStatus { get; set; }
        /// <summary>
        /// 审批人名
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 分类id
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 分类名
        /// </summary>
        public string categoryName { get; set; }
        /// <summary>
        /// 流转操作人
        /// </summary>
        public string operatorId { get; set; }
    }
}
