using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.HltHrTemplate
{
    /// <summary>
    /// 系统模板参数管理列表查询输入
    /// </summary>
    public class HltHrTemplateListQueryInput : PageInputBase
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
        public string g2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string g3 { get; set; }
        
    }
}