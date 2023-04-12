using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsRoadwayInboundPlan
{
    /// <summary>
    /// 巷道入库策略均衡修改输入参数
    /// </summary>
    public class ZjnWmsRoadwayInboundPlanCrInput
    {
        /// <summary>
        /// 巷道分组编号
        /// </summary>
        public string roadwayGroupNo { get; set; }
        
        /// <summary>
        /// 巷道分组名称
        /// </summary>
        public string roadwayGroupName { get; set; }
        
        /// <summary>
        /// 当前巷道
        /// </summary>
        public string nowroadwayGroup { get; set; }
        
        /// <summary>
        /// 当前顺序
        /// </summary>
        public int? nowroadwayGroupCode { get; set; }
        
    }
}