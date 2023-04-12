using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.HltHrUnusual
{
    /// <summary>
    /// 实时异常列表查询输入
    /// </summary>
    public class HltHrUnusualListQueryInput : PageInputBase
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
        public string m1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string m3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string m4 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string m5 { get; set; }
        
    }
}