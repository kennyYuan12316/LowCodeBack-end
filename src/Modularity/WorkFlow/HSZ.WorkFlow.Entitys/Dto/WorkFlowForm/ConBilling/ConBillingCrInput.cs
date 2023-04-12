using HSZ.Dependency;
using System;
using System.Collections.Generic;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.ConBilling
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class ConBillingCrInput
    {
        /// <summary>
        /// 
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? flowUrgent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string drawer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? billDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string bank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? billAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string companyName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string conName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fileJson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string invoAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string invoiceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? payAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string taxId { get; set; }
        /// <summary>
        /// 候选人
        /// </summary>
        public Dictionary<string, List<string>> candidateList { get; set; }
    }
}
