using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 物料承重配置
    /// </summary>
    [SugarTable("zjn_wms_goods_weight")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsGoodsWeightEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        [SugarColumn(ColumnName = "F_GoodsCode")]        
        public string GoodsCode { get; set; }
        
        /// <summary>
        /// 最小承重
        /// </summary>
        [SugarColumn(ColumnName = "F_Min")]        
        public int? Min { get; set; }
        
        /// <summary>
        /// 最大承重
        /// </summary>
        [SugarColumn(ColumnName = "F_Max")]        
        public int? Max { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        [SugarColumn(ColumnName = "F_Unit")]        
        public int? Unit { get; set; }
        
        /// <summary>
        /// 是否删除 1删除 0未删除
        /// </summary>
        [SugarColumn(ColumnName = "F_IsDelete")]        
        public int? IsDelete { get; set; }
        
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
        /// 扩展字段
        /// </summary>
        [SugarColumn(ColumnName = "F_Extend")]        
        public string Extend { get; set; }
        
    }
}