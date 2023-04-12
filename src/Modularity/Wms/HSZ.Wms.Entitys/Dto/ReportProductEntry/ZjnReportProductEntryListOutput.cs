using System;

namespace HSZ.wms.Entitys.Dto.ZjnReportProductEntry
{
    /// <summary>
    /// 成品入库单输入参数
    /// </summary>
    public class ZjnReportProductEntryListOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 所属托盘
        /// </summary>
        public string F_Tray { get; set; }
        
        /// <summary>
        /// 电芯条码
        /// </summary>
        public string F_BatteryCode { get; set; }
        
        /// <summary>
        /// 生产单号
        /// </summary>
        public string F_ProductionOrder { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        public string F_ProductsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string F_ProductsName { get; set; }
        
        /// <summary>
        /// 生产时间
        /// </summary>
        public DateTime? F_ProductionTime { get; set; }
        
        /// <summary>
        /// 入库单
        /// </summary>
        public string F_EntryOrder { get; set; }
        
        /// <summary>
        /// 组盘时间
        /// </summary>
        public DateTime? F_ComboTime { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? F_EntryTime { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public string F_BusinessType { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        public string F_Batch { get; set; }
        
        /// <summary>
        /// 物料数量
        /// </summary>
        public int? F_ProductsQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string F_ProductsUnit { get; set; }
        
        /// <summary>
        /// 质量状态
        /// </summary>
        public string F_Quality { get; set; }
        
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
        /// 交流电阻
        /// </summary>
        public double F_ACR { get; set; }
        
        /// <summary>
        /// 直流电阻
        /// </summary>
        public double F_DCR { get; set; }
        
        /// <summary>
        /// K值
        /// </summary>
        public double F_KValue { get; set; }
        
        /// <summary>
        /// 生产线
        /// </summary>
        public string F_LineNum { get; set; }
        
        /// <summary>
        /// 通道号
        /// </summary>
        public int? F_ChannelNum { get; set; }
        
    }
}