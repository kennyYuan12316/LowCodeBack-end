using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 区域信息
    /// </summary>
    [SugarTable("zjn_wms_region")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsRegionEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 区域编号
        /// </summary>
        [SugarColumn(ColumnName = "F_RegionNo")]        
        public string RegionNo { get; set; }
        
        /// <summary>
        /// 区域名称
        /// </summary>
        [SugarColumn(ColumnName = "F_RegionName")]        
        public string RegionName { get; set; }
        
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
        
    }
}