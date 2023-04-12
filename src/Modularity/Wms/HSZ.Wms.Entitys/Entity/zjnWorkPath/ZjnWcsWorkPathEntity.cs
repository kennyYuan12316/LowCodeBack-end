using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 路径信息管理
    /// </summary>
    [SugarTable("zjn_wcs_work_path")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWcsWorkPathEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 路径编号 
        /// </summary>
        [SugarColumn(ColumnName = "PathID")]        
        public string PathId { get; set; }
        
        /// <summary>
        /// 路径名称
        /// </summary>
        [SugarColumn(ColumnName = "StationFrom")]        
        public string StationFrom { get; set; }
        
        /// <summary>
        /// 起点站点
        /// </summary>
        [SugarColumn(ColumnName = "DeviceFrom")]        
        public string DeviceFrom { get; set; }
        
        /// <summary>
        /// 输送设备
        /// </summary>
        [SugarColumn(ColumnName = "StationTo")]        
        public string StationTo { get; set; }
        
        /// <summary>
        /// 终点站点
        /// </summary>
        [SugarColumn(ColumnName = "DeviceTo")]        
        public string DeviceTo { get; set; }
        
        /// <summary>
        /// 路径类型
        /// </summary>
        [SugarColumn(ColumnName = "PathType")]        
        public int? PathType { get; set; }
        
        /// <summary>
        /// 有效
        /// </summary>
        [SugarColumn(ColumnName = "IsActive")]        
        public int? IsActive { get; set; }
        
        /// <summary>
        /// 区域
        /// </summary>
        [SugarColumn(ColumnName = "Region")]        
        public string Region { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "Descrip")]        
        public string Descrip { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateUser")]        
        public string CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 最后修改者
        /// </summary>
        [SugarColumn(ColumnName = "F_ModifiedUser")]        
        public string ModifiedUser { get; set; }
        
        /// <summary>
        /// 最后修改时间
        /// </summary>
        [SugarColumn(ColumnName = "F_ModifiedTime")]        
        public DateTime? ModifiedTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        [SugarColumn(ColumnName = "F_EnabledMark")]        
        public int? EnabledMark { get; set; }
        
        /// <summary>
        /// 是否删除 0未删除 1删除
        /// </summary>
        [SugarColumn(ColumnName = "F_IsDelete")]        
        public int? IsDelete { get; set; }
        
    }
}