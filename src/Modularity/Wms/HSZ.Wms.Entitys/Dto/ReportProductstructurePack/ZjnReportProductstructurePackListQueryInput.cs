using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnReportProductstructurePack
{
    /// <summary>
    /// 库存结构分析（集成）列表查询输入
    /// </summary>
    public class ZjnReportProductstructurePackListQueryInput : PageInputBase
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
        
    }
}