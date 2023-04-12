using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.HltHrLog
{
    /// <summary>
    /// 日志记录列表查询输入
    /// </summary>
    public class HltHrLogListQueryInput : PageInputBase
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
        public string j2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string j3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string j5 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string j8 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string j9 { get; set; }
        
    }
}