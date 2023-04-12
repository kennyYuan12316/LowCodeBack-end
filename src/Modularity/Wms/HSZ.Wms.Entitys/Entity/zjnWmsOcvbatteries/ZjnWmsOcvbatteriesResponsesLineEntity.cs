using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// OCV
    /// </summary>
    [SugarTable("zjn_wms_ocvbatteries_responsesLine")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsOcvbatteriesResponsesLineEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 主表id
        /// </summary>
        [SugarColumn(ColumnName = "F_batteriesId")]        
        public string BatteriesId { get; set; }
        
        /// <summary>
        /// 托盘号
        /// </summary>
        [SugarColumn(ColumnName = "F_container")]        
        public string Container { get; set; }
        
        /// <summary>
        /// 产品每次
        /// </summary>
        [SugarColumn(ColumnName = "F_productionCode")]        
        public string ProductionCode { get; set; }
        
        /// <summary>
        /// 质检结果
        /// </summary>
        [SugarColumn(ColumnName = "F_inspectionResult")]        
        public string InspectionResult { get; set; }
        
        /// <summary>
        /// 产品标识
        /// </summary>
        [SugarColumn(ColumnName = "F_productSign")]        
        public string ProductSign { get; set; }
        
        /// <summary>
        /// 产品单号
        /// </summary>
        [SugarColumn(ColumnName = "F_productOrder")]        
        public string ProductOrder { get; set; }
        
        /// <summary>
        /// 电池等级
        /// </summary>
        [SugarColumn(ColumnName = "F_batterGradeNo")]        
        public string BatterGradeNo { get; set; }
        
        /// <summary>
        /// 通道号
        /// </summary>
        [SugarColumn(ColumnName = "F_positionNo")]        
        public string PositionNo { get; set; }
        
        /// <summary>
        /// 容量
        /// </summary>
        [SugarColumn(ColumnName = "F_capacity")]        
        public string Capacity { get; set; }
        
        /// <summary>
        /// OCV1电压
        /// </summary>
        [SugarColumn(ColumnName = "F_voltageOcv1")]        
        public string VoltageOcv1 { get; set; }
        
        /// <summary>
        /// OCV2电压
        /// </summary>
        [SugarColumn(ColumnName = "F_voltageOcv2")]        
        public string VoltageOcv2 { get; set; }
        
        /// <summary>
        /// 交流电阻
        /// </summary>
        [SugarColumn(ColumnName = "F_resistanceAc")]        
        public string ResistanceAc { get; set; }
        
        /// <summary>
        /// 直流电阻
        /// </summary>
        [SugarColumn(ColumnName = "F_resistanceDc")]        
        public string ResistanceDc { get; set; }
        
        /// <summary>
        /// K值
        /// </summary>
        [SugarColumn(ColumnName = "F_kValue")]        
        public string KValue { get; set; }
        
        /// <summary>
        /// 生产日期
        /// </summary>
        [SugarColumn(ColumnName = "F_productDate")]        
        public string ProductDate { get; set; }
        
        /// <summary>
        /// 生产线
        /// </summary>
        [SugarColumn(ColumnName = "F_productionLine")]        
        public string ProductionLine { get; set; }
        
        /// <summary>
        /// OCV2测试时间
        /// </summary>
        [SugarColumn(ColumnName = "F_ocv2Date")]        
        public DateTime? Ocv2Date { get; set; }
        
        /// <summary>
        /// 具体产线
        /// </summary>
        [SugarColumn(ColumnName = "F_prodline")]        
        public string Prodline { get; set; }
        
        /// <summary>
        /// 物料数量，入库时必输
        /// </summary>
        [SugarColumn(ColumnName = "qty")]        
        public int? Qty { get; set; }
        
        /// <summary>
        /// ocv3Value-OCV3测试值
        /// </summary>
        [SugarColumn(ColumnName = "F_attribute1")]        
        public string Attribute1 { get; set; }
        
        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(ColumnName = "F_IsDeleted")]        
        public int? IsDeleted { get; set; }
        
    }
}