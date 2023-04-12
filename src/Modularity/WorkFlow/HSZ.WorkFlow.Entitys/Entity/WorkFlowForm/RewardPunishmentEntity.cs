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
    /// 描 述：行政赏罚单
    /// </summary>
    [SugarTable("ZJN_WFORM_REWARD_PUNISHMENT")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class RewardPunishmentEntity : EntityBase<string>
    {
        /// <summary>
        /// 主键
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
        /// 填表日期
        /// </summary>
        [SugarColumn(ColumnName = "F_FILLFROMDATE")]
        public DateTime? FillFromDate { get; set; }
        /// <summary>
        /// 员工部门
        /// </summary>
        [SugarColumn(ColumnName = "F_DEPARTMENT")]
        public string Department { get; set; }
        /// <summary>
        /// 员工职位
        /// </summary>
        [SugarColumn(ColumnName = "F_POSITION")]
        public string Position { get; set; }
        /// <summary>
        /// 赏罚金额
        /// </summary>
        [SugarColumn(ColumnName = "F_REWARDPUN")]
        public decimal? RewardPun { get; set; }
        /// <summary>
        /// 赏罚原因
        /// </summary>
        [SugarColumn(ColumnName = "F_REASON")]
        public string Reason { get; set; }
    }
}
