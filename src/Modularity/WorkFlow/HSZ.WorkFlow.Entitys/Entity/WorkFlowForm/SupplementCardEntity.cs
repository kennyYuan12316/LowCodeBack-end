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
    /// 描 述：补卡申请
    /// </summary>
    [SugarTable("ZJN_WFORM_SUPPLEMENT_CARD")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class SupplementCardEntity : EntityBase<string>
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
        /// 员工姓名
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }
        /// <summary>
        /// 所在部门
        /// </summary>
        [SugarColumn(ColumnName = "F_DEPARTMENT")]
        public string Department { get; set; }
        /// <summary>
        /// 所在职务
        /// </summary>
        [SugarColumn(ColumnName = "F_POSITION")]
        public string Position { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYDATE")]
        public DateTime? ApplyDate { get; set; }
        /// <summary>
        /// 证明人
        /// </summary>
        [SugarColumn(ColumnName = "F_WITNESS")]
        public string Witness { get; set; }
        /// <summary>
        /// 补卡次数
        /// </summary>
        [SugarColumn(ColumnName = "F_SUPPLEMENTNUM")]
        public string SupplementNum { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [SugarColumn(ColumnName = "F_STARTTIME")]
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        [SugarColumn(ColumnName = "F_ENDTIME")]
        public DateTime? EndTime { get; set; }
    }
}
