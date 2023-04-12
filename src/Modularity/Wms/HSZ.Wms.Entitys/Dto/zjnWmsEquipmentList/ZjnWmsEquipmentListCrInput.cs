using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsEquipmentList
{
    /// <summary>
    /// 设备入库管理修改输入参数
    /// </summary>
    public class ZjnWmsEquipmentListCrInput
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string equipmentSerialNumber { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        public string deviceName { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }
        

        public string theBinding { get; set; }

        /// <summary>
        /// 绑定终点设备
        /// </summary>
        public string positionTo { get; set; }
    }
}