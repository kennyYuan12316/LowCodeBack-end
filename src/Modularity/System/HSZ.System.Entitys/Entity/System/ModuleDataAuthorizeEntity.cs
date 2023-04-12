using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;

namespace HSZ.System.Entitys.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：数据权限
    /// </summary>
    [SugarTable("ZJN_BASE_MODULE_AUTHORIZE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ModuleDataAuthorizeEntity : CLDEntityBase
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }

        /// <summary>
        /// 字段编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE")]
        public string Type { get; set; }

        /// <summary>
        /// 条件符号
        /// </summary>
        [SugarColumn(ColumnName = "F_CONDITIONSYMBOL")]
        public string ConditionSymbol { get; set; }

        /// <summary>
        /// 条件符号Json
        /// </summary>
        [SugarColumn(ColumnName = "F_CONDITIONSYMBOLJSON")]
        public string ConditionSymbolJson { get; set; }

        /// <summary>
        /// 条件内容
        /// </summary>
        [SugarColumn(ColumnName = "F_CONDITIONTEXT")]
        public string ConditionText { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        [SugarColumn(ColumnName = "F_PROPERTYJSON")]
        public string PropertyJson { get; set; }

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
        /// 功能主键
        /// </summary>
        [SugarColumn(ColumnName = "F_MODULEID")]
        public string ModuleId { get; set; }
    }
}
