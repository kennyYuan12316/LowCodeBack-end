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
    /// 描 述：出库单
    /// </summary>
    [SugarTable("ZJN_WFORM_OUTBOUND_ORDER")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class OutboundOrderEntity : EntityBase<string>
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
        /// 客户名称
        /// </summary>
        [SugarColumn(ColumnName = "F_CUSTOMERNAME")]
        public string CustomerName { get; set; }
        /// <summary>
        /// 仓库
        /// </summary>
        [SugarColumn(ColumnName = "F_WAREHOUSE")]
        public string Warehouse { get; set; }
        /// <summary>
        /// 仓库人
        /// </summary>
        [SugarColumn(ColumnName = "F_OUTSTORAGE")]
        public string OutStorage { get; set; }
        /// <summary>
        /// 业务人员
        /// </summary>
        [SugarColumn(ColumnName = "F_BUSINESSPEOPLE")]
        public string BusinessPeople { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        [SugarColumn(ColumnName = "F_BUSINESSTYPE")]
        public string BusinessType { get; set; }
        /// <summary>
        /// 出库日期
        /// </summary>
        [SugarColumn(ColumnName = "F_OUTBOUNDDATE")]
        public DateTime? OutboundDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}