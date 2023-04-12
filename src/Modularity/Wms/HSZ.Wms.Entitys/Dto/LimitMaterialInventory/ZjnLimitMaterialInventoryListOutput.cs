using System;

namespace HSZ.wms.Entitys.Dto.ZjnLimitMaterialInventory
{
    /// <summary>
    /// 物料库存上下限输入参数
    /// </summary>
    public class ZjnLimitMaterialInventoryListOutput
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
        /// 所属仓库编码
        /// </summary>
        public string F_WareHouseCode { get; set; }
        
        /// <summary>
        /// 所属仓库
        /// </summary>
        public string F_WareHouse { get; set; }
        
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
        /// 库存上限
        /// </summary>
        public int? F_InventoryMax { get; set; }
        
        /// <summary>
        /// 库存下限
        /// </summary>
        public int? F_InventoryMin { get; set; }
        
    }
}