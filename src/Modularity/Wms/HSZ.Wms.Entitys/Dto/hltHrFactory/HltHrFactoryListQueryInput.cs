using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.HltHrFactory
{
    /// <summary>
    /// 厂区管理列表查询输入
    /// </summary>
    public class HltHrFactoryListQueryInput : PageInputBase
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
        public string e2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string e3 { get; set; }
        
    }
}