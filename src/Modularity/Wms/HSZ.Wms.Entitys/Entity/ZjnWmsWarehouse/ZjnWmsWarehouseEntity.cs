using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 仓库信息
    /// </summary>
    [SugarTable("zjn_wms_warehouse")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsWarehouseEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 仓库编号
        /// </summary>
        [SugarColumn(ColumnName = "F_WarehouseNo")]        
        public string WarehouseNo { get; set; }
        
        /// <summary>
        /// 仓库名称
        /// </summary>
        [SugarColumn(ColumnName = "F_WarehouseName")]        
        public string WarehouseName { get; set; }
        
        /// <summary>
        /// 区域编号
        /// </summary>
        [SugarColumn(ColumnName = "F_RegionNo")]        
        public string RegionNo { get; set; }
        
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
        /// 是否删除
        /// </summary>
        [SugarColumn(ColumnName = "F_IsDelete")]
        public int? IsDelete { get; set; }

    }
}