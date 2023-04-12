using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.zjnWmsMaterialInventory
{

    /// <summary>
    /// 立库库存信息列表查询输入
    /// </summary>
    public class zjnWmsMaterialInventoryListQueryInput : PageInputBase
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
        /// 物料单位
        /// </summary>
        public string F_ProductsUnit { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        public string F_ProductsType { get; set; }
        
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

        /// <summary>
        /// 物料容器
        /// </summary>
        public string F_ProductsContainer { get; set; }

    }
}