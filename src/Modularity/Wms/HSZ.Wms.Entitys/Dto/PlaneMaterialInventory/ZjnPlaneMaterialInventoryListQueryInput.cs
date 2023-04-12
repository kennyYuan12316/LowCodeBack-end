using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneMaterialInventory
{
    /// <summary>
    /// 库存信息数据源列表查询输入
    /// </summary>
    public class ZjnPlaneMaterialInventoryListQueryInput : PageInputBase
    {
        /// <summary>
        /// 选择导出数据key
        /// </summary>
        public string selectKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int dataType { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string F_ProductsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string F_ProductsName { get; set; }
        
        /// <summary>
        /// 物料规格
        /// </summary>
        public string F_ProductsStyle { get; set; }

        public string F_ProductsLocation { get; set; }

        public string F_roductsBatch { get; set; }

        public string productsState { get; set; }

        public string productsUnit { get; set; }

        public decimal productsQuantity { get; set; }

        public string F_TheContainer { get; set; }

        public string F_ProductsNo { get; set; }

       

    }
}