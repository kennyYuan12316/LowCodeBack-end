using HSZ.Dependency;
using System;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.ViolationHandling
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class ViolationHandlingInfoOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public decimal? amountMoney { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string deduction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string driver { get; set; }
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
        public string leadingOfficial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? limitDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? noticeDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? peccancyDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string plateNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string violationBehavior { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string violationSite { get; set; }
    }
}
