using HSZ.Common.Const;
using SqlSugar;

namespace HSZ.VisualData.Entity
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：可视化分类表
    /// </summary>
    [SugarTable("ZJN_BLADE_VISUAL_CATEGORY")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class VisualCategoryEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "ID", ColumnDescription = "主键", IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 分类键值
        /// </summary>
        [SugarColumn(ColumnName = "CATEGORY_KEY", ColumnDescription = "分类键值")]
        public string CategoryKey { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [SugarColumn(ColumnName = "CATEGORY_VALUE", ColumnDescription = "分类名称")]
        public string CategoryValue { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        [SugarColumn(ColumnName = "IS_DELETED", ColumnDescription = "是否已删除")]
        public int IsDeleted { get; set; }

        /// <summary>
        /// 删除
        /// </summary>
        public virtual void Delete()
        {
            this.IsDeleted = 1;
        }
    }
}
