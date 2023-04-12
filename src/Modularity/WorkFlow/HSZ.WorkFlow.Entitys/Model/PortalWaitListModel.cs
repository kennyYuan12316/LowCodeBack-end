using HSZ.Dependency;
using System;

namespace HSZ.WorkFlow.Entitys.Model
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class PortalWaitListModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? formType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string processId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string taskNodeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string taskOperatorId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? type { get; set; }
    }
}
