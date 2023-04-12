using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 货位信息
    /// </summary>
    [SugarTable("zjn_plane_location")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnPlaneLocationEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 货位编号
        /// </summary>
        [SugarColumn(ColumnName = "F_LocationNo")]        
        public string LocationNo { get; set; }
        
        /// <summary>
        /// 巷道编号
        /// </summary>
        [SugarColumn(ColumnName = "F_AisleNo")]        
        public string AisleNo { get; set; }
        
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
        /// 深
        /// </summary>
        [SugarColumn(ColumnName = "F_Depth")]        
        public int? Depth { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayNo")]        
        public string TrayNo { get; set; }

        /// <summary>
        /// 上次货位状态
        /// </summary>
        [SugarColumn(ColumnName = "F_LastStatus")]        
        public int? LastStatus { get; set; }

        /// <summary>
        /// 货位状态：详见数据字典，我懒得写了
        /// </summary>
        [SugarColumn(ColumnName = "F_LocationStatus")]        
        public string LocationStatus { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_Description")]        
        public string Description { get; set; }
        
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
        /// 有效标志
        /// </summary>
        [SugarColumn(ColumnName = "F_EnabledMark")]        
        public int? EnabledMark { get; set; }
        

        /// <summary>
        /// 所属库位
        /// </summary>
        [SugarColumn(ColumnName = "F_ByWarehouse")]
        public string ByWarehouse { get; set; }
    }
}