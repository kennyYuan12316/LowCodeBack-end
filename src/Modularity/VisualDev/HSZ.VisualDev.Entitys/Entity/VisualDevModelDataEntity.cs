using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;

namespace HSZ.VisualDev.Entitys.Entity
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：可视化开发功能实体
    /// </summary>
    [SugarTable("ZJN_BASE_VISUALDEV_MODEL")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class VisualDevModelDataEntity : CLDEntityBase
    {
        /// <summary>
        /// 功能ID
        /// </summary>
        [SugarColumn(ColumnName = "F_VISUALDEVID")]
        public string VisualDevId { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }

        /// <summary>
        /// 区分主子表-
        /// </summary>
        [SugarColumn(ColumnName = "F_PARENTID")]
        public string ParentId { get; set; }

        /// <summary>
        /// 数据包
        /// </summary>
        [SugarColumn(ColumnName = "F_DATA")]
        public string Data { get; set; }
    }
}
