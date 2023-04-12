using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseMaterialInventoryDetail
{
    /// <summary>
    /// 库存明细列表查询输入
    /// </summary>
    public class ZjnBaseMaterialInventoryDetailListQueryInput : PageInputBase
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
        /// 32位批次号
        /// </summary>
        public string F_Batch { get; set; }
        
        /// <summary>
        /// 质量状态
        /// </summary>
        public string F_Quality { get; set; }
        
        /// <summary>
        /// 是否冻结
        /// </summary>
        public string F_Freeze { get; set; }
        
        /// <summary>
        /// 所属仓库
        /// </summary>
        public string F_WareHouse { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        public string F_EntryTime { get; set; }
        
    }
}