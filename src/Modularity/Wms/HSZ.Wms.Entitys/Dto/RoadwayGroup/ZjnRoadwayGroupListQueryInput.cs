using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnRoadwayGroup
{
    /// <summary>
    /// 巷道分组信息列表查询输入
    /// </summary>
    public class ZjnRoadwayGroupListQueryInput : PageInputBase
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
        /// 巷道分组编号
        /// </summary>
        public string F_roadwayNo { get; set; }
        
        /// <summary>
        /// 巷道分组名称
        /// </summary>
        public string F_roadwayName { get; set; }
        
    }
}