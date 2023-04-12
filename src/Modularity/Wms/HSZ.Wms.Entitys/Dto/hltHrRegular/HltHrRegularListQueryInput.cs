using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.HltHrRegular
{
    /// <summary>
    /// 定时任务列表查询输入
    /// </summary>
    public class HltHrRegularListQueryInput : PageInputBase
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
        public string i2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string i3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string i4 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string i6 { get; set; }
        
    }
}