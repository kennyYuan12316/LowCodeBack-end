using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 巷道分组信息
    /// </summary>
    [SugarTable("zjn_wms_roadway_group")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsRoadwayGroupEntity
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 巷道分组编号
        /// </summary>
        [SugarColumn(ColumnName = "F_roadwayNo")]        
        public string RoadwayNo { get; set; }
        
        /// <summary>
        /// 巷道分组名称
        /// </summary>
        [SugarColumn(ColumnName = "F_roadwayName")]        
        public string RoadwayName { get; set; }
        
        /// <summary>
        /// 巷道分组描述
        /// </summary>
        [SugarColumn(ColumnName = "F_Description")]        
        public string Description { get; set; }
        
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