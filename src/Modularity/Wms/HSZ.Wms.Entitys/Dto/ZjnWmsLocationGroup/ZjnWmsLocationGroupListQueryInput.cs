using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsLocationGroup
{
    /// <summary>
    /// 货位分组信息列表查询输入
    /// </summary>
    public class ZjnWmsLocationGroupListQueryInput : PageInputBase
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
        /// 货位分组编号
        /// </summary>
        public string F_GroupNo { get; set; }
        
        /// <summary>
        /// 货位分组编号
        /// </summary>
        public string F_GroupName { get; set; }
        
    }
}