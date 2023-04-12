using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.HltHrEmployee
{
    /// <summary>
    /// 员工信息列表查询输入
    /// </summary>
    public class HltHrEmployeeListQueryInput : PageInputBase
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
        public string a1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string a2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string a3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string a4 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string a5 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string a9 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string a10 { get; set; }
        
    }
}