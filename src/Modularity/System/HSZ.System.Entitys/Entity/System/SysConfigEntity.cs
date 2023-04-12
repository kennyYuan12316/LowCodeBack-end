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
    /// 描 述：系统设置
    /// </summary>
    [SugarTable("ZJN_BASE_SYS_CONFIG")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class SysConfigEntity : EntityBase<string>
    {
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "F_NAME", ColumnDescription = "名称")]
        public string Name { get; set; }

        /// <summary>
        /// 键
        /// </summary>
        [SugarColumn(ColumnName = "F_KEY", ColumnDescription = "键")]
        public string Key { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [SugarColumn(ColumnName = "F_VALUE", ColumnDescription = "值")]
        public string Value { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        [SugarColumn(ColumnName = "F_CATEGORY", ColumnDescription = "分类")]
        public string Category { get; set; }
    }
}
