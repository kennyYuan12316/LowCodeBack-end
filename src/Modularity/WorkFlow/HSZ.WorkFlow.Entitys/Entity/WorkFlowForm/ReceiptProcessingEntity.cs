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
    /// 描 述：收文处理表
    /// </summary>
    [SugarTable("ZJN_WFORM_RECEIPT_PROCESSING")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ReceiptProcessingEntity : EntityBase<string>
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
        /// 紧急程度
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWURGENT")]
        public int? FlowUrgent { get; set; }
        /// <summary>
        /// 流程单据
        /// </summary>
        [SugarColumn(ColumnName = "F_BILLNO")]
        public string BillNo { get; set; }
        /// <summary>
        /// 文件标题
        /// </summary>
        [SugarColumn(ColumnName = "F_FILETITLE")]
        public string FileTitle { get; set; }
        /// <summary>
        /// 来文单位
        /// </summary>
        [SugarColumn(ColumnName = "F_COMMUNICATIONUNIT")]
        public string CommunicationUnit { get; set; }
        /// <summary>
        /// 来文字号
        /// </summary>
        [SugarColumn(ColumnName = "F_LETTERNUM")]
        public string LetterNum { get; set; }
        /// <summary>
        /// 收文日期
        /// </summary>
        [SugarColumn(ColumnName = "F_RECEIPTDATE")]
        public DateTime? ReceiptDate { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
    }
}
