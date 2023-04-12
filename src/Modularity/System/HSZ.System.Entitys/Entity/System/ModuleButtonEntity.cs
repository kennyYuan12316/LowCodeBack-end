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
    /// 描 述：系统功能按钮
    /// </summary>
    [SugarTable("ZJN_BASE_MODULE_BUTTON")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ModuleButtonEntity : CLDEntityBase
    {
        /// <summary>
        /// 按钮上级
        /// </summary>
        [SugarColumn(ColumnName = "F_PARENTID")]
        public string ParentId { get; set; }

        /// <summary>
        /// 按钮名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }

        /// <summary>
        /// 按钮编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }

        /// <summary>
        /// 按钮图标
        /// </summary>
        [SugarColumn(ColumnName = "F_ICON")]
        public string Icon { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        [SugarColumn(ColumnName = "F_URLADDRESS")]
        public string UrlAddress { get; set; }

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
