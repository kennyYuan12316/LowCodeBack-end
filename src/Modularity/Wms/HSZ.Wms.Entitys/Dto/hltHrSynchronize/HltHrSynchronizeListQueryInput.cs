using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.HltHrSynchronize
{
    /// <summary>
    /// 同步日志列表查询输入
    /// </summary>
    public class HltHrSynchronizeListQueryInput : PageInputBase
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
        public string n2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string n4 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string n6 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string n8 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string n10 { get; set; }
        
    }
}