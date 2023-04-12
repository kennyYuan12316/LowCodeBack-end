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
    /// 描 述：用户关系映射
    /// </summary>
    [SugarTable("ZJN_BASE_USER_RELATION")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class UserRelationEntity: CEntityBase
    {
        /// <summary>
        /// 获取或设置 用户编号
        /// </summary>
        [SugarColumn(ColumnName = "F_USERID", ColumnDescription = "用户编号")]
        public string UserId { get; set; }

        /// <summary>
        /// 获取或设置 对象类型
        /// </summary>
        [SugarColumn(ColumnName = "F_OBJECTTYPE", ColumnDescription = "对象类型")]
        public string ObjectType { get; set; }

        /// <summary>
        /// 获取或设置 对象主键
        /// </summary>
        [SugarColumn(ColumnName = "F_OBJECTID", ColumnDescription = "对象主键")]
        public string ObjectId { get; set; }

        /// <summary>
        /// 获取或设置 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE", ColumnDescription = "排序")]
        public long? SortCode { get; set; }
    }
}
