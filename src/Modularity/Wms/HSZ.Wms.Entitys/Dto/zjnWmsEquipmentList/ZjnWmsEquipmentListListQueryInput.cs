using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsEquipmentList
{
    /// <summary>
    /// 设备入库管理列表查询输入
    /// </summary>
    public class ZjnWmsEquipmentListListQueryInput : PageInputBase
    {
        /// <summary>
        /// 选择导出数据key
        /// </summary>
        public string selectKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int dataType { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string F_EquipmentSerialNumber { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        public string F_DeviceName { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public string F_Type { get; set; }
        
        public string F_TheBinding { get; set; }
        /// <summary>
        /// 绑定终点设备
        /// </summary>
        public string F_PositionTo { get; set; }
    }
}