using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.HltHrError
{
    /// <summary>
    /// 错误日志管理列表查询输入
    /// </summary>
    public class HltHrErrorListQueryInput : PageInputBase
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
        public string k2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string k5 { get; set; }
        
    }
}