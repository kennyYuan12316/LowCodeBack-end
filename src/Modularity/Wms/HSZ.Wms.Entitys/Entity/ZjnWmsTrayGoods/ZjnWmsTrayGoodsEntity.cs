using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 托盘货物绑定表
    /// </summary>
    [SugarTable("zjn_wms_tray_goods")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsTrayGoodsEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 货物ID
        /// </summary>
        [SugarColumn(ColumnName = "F_GoodsId")]        
        public string GoodsId { get; set; }
        
        /// <summary>
        /// 货物代码
        /// </summary>
        [SugarColumn(ColumnName = "F_GoodsCode")]        
        public string GoodsCode { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        [SugarColumn(ColumnName = "F_Quantity")]        
        public int? Quantity { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        [SugarColumn(ColumnName = "F_Unit")]        
        public string Unit { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayNo")]        
        public string TrayNo { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateUser")]        
        public string CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        [SugarColumn(ColumnName = "F_EnabledMark")]        
        public int? EnabledMark { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        [SugarColumn(ColumnName = "F_IsDeleted")]
        public int? IsDeleted { get; set; }

    }
}