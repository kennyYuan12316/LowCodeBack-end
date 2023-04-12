using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 设备入库管理
    /// </summary>
    [SugarTable("zjn_wms_equipmentList")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsEquipmentListEntity
    {
        /// <summary>
        /// 唯一id
        /// </summary>
        [SugarColumn(ColumnName = "F_id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 设备编号
        /// </summary>
        [SugarColumn(ColumnName = "F_EquipmentSerialNumber")]        
        public string EquipmentSerialNumber { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        [SugarColumn(ColumnName = "F_DeviceName")]        
        public string DeviceName { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(ColumnName = "F_Type")]        
        public string Type { get; set; }
        
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
        /// 是否有效
        /// </summary>
        [SugarColumn(ColumnName = "F_EnabledMark")]        
        public int? EnabledMark { get; set; }
        
        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(ColumnName = "F_IsDeleted")]        
        public int? IsDeleted { get; set; }
        /// <summary>
        /// 绑定设备
        /// </summary>
        [SugarColumn(ColumnName = "F_TheBinding")]     
        public string TheBinding { get; set; }

        /// <summary>
        /// 绑定终点设备
        /// </summary>
        [SugarColumn(ColumnName = "F_PositionTo")]
        public string PositionTo { get; set; }
    }
}