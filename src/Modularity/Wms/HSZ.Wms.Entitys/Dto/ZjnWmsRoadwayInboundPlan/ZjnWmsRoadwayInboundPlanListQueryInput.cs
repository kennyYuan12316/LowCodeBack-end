using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsRoadwayInboundPlan
{
    /// <summary>
    /// 巷道入库策略均衡列表查询输入
    /// </summary>
    public class ZjnWmsRoadwayInboundPlanListQueryInput : PageInputBase
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
        public string F_roadwayGroupNo { get; set; }
        
        /// <summary>
        /// 巷道分组名称
        /// </summary>
        public string F_roadwayGroupName { get; set; }
        
    }
}