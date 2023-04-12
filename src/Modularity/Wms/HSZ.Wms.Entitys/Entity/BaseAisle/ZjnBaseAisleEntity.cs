using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 巷道信息
    /// </summary>
    [SugarTable("zjn_base_aisle")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBaseAisleEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 巷道编号
        /// </summary>
        [SugarColumn(ColumnName = "F_AisleNo")]        
        public string AisleNo { get; set; }
        
        /// <summary>
        /// 巷道名称
        /// </summary>
        [SugarColumn(ColumnName = "F_AisleName")]        
        public string AisleName { get; set; }
        
        /// <summary>
        /// 区域编号
        /// </summary>
        [SugarColumn(ColumnName = "F_RegionNo")]        
        public string RegionNo { get; set; }
        
        /// <summary>
        /// 仓库编号
        /// </summary>
        [SugarColumn(ColumnName = "F_WarehouseNo")]        
        public string WarehouseNo { get; set; }
        
        /// <summary>
        /// 堆垛机编号
        /// </summary>
        [SugarColumn(ColumnName = "F_StackerNo")]        
        public string StackerNo { get; set; }
        
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
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_IsDelete")]
        public int? IsDelete { get; set; }

    }
}