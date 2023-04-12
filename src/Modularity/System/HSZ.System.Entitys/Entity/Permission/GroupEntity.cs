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
    /// 描 述：分组信息基类
    /// </summary>
    [SugarTable("ZJN_BASE_GROUP")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class GroupEntity : CLDEntityBase
    {
        /// <summary>
        /// 获取或设置 分组名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME", ColumnDescription = "分组名称")]
        public string FullName { get; set; }

        /// <summary>
        /// 获取或设置 分组编号
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE", ColumnDescription = "分组编号")]
        public string EnCode { get; set; }

        /// <summary>
        /// 获取或设置 分组类型
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE", ColumnDescription = "分组类型")]
        public string Type { get; set; }

        /// <summary>
        /// 获取或设置 排序
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE", ColumnDescription = "排序")]
        public long? SortCode { get; set; }

        /// <summary>
        /// 获取或设置 说明
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION", ColumnDescription = "说明")]
        public string Description { get; set; }

    }
}
