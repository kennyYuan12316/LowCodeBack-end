using HSZ.Dependency;
using System;
using System.Collections.Generic;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.IncomeRecognition
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class IncomeRecognitionCrInput
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
        public string settlementMonth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contractNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string moneyBank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? paymentDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contactName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contacPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? actualAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? amountPaid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contactQQ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string customerName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? totalAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? unpaidAmount { get; set; }
        /// <summary>
        /// 候选人
        /// </summary>
        public Dictionary<string, List<string>> candidateList { get; set; }
    }
}
