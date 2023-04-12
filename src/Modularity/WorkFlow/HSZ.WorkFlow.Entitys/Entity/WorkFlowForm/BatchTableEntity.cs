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
    /// 描 述：行文呈批表
    /// </summary>
    [SugarTable("ZJN_WFORM_BATCH_TABLE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class BatchTableEntity : EntityBase<string>
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
        /// 主办单位
        /// </summary>
        [SugarColumn(ColumnName = "F_DRAFTEDPERSON")]
        public string DraftedPerson { get; set; }
        /// <summary>
        /// 文件编码
        /// </summary>
        [SugarColumn(ColumnName = "F_FILLNUM")]
        public string FillNum { get; set; }
        /// <summary>
        /// 发往单位
        /// </summary>
        [SugarColumn(ColumnName = "F_SENDUNIT")]
        public string SendUnit { get; set; }
        /// <summary>
        /// 打字
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPING")]
        public string Typing { get; set; }
        /// <summary>
        /// 发文日期
        /// </summary>
        [SugarColumn(ColumnName = "F_WRITINGDATE")]
        public DateTime? WritingDate { get; set; }
        /// <summary>
        /// 份数
        /// </summary>
        [SugarColumn(ColumnName = "F_SHARENUM")]
        public string ShareNum { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
