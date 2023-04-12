using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 立库货位信息
    /// </summary>
    [SugarTable("zjn_base_location_asrs")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBaseLocationAsrsEntity
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
        /// 设备编号（堆垛机）
        /// </summary>
        [SugarColumn(ColumnName = "F_DeviceNo")]        
        public string DeviceNo { get; set; }
        
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
        /// 货位状态：0空 1满 2未满 3故障 4火警 5静置中 6静置完成 7预约 8禁用
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
        /// 仓库编号
        /// </summary>
        [SugarColumn(ColumnName = "F_ByWarehouse")]        
        public string ByWarehouse { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(ColumnName = "F_IsDelete")]
        public int? IsDelete { get; set; }

        /// <summary>
        /// 货位类型
        /// </summary>
        [SugarColumn(ColumnName = "F_type")]
        public string type { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        [SugarColumn(ColumnName = "F_Legion")]
        public string Legion { get; set; }

    }
}