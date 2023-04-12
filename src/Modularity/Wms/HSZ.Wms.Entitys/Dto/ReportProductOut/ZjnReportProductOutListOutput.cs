using System;

namespace HSZ.wms.Entitys.Dto.ZjnReportProductOut
{
    /// <summary>
    /// 成品出库单输入参数
    /// </summary>
    public class ZjnReportProductOutListOutput
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
        /// 出库单
        /// </summary>
        public string F_OutOrder { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public string F_BusinessType { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        public string F_Batch { get; set; }
        
        /// <summary>
        /// 出库时间
        /// </summary>
        public DateTime? F_OutTime { get; set; }
        
        /// <summary>
        /// 出库时间确认
        /// </summary>
        public DateTime? F_OutTimeConfirm { get; set; }
        
        /// <summary>
        /// 出库站台
        /// </summary>
        public string F_OutStation { get; set; }
        
        /// <summary>
        /// 出库数量
        /// </summary>
        public int? F_OutQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string F_ProductsUnit { get; set; }
        
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
        
    }
}