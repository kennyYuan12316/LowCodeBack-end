using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 设备信息
    /// </summary>
    [SugarTable("zjn_hard_list")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnHardListEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 设备编号
        /// </summary>
        [SugarColumn(ColumnName = "F_HardNo")]        
        public string HardNo { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        [SugarColumn(ColumnName = "F_HardName")]        
        public string HardName { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(ColumnName = "F_Type")]        
        public int? Type { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(ColumnName = "F_Status")]        
        public int? Status { get; set; }
        
        /// <summary>
        /// 0真实 1虚拟
        /// </summary>
        [SugarColumn(ColumnName = "F_FictitiousHard")]        
        public int? FictitiousHard { get; set; }
        
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