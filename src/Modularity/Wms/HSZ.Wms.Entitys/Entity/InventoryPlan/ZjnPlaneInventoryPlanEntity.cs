using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 盘点计划
    /// </summary>
    [SugarTable("zjn_plane_InventoryPlan")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnPlaneInventoryPlanEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 盘点单据号
        /// </summary>
        [SugarColumn(ColumnName = "F_InventoryNo")]        
        public string InventoryNo { get; set; }
        
        /// <summary>
        /// 盘点类型
        /// </summary>
        [SugarColumn(ColumnName = "F_InventoryType")]        
        public string InventoryType { get; set; }
        
        /// <summary>
        /// 盘点计划开始时间
        /// </summary>
        [SugarColumn(ColumnName = "F_StartTime")]        
        public DateTime? StartTime { get; set; }
        
        /// <summary>
        /// 盘点计划结束时间
        /// </summary>
        [SugarColumn(ColumnName = "F_EndTime")]        
        public DateTime? EndTime { get; set; }
        
        /// <summary>
        /// 描述（备注）
        /// </summary>
        [SugarColumn(ColumnName = "F_Description")]        
        public string Description { get; set; }
        
        /// <summary>
        /// 有效标志
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
        
        /// <summary>
        /// 修改用户
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyUserId")]        
        public string LastModifyUserId { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyTime")]        
        public DateTime? LastModifyTime { get; set; }
        
        /// <summary>
        /// 备用字段
        /// </summary>
        [SugarColumn(ColumnName = "case1")]        
        public string Case1 { get; set; }
        
        /// <summary>
        /// 备用字段
        /// </summary>
        [SugarColumn(ColumnName = "case2")]        
        public string Case2 { get; set; }
        
        /// <summary>
        /// 备用字段
        /// </summary>
        [SugarColumn(ColumnName = "case3")]        
        public string Case3 { get; set; }
        
        /// <summary>
        /// 备用字段
        /// </summary>
        [SugarColumn(ColumnName = "case4")]        
        public string Case4 { get; set; }
        
        /// <summary>
        /// 备用字段
        /// </summary>
        [SugarColumn(ColumnName = "case5")]        
        public string Case5 { get; set; }
        
    }
}