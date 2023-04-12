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
    /// 描 述：分级管理
    /// </summary>
    [SugarTable("ZJN_BASE_ORGANIZE_ADMINISTRATOR")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class OrganizeAdministratorEntity : CLDEntityBase
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(ColumnName = "F_USERID")]
        public string UserId { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        [SugarColumn(ColumnName = "F_ORGANIZEID")]
        public string OrganizeId { get; set; }

        /// <summary>
        /// 机构类型
        /// </summary>
        [SugarColumn(ColumnName = "F_ORGANIZETYPE")]
        public string OrganizeType { get; set; }

        /// <summary>
        /// 本层级添加
        /// </summary>
        [SugarColumn(ColumnName = "F_THISLAYERADD")]
        public int ThisLayerAdd { get; set; }

        /// <summary>
        /// 本层级编辑
        /// </summary>
        [SugarColumn(ColumnName = "F_THISLAYEREDIT")]
        public int ThisLayerEdit { get; set; }

        /// <summary>
        /// 本层级删除
        /// </summary>
        [SugarColumn(ColumnName = "F_THISLAYERDELETE")]
        public int ThisLayerDelete { get; set; }

        /// <summary>
        /// 子层级添加
        /// </summary>
        [SugarColumn(ColumnName = "F_SUBLAYERADD")]
        public int SubLayerAdd { get; set; }

        /// <summary>
        /// 子层级编辑
        /// </summary>
        [SugarColumn(ColumnName = "F_SUBLAYEREDIT")]
        public int SubLayerEdit { get; set; }

        /// <summary>
        /// 子层级删除
        /// </summary>
        [SugarColumn(ColumnName = "F_SUBLAYERDELETE")]
        public int SubLayerDelete { get; set; }

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
    }
}