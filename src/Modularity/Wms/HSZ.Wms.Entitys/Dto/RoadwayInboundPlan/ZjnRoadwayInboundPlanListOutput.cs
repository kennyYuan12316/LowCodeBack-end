using System;

namespace HSZ.wms.Entitys.Dto.ZjnRoadwayInboundPlan
{
    /// <summary>
    /// 巷道入库策略均衡输入参数
    /// </summary>
    public class ZjnRoadwayInboundPlanListOutput
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 巷道分组编号
        /// </summary>
        public string F_roadwayGroupNo { get; set; }
        
        /// <summary>
        /// 巷道分组名称
        /// </summary>
        public string F_roadwayGroupName { get; set; }
        
        /// <summary>
        /// 当前巷道
        /// </summary>
        public string F_NowroadwayGroup { get; set; }
        
        /// <summary>
        /// 当前顺序
        /// </summary>
        public int? F_NowroadwayGroupCode { get; set; }
        
    }
}