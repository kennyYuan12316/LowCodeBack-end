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
    /// 描 述：领料单
    /// </summary>
    [SugarTable("ZJN_WFORM_MATERIAL_REQUISITION")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class MaterialRequisitionEntity : EntityBase<string>
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
        /// 领料人
        /// </summary>
        [SugarColumn(ColumnName = "F_LEADPEOPLE")]
        public string LeadPeople { get; set; }
        /// <summary>
        /// 领料部门
        /// </summary>
        [SugarColumn(ColumnName = "F_LEADDEPARTMENT")]
        public string LeadDepartment { get; set; }
        /// <summary>
        /// 领料日期
        /// </summary>
        [SugarColumn(ColumnName = "F_LEADDATE")]
        public DateTime? LeadDate { get; set; }
        /// <summary>
        /// 仓库
        /// </summary>
        [SugarColumn(ColumnName = "F_WAREHOUSE")]
        public string Warehouse { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}