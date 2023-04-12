using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// OCV
    /// </summary>
    [SugarTable("zjn_wms_ocvbatteries")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsOcvbatteriesEntity
    {
        /// <summary>
        /// 唯一id
        /// </summary>
        [SugarColumn(ColumnName = "F_id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 单据指令
        /// </summary>
        [SugarColumn(ColumnName = "F_InstructionNum")]        
        public string InstructionNum { get; set; }
        
        /// <summary>
        /// 容器编码
        /// </summary>
        [SugarColumn(ColumnName = "F_containerCode")]        
        public string ContainerCode { get; set; }
        
        /// <summary>
        /// 产品条码
        /// </summary>
        [SugarColumn(ColumnName = "F_productionCode")]        
        public string ProductionCode { get; set; }
        
        /// <summary>
        /// 批次号
        /// </summary>
        [SugarColumn(ColumnName = "F_lot")]        
        public string Lot { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        [SugarColumn(ColumnName = "F_materialCode")]        
        public string MaterialCode { get; set; }
        
        /// <summary>
        /// 是否输出
        /// </summary>
        [SugarColumn(ColumnName = "F_IsDeleted")]        
        public int? IsDeleted { get; set; }
        
    }
}