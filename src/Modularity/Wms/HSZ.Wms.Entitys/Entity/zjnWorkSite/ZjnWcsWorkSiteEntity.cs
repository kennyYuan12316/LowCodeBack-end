using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 站点信息管理
    /// </summary>
    [SugarTable("zjn_wcs_work_site")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWcsWorkSiteEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 站点ID
        /// </summary>
        [SugarColumn(ColumnName = "StationID")]        
        public string StationId { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "Capion")]        
        public string Capion { get; set; }
        
        /// <summary>
        /// 设备ID
        /// </summary>
        [SugarColumn(ColumnName = "DeviceID")]        
        public string DeviceId { get; set; }
        
        /// <summary>
        /// 有效
        /// </summary>
        [SugarColumn(ColumnName = "IsActive")]        
        public bool IsActive { get; set; }
        
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
        /// 站类别
        /// </summary>
        [SugarColumn(ColumnName = "StationType")]        
        public string StationType { get; set; }
        
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

        /// <summary>
        /// 行
        /// </summary>
        [SugarColumn(ColumnName = "F_Row")]
        public int? Row { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        [SugarColumn(ColumnName = "F_Cell")]
        public int? Cell { get; set; }

        /// <summary>
        /// 层
        /// </summary>
        [SugarColumn(ColumnName = "F_Layer")]
        public int? Layer { get; set; }


        /// <summary>
        /// 行
        /// </summary>
        [SugarColumn(ColumnName = "F_Row2")]
        public int? Row2 { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        [SugarColumn(ColumnName = "F_Cell2")]
        public int? Cell2 { get; set; }

        /// <summary>
        /// 层2
        /// </summary>
        [SugarColumn(ColumnName = "F_Layer2")]
        public int? Layer2 { get; set; }

    }
}