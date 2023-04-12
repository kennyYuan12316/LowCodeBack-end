using HSZ.Dependency;
using System;
using System.Collections.Generic;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.SalesSupport
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class SalesSupportCrInput
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
        public string customer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string project { get; set; }
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
        public DateTime? applyDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string applyDept { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string applyUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string conclusion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string consulManager { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string consultResult { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fileJson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ievaluation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string psalSupConsul { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string psalePreDays { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string psaleSupDays { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string psaleSupInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string salSupConclu { get; set; }
        /// <summary>
        /// 候选人
        /// </summary>
        public Dictionary<string, List<string>> candidateList { get; set; }
    }
}
