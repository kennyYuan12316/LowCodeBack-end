using System;

namespace HSZ.wms.Entitys.Dto.ZjnBaseMaterialInventory
{
    /// <summary>
    /// 立库库存信息输入参数
    /// </summary>
    public class ZjnBaseMaterialInventoryListOutput
    {
        /// <summary>
        /// 主键
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
        /// 物料数量
        /// </summary>
        public decimal F_ProductsQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string F_ProductsUnit { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        public string F_ProductsType { get; set; }
        
        /// <summary>
        /// 物料规格
        /// </summary>
        public string F_ProductsStyle { get; set; }
        
        /// <summary>
        /// 物料等级
        /// </summary>
        public string F_ProductsGrade { get; set; }
        
        /// <summary>
        /// 物料状态
        /// </summary>
        public string F_ProductsState { get; set; }
        
        /// <summary>
        /// 物料批次
        /// </summary>
        public string F_ProductsBatch { get; set; }
        
        /// <summary>
        /// 物料货位
        /// </summary>
        public string F_ProductsLocation { get; set; }
        
    }
}