using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;
using System;

namespace HSZ.WorkFlow.Entitys
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：报价审批表
    /// </summary>
    [SugarTable("ZJN_WFORM_QUOTATION_APPROVAL")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class QuotationApprovalEntity : EntityBase<string>
    {
        /// <summary>
        /// 流程主键
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWID")]
        public string FlowId { get; set; }
        /// <summary>
        /// 流程标题
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWTITLE")]
        public string FlowTitle { get; set; }
        /// <summary>
        /// 流程等级
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWURGENT")]
        public int? FlowUrgent { get; set; }
        /// <summary>
        /// 流程单据
        /// </summary>
        [SugarColumn(ColumnName = "F_BILLNO")]
        public string BillNo { get; set; }
        /// <summary>
        /// 填报人
        /// </summary>
        [SugarColumn(ColumnName = "F_WRITER")]
        public string Writer { get; set; }
        /// <summary>
        /// 填表日期
        /// </summary>
        [SugarColumn(ColumnName = "F_WRITEDATE")]
        public DateTime? WriteDate { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [SugarColumn(ColumnName = "F_CUSTOMERNAME")]
        public string CustomerName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(ColumnName = "F_QUOTATIONTYPE")]
        public string QuotationType { get; set; }
        /// <summary>
        /// 合作人名
        /// </summary>
        [SugarColumn(ColumnName = "F_PARTNERNAME")]
        public string PartnerName { get; set; }
        /// <summary>
        /// 模板参考
        /// </summary>
        [SugarColumn(ColumnName = "F_STANDARDFILE")]
        public string StandardFile { get; set; }
        /// <summary>
        /// 情况描述
        /// </summary>
        [SugarColumn(ColumnName = "F_CUSTSITUATION")]
        public string CustSituation { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
    }
}
