using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 巷道入库策略均衡
    /// </summary>
    [SugarTable("zjn_roadway_inbound_plan")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnRoadwayInboundPlanEntity
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 巷道分组编号
        /// </summary>
        [SugarColumn(ColumnName = "F_roadwayGroupNo")]        
        public string RoadwayGroupNo { get; set; }
        
        /// <summary>
        /// 巷道分组名称
        /// </summary>
        [SugarColumn(ColumnName = "F_roadwayGroupName")]        
        public string RoadwayGroupName { get; set; }
        
        /// <summary>
        /// 当前巷道
        /// </summary>
        [SugarColumn(ColumnName = "F_NowroadwayGroup")]        
        public string NowroadwayGroup { get; set; }
        
        /// <summary>
        /// 当前顺序
        /// </summary>
        [SugarColumn(ColumnName = "F_NowroadwayGroupCode")]        
        public int? NowroadwayGroupCode { get; set; }
        
    }
}