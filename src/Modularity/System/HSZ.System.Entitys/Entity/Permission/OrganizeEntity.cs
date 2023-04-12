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
    /// 描 述：机构管理
    /// </summary>
    [SugarTable("ZJN_BASE_ORGANIZE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class OrganizeEntity : CLDEntityBase
    {
        /// <summary>
        /// 机构上级
        /// </summary>
        [SugarColumn(ColumnName = "F_PARENTID")]
        public string ParentId { get; set; }
         
        /// <summary>
        /// 父级组织 
        /// </summary>
        [SugarColumn(ColumnName = "F_ORGANIZEIDTREE")]
        public string OrganizeIdTree { get; set; }

        /// <summary>
        /// 机构分类【company-公司、department-部门】
        /// </summary>
        [SugarColumn(ColumnName = "F_CATEGORY")]
        public string Category { get; set; }

        /// <summary>
        /// 机构编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }

        /// <summary>
        /// 机构主管
        /// </summary>
        [SugarColumn(ColumnName = "F_MANAGERID")]
        public string ManagerId { get; set; }

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
    }
}
