using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.HltHrT5
{
    /// <summary>
    /// 接收类型列表查询输入
    /// </summary>
    public class HltHrT5ListQueryInput : PageInputBase
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
        public string t2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string t3 { get; set; }
        
    }
}