using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;

namespace HSZ.System.Entitys.Permission
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：模块列表权限
    /// </summary>
    [SugarTable("ZJN_BASE_COLUMN_PURVIEW")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ColumnsPurviewEntity : CLDEntityBase
    {
        /// <summary>
        /// 模块ID
        /// </summary>
        [SugarColumn(ColumnName = "F_MODULEID")]
        public string ModuleId { get; set; }

        /// <summary>
        /// 列表字段数组
        /// </summary>
        [SugarColumn(ColumnName = "F_FIELDLIST")]
        public string FieldList { get; set; }

    }
}
