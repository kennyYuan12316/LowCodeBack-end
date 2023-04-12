using HSZ.Dependency;
using System;
using System.Collections.Generic;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.WarehouseReceipt
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class WarehouseReceiptCrInput
    {
        public int? status { get; set; }
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
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contactPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? warehousDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string deliveryNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string supplierName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehousCategory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehouse { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehouseNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehousesPeople { get; set; }
        public List<WarehouseEntryListItem> entryList { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        /// <summary>
        /// 候选人
        /// </summary>
        public Dictionary<string, List<string>> candidateList { get; set; }
    }



    public class WarehouseEntryListItem
    {
        /// <summary>
        /// 
        /// </summary>
        public decimal? amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string goodsName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? qty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? sortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string specifications { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehouseId { get; set; }
        public string id { get; set; }
    }
}
