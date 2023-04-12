using HSZ.Common.Filter;
using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.WorkFlow.Entitys.Model
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：流程经办
    /// </summary>
    [SuppressSniffer]
    public class FlowHandleModel: PageInputBase
    {
        /// <summary>
        /// 意见
        /// </summary>
        public string handleOpinion { get; set; }
        /// <summary>
        /// 加签人
        /// </summary>
        public string freeApproverUserId { get; set; }
        /// <summary>
        /// 自定义抄送人
        /// </summary>
        public string copyIds { get; set; }
        /// <summary>
        /// 流程编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 表单数据
        /// </summary>
        public object formData { get; set; }
        /// <summary>
        /// 流程监控指派节点
        /// </summary>
        public string nodeCode { get; set; }
        /// <summary>
        /// 电子签名
        /// </summary>
        public string signImg { get; set; }

        /// <summary>
        /// 候选人
        /// </summary>
        public Dictionary<string,List<string>> candidateList { get; set; }

        /// <summary>
        /// 批量id
        /// </summary>
        public List<string> ids { get; set; }

        /// <summary>
        /// 批量类型
        /// </summary>
        public int batchType { get; set; }
    }


}
