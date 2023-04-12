using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.HltHrJob
{
    /// <summary>
    /// 在职状态管理列表查询输入
    /// </summary>
    public class HltHrJobListQueryInput : PageInputBase
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
        public string f2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string f3 { get; set; }
        
    }
}