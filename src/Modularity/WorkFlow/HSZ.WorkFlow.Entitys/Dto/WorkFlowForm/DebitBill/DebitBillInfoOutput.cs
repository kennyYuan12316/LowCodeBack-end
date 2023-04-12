using HSZ.Dependency;
using System;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.DebitBill
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class DebitBillInfoOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public decimal? amountDebit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? applyDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string departmental { get; set; }
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
        public string loanMode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string paymentMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string reason { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string repaymentBill { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string staffId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string staffName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string staffPost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? teachingDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string transferAccount { get; set; }
    }
}
