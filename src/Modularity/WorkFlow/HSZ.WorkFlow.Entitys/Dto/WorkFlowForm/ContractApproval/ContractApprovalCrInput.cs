using HSZ.Dependency;
using System;
using System.Collections.Generic;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.ContractApproval
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class ContractApprovalCrInput
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
        public string contractName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contractId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? signingDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? startDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? endDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string businessPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contractClass { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contractType { get; set; }
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
        public string firstPartyContact { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string firstPartyPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string firstPartyUnit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string freeApproverUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? incomeAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string inputPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string primaryCoverage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string secondPartyContact { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string secondPartyPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string secondPartyUnit { get; set; }
        /// <summary>
        /// 候选人
        /// </summary>
        public Dictionary<string, List<string>> candidateList { get; set; }
    }
}
