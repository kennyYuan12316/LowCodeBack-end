using HSZ.Dependency;
using System;
using System.Collections.Generic;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.DocumentSigning
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class DocumentSigningCrInput
    {

        /// <summary>
        /// 
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? publicationDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? checkDate { get; set; }
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
        public decimal? adviceColumn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string documentContent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string draftedPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fileJson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fillNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fillPreparation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string reader { get; set; }
        /// <summary>
        /// 候选人
        /// </summary>
        public Dictionary<string, List<string>> candidateList { get; set; }
    }
}
