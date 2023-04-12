using HSZ.Common.Const;
using SqlSugar;
using Yitter.IdGenerator;

namespace HSZ.VisualData.Entity
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：可视化配置表
    /// </summary>
    [SugarTable("ZJN_BLADE_VISUAL_CONFIG")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class VisualConfigEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "ID", ColumnDescription = "主键", IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 可视化表主键
        /// </summary>
        [SugarColumn(ColumnName = "VISUAL_ID", ColumnDescription = "可视化表主键")]
        public string VisualId { get; set; }

        /// <summary>
        /// 配置json
        /// </summary>
        [SugarColumn(ColumnName = "DETAIL", ColumnDescription = "配置json")]
        public string Detail { get; set; }

        /// <summary>
        /// 组件json
        /// </summary>
        [SugarColumn(ColumnName = "COMPONENT", ColumnDescription = "组件json")]
        public string Component { get; set; }

        /// <summary>
        /// 创建
        /// </summary>
        public virtual void Create()
        {
            this.Id = YitIdHelper.NextId().ToString();
        }
    }
}
