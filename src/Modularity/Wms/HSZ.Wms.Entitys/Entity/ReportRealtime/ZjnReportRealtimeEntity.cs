using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 实时库存
    /// </summary>
    [SugarTable("zjn_report_realtime")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnReportRealtimeEntity
    {
        /// <summary>
        /// F_Id
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_Description")]        
        public string Description { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
        
        /// <summary>
        /// 工厂
        /// </summary>
        [SugarColumn(ColumnName = "F_Factory")]        
        public string Factory { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsCode")]        
        public string ProductsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsName")]        
        public string ProductsName { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        [SugarColumn(ColumnName = "F_Batch")]        
        public string Batch { get; set; }
        
        /// <summary>
        /// 质量状态
        /// </summary>
        [SugarColumn(ColumnName = "F_Quality")]        
        public string Quality { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsUnit")]        
        public string ProductsUnit { get; set; }
        
        /// <summary>
        /// 总数库内
        /// </summary>
        [SugarColumn(ColumnName = "F_SumInside")]        
        public int? SumInside { get; set; }
        
        /// <summary>
        /// 托盘数库内
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayCountInside")]        
        public int? TrayCountInside { get; set; }
        
        /// <summary>
        /// 总数库外
        /// </summary>
        [SugarColumn(ColumnName = "F_SumOutside")]        
        public int? SumOutside { get; set; }
        
        /// <summary>
        /// 托盘数库外
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayCountOutside")]        
        public int? TrayCountOutside { get; set; }
        
        /// <summary>
        /// 总数移动中
        /// </summary>
        [SugarColumn(ColumnName = "F_SumMoving")]        
        public int? SumMoving { get; set; }
        
        /// <summary>
        /// 托盘数移动中
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayCountMoving")]        
        public int? TrayCountMoving { get; set; }
        
        /// <summary>
        /// 总数全部
        /// </summary>
        [SugarColumn(ColumnName = "F_SumTotal")]        
        public int? SumTotal { get; set; }
        
        /// <summary>
        /// 托盘数全部
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayCountTotal")]        
        public int? TrayCountTotal { get; set; }
        
    }
}