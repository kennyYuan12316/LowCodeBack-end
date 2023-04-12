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
    /// 描 述：文件签批意见表
    /// </summary>
    [SugarTable("ZJN_WFORM_DOCUMENT_APPROVAL")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class DocumentApprovalEntity : EntityBase<string>
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
        /// 文件名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FILENAME")]
        public string FileName { get; set; }
        /// <summary>
        /// 拟稿人
        /// </summary>
        [SugarColumn(ColumnName = "F_DRAFTEDPERSON")]
        public string DraftedPerson { get; set; }
        /// <summary>
        /// 发文单位
        /// </summary>
        [SugarColumn(ColumnName = "F_SERVICEUNIT")]
        public string ServiceUnit { get; set; }
        /// <summary>
        /// 文件拟办
        /// </summary>
        [SugarColumn(ColumnName = "F_FILLPREPARATION")]
        public string FillPreparation { get; set; }
        /// <summary>
        /// 文件编码
        /// </summary>
        [SugarColumn(ColumnName = "F_FILLNUM")]
        public string FillNum { get; set; }
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
        /// <summary>
        /// 修改意见
        /// </summary>
        [SugarColumn(ColumnName = "F_MODIFYOPINION")]
        public string ModifyOpinion { get; set; }
    }
}
