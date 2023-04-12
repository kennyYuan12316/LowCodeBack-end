using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;

namespace HSZ.WorkFlow.Entitys
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：流程可见
    /// </summary>
    [SugarTable("ZJN_FLOW_ENGINE_VISIBLE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class FlowEngineVisibleEntity:CEntityBase
    {
        /// <summary>
        /// 流程主键
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWID")]
        public string FlowId { get; set; }
        /// <summary>
        /// 经办类型
        /// </summary>
        [SugarColumn(ColumnName = "F_OPERATORTYPE")]
        public string OperatorType { get; set; }
        /// <summary>
        /// 经办主键
        /// </summary>
        [SugarColumn(ColumnName = "F_OPERATORID")]
        public string OperatorId { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }
    }
}
