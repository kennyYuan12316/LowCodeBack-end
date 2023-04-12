using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 巷道分组信息
    /// </summary>
    [SugarTable("zjn_roadway_group_details")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnRoadwayGroupDetailsEntity
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 巷道编号
        /// </summary>
        [SugarColumn(ColumnName = "F_roadwayDetailsNo")]        
        public string RoadwayDetailsNo { get; set; }
        
        /// <summary>
        /// 巷道分组编号
        /// </summary>
        [SugarColumn(ColumnName = "F_roadwayCode")]        
        public string RoadwayCode { get; set; }
        
        /// <summary>
        /// 巷道名称
        /// </summary>
        [SugarColumn(ColumnName = "F_roadwayDetailsName")]        
        public string RoadwayDetailsName { get; set; }
        
        /// <summary>
        /// 巷道排序
        /// </summary>
        [SugarColumn(ColumnName = "F_roadwayDetailsGrade")]        
        public int? RoadwayDetailsGrade { get; set; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        [SugarColumn(ColumnName = "F_EnabledMark")]        
        public int? EnabledMark { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateUser")]        
        public string CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
    }
}