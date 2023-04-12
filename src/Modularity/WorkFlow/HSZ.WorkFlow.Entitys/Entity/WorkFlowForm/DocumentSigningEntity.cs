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
    /// 描 述：文件签阅表
    /// </summary>
    [SugarTable("ZJN_WFORM_DOCUMENT_SIGNING")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class DocumentSigningEntity : EntityBase<string>
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
        /// 文件编码
        /// </summary>
        [SugarColumn(ColumnName = "F_FILLNUM")]
        public string FillNum { get; set; }
        /// <summary>
        /// 拟稿人
        /// </summary>
        [SugarColumn(ColumnName = "F_DRAFTEDPERSON")]
        public string DraftedPerson { get; set; }
        /// <summary>
        /// 签阅人
        /// </summary>
        [SugarColumn(ColumnName = "F_READER")]
        public string Reader { get; set; }
        /// <summary>
        /// 文件拟办
        /// </summary>
        [SugarColumn(ColumnName = "F_FILLPREPARATION")]
        public string FillPreparation { get; set; }
        /// <summary>
        /// 签阅时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CHECKDATE")]
        public DateTime? CheckDate { get; set; }
        /// <summary>
        /// 发稿日期
        /// </summary>
        [SugarColumn(ColumnName = "F_PUBLICATIONDATE")]
        public DateTime? PublicationDate { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
        /// <summary>
        /// 文件内容
        /// </summary>
        [SugarColumn(ColumnName = "F_DOCUMENTCONTENT")]
        public string DocumentContent { get; set; }
        /// <summary>
        /// 建议栏
        /// </summary>
        [SugarColumn(ColumnName = "F_ADVICECOLUMN")]
        public string AdviceColumn { get; set; }
    }
}
