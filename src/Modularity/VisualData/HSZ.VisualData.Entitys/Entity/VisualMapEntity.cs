using HSZ.Common.Const;
using SqlSugar;

namespace HSZ.VisualData.Entity
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：可视化地图配置表
    /// </summary>
    [SugarTable("ZJN_BLADE_VISUAL_MAP")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class VisualMapEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "ID", ColumnDescription = "主键", IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "Name", ColumnDescription = "名称")]
        public string Name { get; set; }

        /// <summary>
        /// 地图数据
        /// </summary>
        [SugarColumn(ColumnName = "DATA", ColumnDescription = "地图数据")]
        public string data { get; set; }

    }
}
