using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsGoodsWeight
{
    /// <summary>
    /// 物料承重配置列表查询输入
    /// </summary>
    public class ZjnWmsGoodsWeightListQueryInput : PageInputBase
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
        /// 最大承重
        /// </summary>
        public string F_Max { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string F_Unit { get; set; }
        
    }
}