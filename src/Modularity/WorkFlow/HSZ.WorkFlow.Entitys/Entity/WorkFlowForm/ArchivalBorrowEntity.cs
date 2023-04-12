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
    /// 描 述：档案借阅申请
    /// </summary>
    [SugarTable("ZJN_WFORM_ARCHIVAL_BORROW")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ArchivalBorrowEntity : EntityBase<string>
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
        /// 申请人
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYUSER")]
        public string ApplyUser { get; set; }
        /// <summary>
        /// 借阅部门
        /// </summary>
        [SugarColumn(ColumnName = "F_BORROWINGDEPARTMENT")]
        public string BorrowingDepartment { get; set; }
        /// <summary>
        /// 档案名称
        /// </summary>
        [SugarColumn(ColumnName = "F_ARCHIVESNAME")]
        public string ArchivesName { get; set; }
        /// <summary>
        /// 借阅时间
        /// </summary>
        [SugarColumn(ColumnName = "F_BORROWINGDATE")]
        public DateTime? BorrowingDate { get; set; }
        /// <summary>
        /// 归还时间
        /// </summary>
        [SugarColumn(ColumnName = "F_RETURNDATE")]
        public DateTime? ReturnDate { get; set; }
        /// <summary>
        /// 档案属性
        /// </summary>
        [SugarColumn(ColumnName = "F_ARCHIVALATTRIBUTES")]
        public string ArchivalAttributes { get; set; }
        /// <summary>
        /// 借阅方式
        /// </summary>
        [SugarColumn(ColumnName = "F_BORROWMODE")]
        public string BorrowMode { get; set; }
        /// <summary>
        /// 申请原因
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYREASON")]
        public string ApplyReason { get; set; }
        /// <summary>
        /// 档案编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ARCHIVESID")]
        public string ArchivesId { get; set; }
    }
}
