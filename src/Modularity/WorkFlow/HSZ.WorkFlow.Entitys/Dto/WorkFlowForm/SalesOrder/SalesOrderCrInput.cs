using HSZ.Dependency;
using HSZ.WorkFlow.Entitys.Model;
using System;
using System.Collections.Generic;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.SalesOrder
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class SalesOrderCrInput
    {
        /// <summary>
        /// 
        /// </summary>
        public string flowTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? flowUrgent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string customerName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ticketDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contactPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contacts { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string customerAddres { get; set; }
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
        public string invoiceType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string paymentMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? paymentMoney { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? salesDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string salesman { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ticketNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<EntryListItem> entryList { get; set; }
        /// <summary>
        /// 候选人
        /// </summary>
        public Dictionary<string, List<string>> candidateList { get; set; }
    }
}
