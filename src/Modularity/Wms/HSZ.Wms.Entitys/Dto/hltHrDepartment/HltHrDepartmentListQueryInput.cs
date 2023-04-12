using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.HltHrDepartment
{
    /// <summary>
    /// 部门信息列表查询输入
    /// </summary>
    public class HltHrDepartmentListQueryInput : PageInputBase
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
        public string b1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string b2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string b3 { get; set; }
        
    }
}