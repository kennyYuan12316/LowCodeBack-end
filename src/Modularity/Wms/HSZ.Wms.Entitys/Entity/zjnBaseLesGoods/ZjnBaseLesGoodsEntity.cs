using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// LES物料原始数据
    /// </summary>
    [SugarTable("zjn_base_les_goods")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBaseLesGoodsEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 物料编号
        /// </summary>
        [SugarColumn(ColumnName = "F_Code")]        
        public string Code { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        [SugarColumn(ColumnName = "F_xName")]        
        public string XName { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        [SugarColumn(ColumnName = "F_xType")]        
        public string XType { get; set; }
        
        /// <summary>
        /// 基本单位
        /// </summary>
        [SugarColumn(ColumnName = "F_DefaultUnit")]        
        public string DefaultUnit { get; set; }
        
        /// <summary>
        /// 总有效期(天）
        /// </summary>
        [SugarColumn(ColumnName = "F_ValidDays")]        
        public int? ValidDays { get; set; }
        
        /// <summary>
        /// 静置时间
        /// </summary>
        [SugarColumn(ColumnName = "F_StayHours")]        
        public int StayHours { get; set; }
        
        /// <summary>
        /// 是否批次管理 1:是；0：否
        /// </summary>
        [SugarColumn(ColumnName = "F_BatchManageFlag")]        
        public int? BatchManageFlag { get; set; }
        
        /// <summary>
        /// 规格型号
        /// </summary>
        [SugarColumn(ColumnName = "F_Specification")]        
        public string Specification { get; set; }
        
        /// <summary>
        /// 最后更新时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
    }
}