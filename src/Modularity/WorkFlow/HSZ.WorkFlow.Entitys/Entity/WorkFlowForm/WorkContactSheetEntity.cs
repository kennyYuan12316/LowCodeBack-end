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
    /// 描 述：工作联系单
    /// </summary>
    [SugarTable("ZJN_WFORM_WORK_CONTACT_SHEET")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class WorkContactSheetEntity : EntityBase<string>
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
        /// 发件人
        /// </summary>
        [SugarColumn(ColumnName = "F_DRAWPEOPLE")]
        public string DrawPeople { get; set; }
        /// <summary>
        /// 发件部门
        /// </summary>
        [SugarColumn(ColumnName = "F_ISSUINGDEPARTMENT")]
        public string IssuingDepartment { get; set; }
        /// <summary>
        /// 发件日期
        /// </summary>
        [SugarColumn(ColumnName = "F_TODATE")]
        public DateTime? ToDate { get; set; }
        /// <summary>
        /// 收件部门
        /// </summary>
        [SugarColumn(ColumnName = "F_SERVICEDEPARTMENT")]
        public string ServiceDepartment { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        [SugarColumn(ColumnName = "F_RECIPIENTS")]
        public string Recipients { get; set; }
        /// <summary>
        /// 收件日期
        /// </summary>
        [SugarColumn(ColumnName = "F_COLLECTIONDATE")]
        public DateTime? CollectionDate { get; set; }
        /// <summary>
        /// 协调事项
        /// </summary>
        [SugarColumn(ColumnName = "F_COORDINATION")]
        public string Coordination { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
    }
}
