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
    /// 描 述：流程引擎
    /// </summary>
    [SugarTable("ZJN_FLOW_ENGINE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class FlowEngineEntity : CLDEntityBase
    {
        /// <summary>
        /// 流程编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }
        /// <summary>
        /// 流程类型（0：发起流程，1：功能流程）
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE")]
        public int? Type { get; set; }
        /// <summary>
        /// 流程分类
        /// </summary>
        [SugarColumn(ColumnName = "F_CATEGORY")]
        public string Category { get; set; }
        /// <summary>
        /// 流程表单
        /// </summary>
        [SugarColumn(ColumnName = "F_FORM")]
        public string Form { get; set; }
        /// <summary>
        /// 可见类型
        /// </summary>
        [SugarColumn(ColumnName = "F_VISIBLETYPE")]
        public int? VisibleType { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        [SugarColumn(ColumnName = "F_ICON")]
        public string Icon { get; set; }
        /// <summary>
        /// 图标背景色
        /// </summary>
        [SugarColumn(ColumnName = "F_ICONBACKGROUND")]
        public string IconBackground { get; set; }
        /// <summary>
        /// 流程版本
        /// </summary>
        [SugarColumn(ColumnName = "F_VERSION")]
        public string Version { get; set; }
        /// <summary>
        /// 流程模板
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWTEMPLATEJSON")]
        public string FlowTemplateJson { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }
        /// <summary>
        /// 表单模板
        /// </summary>
        [SugarColumn(ColumnName = "F_FORMTEMPLATEJSON")]
        public string FormTemplateJson { get; set; }
        /// <summary>
        /// 表单分类（2：自定义表单，1：系统表单）
        /// </summary>
        [SugarColumn(ColumnName = "F_FORMTYPE")]
        public int? FormType { get; set; }
        /// <summary>
        /// 关联表单
        /// </summary>
        [SugarColumn(ColumnName = "F_TABLES")]
        public string Tables { get; set; }
        /// <summary>
        /// 数据源id
        /// </summary>
        [SugarColumn(ColumnName = "F_DBLINKID")]
        public string DbLinkId { get; set; }
        /// <summary>
        /// 表单分类
        /// </summary>
        [SugarColumn(ColumnName = "F_APPFORMURL")]
        public string AppFormUrl { get; set; }
        /// <summary>
        /// 数据源id
        /// </summary>
        [SugarColumn(ColumnName = "F_FORMURL")]
        public string FormUrl { get; set; }
    }
}
