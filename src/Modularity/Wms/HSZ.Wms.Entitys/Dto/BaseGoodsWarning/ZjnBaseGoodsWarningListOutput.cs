using System;

namespace HSZ.wms.Entitys.Dto.ZjnBaseGoodsWarning
{
    /// <summary>
    /// 物料库存预警输入参数
    /// </summary>
    public class ZjnBaseGoodsWarningListOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string F_Description { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        public string F_ProductsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string F_ProductsName { get; set; }
        
        /// <summary>
        /// 物料数量
        /// </summary>
        public int? F_ProductsQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string F_ProductsUnit { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        public string F_Batch { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? F_EntryTime { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        public string F_ProductsType { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        public string F_ProductsSupplier { get; set; }
        
        /// <summary>
        /// 库存上限
        /// </summary>
        public int? F_InventoryMax { get; set; }
        
        /// <summary>
        /// 库存下限
        /// </summary>
        public int? F_InventoryMin { get; set; }
        
        /// <summary>
        /// 预警原因
        /// </summary>
        public string F_WarningResult { get; set; }
        
        /// <summary>
        /// 保质期
        /// </summary>
        public string F_ExpirationDate { get; set; }
        
        /// <summary>
        /// 生产时间
        /// </summary>
        public DateTime? F_ProductionTime { get; set; }
        
        /// <summary>
        /// 失效时间
        /// </summary>
        public DateTime? F_FailureTime { get; set; }
        
        /// <summary>
        /// 预警周期
        /// </summary>
        public string F_WarningCycle { get; set; }
        
        /// <summary>
        /// 保质期预警
        /// </summary>
        public DateTime? F_WarningTime { get; set; }
        
    }
}