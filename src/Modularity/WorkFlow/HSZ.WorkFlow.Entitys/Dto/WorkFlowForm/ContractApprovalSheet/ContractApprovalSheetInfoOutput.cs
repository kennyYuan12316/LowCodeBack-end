using HSZ.Dependency;
using System;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.ContractApprovalSheet
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class ContractApprovalSheetInfoOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? applyDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string applyUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string budgetaryApproval { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contractContent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contractId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contractName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contractNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contractPeriod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contractType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? endContractDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fileJson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string firstParty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
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
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? incomeAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string leadDepartment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string paymentMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string personCharge { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string secondParty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string signArea { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? startContractDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? totalExpenditure { get; set; }
    }
}
