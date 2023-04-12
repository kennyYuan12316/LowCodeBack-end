using HSZ.Dependency;
using System;
using System.Collections.Generic;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.BatchPack
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class BatchPackCrInput
    {
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? flowUrgent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? compactorDate { get; set; }
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
        public string compactor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? operationDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string packing { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string productName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string production { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string productionQuty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string regulations { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string standard { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehousNo { get; set; }
        /// <summary>
        /// 候选人
        /// </summary>
        public Dictionary<string, List<string>> candidateList { get; set; }
    }
}
