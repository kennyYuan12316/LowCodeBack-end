using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.HltHrShineupon
{
    /// <summary>
    /// 映射管理列表查询输入
    /// </summary>
    public class HltHrShineuponListQueryInput : PageInputBase
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
        public string h2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string h3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string h4 { get; set; }
        
    }
}