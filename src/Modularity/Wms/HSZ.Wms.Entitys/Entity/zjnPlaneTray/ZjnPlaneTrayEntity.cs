using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 平面库托盘信息维护
    /// </summary>
    [SugarTable("zjn_plane_tray")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnPlaneTrayEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayNo")]        
        public string TrayNo { get; set; }
        
        /// <summary>
        /// 托盘类型（根据数据字典来）
        /// </summary>
        [SugarColumn(ColumnName = "F_Type")]        
        public int? Type { get; set; }
        
        /// <summary>
        /// 托盘状态（根据数据字典来）
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayState")]        
        public int? TrayState { get; set; }
        
        /// <summary>
        /// 是否删除 0未删除 1删除
        /// </summary>
        [SugarColumn(ColumnName = "F_IsDelete")]        
        public int? IsDelete { get; set; }
        
        /// <summary>
        /// 禁用原因
        /// </summary>
        [SugarColumn(ColumnName = "F_DisableMark")]        
        public string DisableMark { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateUser")]        
        public string CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 修改人
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