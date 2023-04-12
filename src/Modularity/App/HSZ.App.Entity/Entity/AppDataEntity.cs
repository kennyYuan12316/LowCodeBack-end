
using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;

namespace HSZ.Apps.Entitys
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：App常用数据
    /// </summary>
    [SugarTable("ZJN_BASE_APPDATA")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class AppDataEntity:CDEntityBase
    {
        /// <summary>
        /// 对象类型
        /// </summary>
        [SugarColumn(ColumnName = "F_OBJECTTYPE")]
        public string ObjectType { get; set; }
        /// <summary>
        /// 对象主键
        /// </summary>
        [SugarColumn(ColumnName = "F_OBJECTID")]
        public string ObjectId { get; set; }
        /// <summary>
        /// 对象json
        /// </summary>
        [SugarColumn(ColumnName = "F_OBJECTDATA")]
        public string ObjectData { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
