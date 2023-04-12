using System;

namespace HSZ.wms.Entitys.Dto.ZjnReportProductstructurePack
{
    /// <summary>
    /// 库存结构分析（集成）输入参数
    /// </summary>
    public class ZjnReportProductstructurePackListOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        public string F_ProductsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string F_ProductsName { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        public string F_Batch { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? F_EntryTime { get; set; }
        
        /// <summary>
        /// 物料数量
        /// </summary>
        public int? F_ProductsQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string F_ProductsUnit { get; set; }
        
        /// <summary>
        /// 库龄
        /// </summary>
        public string F_LocationAge { get; set; }
        
        /// <summary>
        /// 集成方式
        /// </summary>
        public string F_CombineType { get; set; }
        
        /// <summary>
        /// 项目号
        /// </summary>
        public string F_ProjectNum { get; set; }
        
        /// <summary>
        /// 包装方式
        /// </summary>
        public string F_PackingType { get; set; }
        
        /// <summary>
        /// 电池型号
        /// </summary>
        public string F_BatteryType { get; set; }
        
        /// <summary>
        /// 电池数量
        /// </summary>
        public int? F_BatteryCount { get; set; }
        
        /// <summary>
        /// 产品标识
        /// </summary>
        public string F_ProductionMark { get; set; }
        
        /// <summary>
        /// 库存标识
        /// </summary>
        public string F_InventoryMark { get; set; }
        
        /// <summary>
        /// 等级标识
        /// </summary>
        public string F_ClassMark { get; set; }
        
        /// <summary>
        /// 电压
        /// </summary>
        public double F_Voltage { get; set; }
        
        /// <summary>
        /// 安时
        /// </summary>
        public double F_Ah { get; set; }
        
        /// <summary>
        /// 电池容量（GWh）
        /// </summary>
        public double F_BatteryCapacity { get; set; }
        
        /// <summary>
        /// 生产时间
        /// </summary>
        public DateTime? F_ProductionTime { get; set; }
        
        /// <summary>
        /// 生产线
        /// </summary>
        public string F_LineNum { get; set; }
        
        /// <summary>
        /// 生产线状态
        /// </summary>
        public string F_LineStatus { get; set; }
        
        /// <summary>
        /// 位置
        /// </summary>
        public string F_Location { get; set; }
        
        /// <summary>
        /// 是否冻结
        /// </summary>
        public string F_Freeze { get; set; }
        
        /// <summary>
        /// 冻结原因
        /// </summary>
        public string F_FreezeResult { get; set; }
        
    }
}