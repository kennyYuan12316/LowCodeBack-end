using System;

namespace HSZ.wms.Entitys.Dto.ZjnRecordMaterialInventory
{
    /// <summary>
    /// 库存流水输入参数
    /// </summary>
    public class ZjnRecordMaterialInventoryListOutput
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
        /// 32位批次号
        /// </summary>
        public string F_Batch { get; set; }
        
        /// <summary>
        /// 质量状态
        /// </summary>
        public string F_Quality { get; set; }
        
        /// <summary>
        /// 位置
        /// </summary>
        public string F_Location { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? F_EntryTime { get; set; }
        
        /// <summary>
        /// 订单
        /// </summary>
        public string F_Order { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public string F_BusinessType { get; set; }
        
        /// <summary>
        /// 入库单
        /// </summary>
        public string F_EntryOrder { get; set; }
        
        /// <summary>
        /// 出库单
        /// </summary>
        public string F_OutOrder { get; set; }
        
        /// <summary>
        /// 操作
        /// </summary>
        public string F_Operation { get; set; }
        
    }
}