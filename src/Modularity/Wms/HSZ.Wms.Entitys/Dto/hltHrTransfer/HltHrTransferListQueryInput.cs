using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.HltHrTransfer
{
    /// <summary>
    /// 实时调用信息列表查询输入
    /// </summary>
    public class HltHrTransferListQueryInput : PageInputBase
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
        /// 
        /// </summary>
        public string l2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string l3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string l5 { get; set; }
        
    }
}