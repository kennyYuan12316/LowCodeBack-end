using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 成品库集成
    /// </summary>
    [SugarTable("zjn_report_product_pack")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnReportProductPackEntity
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
        /// 料架号
        /// </summary>
        [SugarColumn(ColumnName = "F_RackNum")]        
        public string RackNum { get; set; }
        
        /// <summary>
        /// 标签条码
        /// </summary>
        [SugarColumn(ColumnName = "F_Label")]        
        public string Label { get; set; }
        
        /// <summary>
        /// 生产单号
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductionOrder")]        
        public string ProductionOrder { get; set; }
        
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
        /// 集成方式
        /// </summary>
        [SugarColumn(ColumnName = "F_CombineType")]        
        public string CombineType { get; set; }
        
        /// <summary>
        /// 项目号
        /// </summary>
        [SugarColumn(ColumnName = "F_ProjectNum")]        
        public string ProjectNum { get; set; }
        
        /// <summary>
        /// 包装方式
        /// </summary>
        [SugarColumn(ColumnName = "F_PackingType")]        
        public string PackingType { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        [SugarColumn(ColumnName = "F_Batch")]        
        public string Batch { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        [SugarColumn(ColumnName = "F_EntryTime")]        
        public DateTime? EntryTime { get; set; }
        
        /// <summary>
        /// 物料数量
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsQuantity")]        
        public int? ProductsQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsUnit")]        
        public string ProductsUnit { get; set; }
        
        /// <summary>
        /// 质量状态
        /// </summary>
        [SugarColumn(ColumnName = "F_Quality")]        
        public string Quality { get; set; }
        
        /// <summary>
        /// 产品标识
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductionMark")]        
        public string ProductionMark { get; set; }
        
        /// <summary>
        /// 库存标识
        /// </summary>
        [SugarColumn(ColumnName = "F_InventoryMark")]        
        public string InventoryMark { get; set; }
        
        /// <summary>
        /// 等级标识
        /// </summary>
        [SugarColumn(ColumnName = "F_ClassMark")]        
        public string ClassMark { get; set; }
        
        /// <summary>
        /// 电压
        /// </summary>
        [SugarColumn(ColumnName = "F_Voltage")]        
        public double Voltage { get; set; }
        
        /// <summary>
        /// 安时
        /// </summary>
        [SugarColumn(ColumnName = "F_Ah")]        
        public double Ah { get; set; }
        
        /// <summary>
        /// 交流电阻
        /// </summary>
        [SugarColumn(ColumnName = "F_ACR")]        
        public double Acr { get; set; }
        
        /// <summary>
        /// 直流电阻
        /// </summary>
        [SugarColumn(ColumnName = "F_DCR")]        
        public double Dcr { get; set; }
        
        /// <summary>
        /// K值
        /// </summary>
        [SugarColumn(ColumnName = "F_KValue")]        
        public double KValue { get; set; }
        
        /// <summary>
        /// 电池容量（GWh）
        /// </summary>
        [SugarColumn(ColumnName = "F_BatteryCapacity")]        
        public double BatteryCapacity { get; set; }
        
        /// <summary>
        /// 生产时间
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductionTime")]        
        public DateTime? ProductionTime { get; set; }
        
        /// <summary>
        /// 生产线
        /// </summary>
        [SugarColumn(ColumnName = "F_LineNum")]        
        public string LineNum { get; set; }
        
        /// <summary>
        /// 生产线状态
        /// </summary>
        [SugarColumn(ColumnName = "F_LineStatus")]        
        public string LineStatus { get; set; }
        
        /// <summary>
        /// 库区
        /// </summary>
        [SugarColumn(ColumnName = "F_LogicLocation")]        
        public string LogicLocation { get; set; }
        
        /// <summary>
        /// 位置
        /// </summary>
        [SugarColumn(ColumnName = "F_Location")]        
        public string Location { get; set; }
        
        /// <summary>
        /// 是否冻结
        /// </summary>
        [SugarColumn(ColumnName = "F_Freeze")]        
        public string Freeze { get; set; }
        
        /// <summary>
        /// 冻结原因
        /// </summary>
        [SugarColumn(ColumnName = "F_FreezeResult")]        
        public string FreezeResult { get; set; }
        
    }
}