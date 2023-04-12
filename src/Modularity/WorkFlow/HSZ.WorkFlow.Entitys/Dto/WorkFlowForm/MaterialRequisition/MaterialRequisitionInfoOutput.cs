using HSZ.Dependency;
using HSZ.WorkFlow.Entitys.Model;
using System;
using System.Collections.Generic;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.MaterialRequisition
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class MaterialRequisitionInfoOutput
    {
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
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
        public DateTime? leadDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string leadDepartment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string leadPeople { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehouse { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<EntryListItem> entryList { get; set; } = new List<EntryListItem>();
    }
}
