using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.HltHrBusiness
{
    /// <summary>
    /// 业务模板组管理列表查询输入
    /// </summary>
    public class HltHrBusinessListQueryInput : PageInputBase
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
        public string d2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string d3 { get; set; }
        
    }
}