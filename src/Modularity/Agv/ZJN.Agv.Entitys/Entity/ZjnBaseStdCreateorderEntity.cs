using HSZ.Common.Const;
using SqlSugar;
using System;

namespace ZJN.Agv.Entitys.Entity
{
    /// <summary>
    /// 立库下单
    /// </summary>
    [SugarTable("zjn_base_std_createorder")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBaseStdCreateorderEntity
    {
        /// <summary>
        /// F_Id
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 业务关系编码
        /// </summary>
        [SugarColumn(ColumnName = "F_BrCode")]        
        public string BrCode { get; set; }
        
        /// <summary>
        /// 终点库区编码
        /// </summary>
        [SugarColumn(ColumnName = "F_EndAreaCode")]        
        public string EndAreaCode { get; set; }
        
        /// <summary>
        /// 终点库位编码
        /// </summary>
        [SugarColumn(ColumnName = "F_EndLocCode")]        
        public string EndLocCode { get; set; }
        
        /// <summary>
        /// LES订单ID
        /// </summary>
        [SugarColumn(ColumnName = "F_LesOrderId")]        
        public string LesOrderId { get; set; }
        
        /// <summary>
        /// 外部订单ID
        /// </summary>
        [SugarColumn(ColumnName = "F_OuterOrderId")]        
        public string OuterOrderId { get; set; }
        
        /// <summary>
        /// 起点库区编码
        /// </summary>
        [SugarColumn(ColumnName = "F_StartAreaCode")]        
        public string StartAreaCode { get; set; }
        
        /// <summary>
        /// 起点库位编码
        /// </summary>
        [SugarColumn(ColumnName = "F_StartLocCode")]        
        public string StartLocCode { get; set; }
        
        /// <summary>
        /// 托盘编码
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayId")]        
        public string TrayId { get; set; }
        
    }
}