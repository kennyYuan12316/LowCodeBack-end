using HSZ.Dependency;
using HSZ.WorkFlow.Entitys.Model;
using System;
using System.Collections.Generic;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.PurchaseList
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class PurchaseListInfoOutput
    {
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
        public string buyer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string departmental { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fileJson { get; set; }
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
        public string paymentMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? paymentMoney { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? purchaseDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string reason { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string telephone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string vendorName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehouse { get; set; }
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<EntryListItem> entryList { get; set; } = new List<EntryListItem>();
    }
}
