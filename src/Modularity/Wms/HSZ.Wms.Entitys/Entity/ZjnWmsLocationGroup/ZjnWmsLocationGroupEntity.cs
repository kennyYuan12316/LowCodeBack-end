using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 货位分组信息
    /// </summary>
    [SugarTable("zjn_wms_location_group")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsLocationGroupEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 货位分组编号
        /// </summary>
        [SugarColumn(ColumnName = "F_GroupNo")]        
        public string GroupNo { get; set; }
        
        /// <summary>
        /// 货位分组编号
        /// </summary>
        [SugarColumn(ColumnName = "F_GroupName")]        
        public string GroupName { get; set; }
        
        /// <summary>
        /// 货位分组描述
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
        /// 创建日期
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public string CreateTime { get; set; }
        
    }
}