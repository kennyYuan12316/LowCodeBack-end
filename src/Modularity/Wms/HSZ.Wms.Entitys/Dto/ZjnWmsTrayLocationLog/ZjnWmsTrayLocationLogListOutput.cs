using System;

namespace HSZ.wms.Entitys.Dto.ZjnWmsTrayLocationLog
{
    /// <summary>
    /// 托盘货位绑定履历表输入参数
    /// </summary>
    public class ZjnWmsTrayLocationLogListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 货物代码
        /// </summary>
        public string F_GoodsCode { get; set; }
        /// <summary>
        /// 货物名称
        /// </summary>
        public string F_GoodsCodeNmae { get; set; }

        ///物料类型
        public string F_GoodsType { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? F_Quantity { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        public string F_Unit { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string F_TrayNo { get; set; }

        /// <summary>
        /// 托盘名称
        /// </summary>
        public string F_TrayName { get; set; }
        /// <summary>
        /// 货位编号
        /// </summary>
        public string F_LocationNo { get; set; }

        /// <summary>
        /// 货位名称
        /// </summary>
        public string F_LocationName { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public string F_EnabledMark { get; set; }
        /// <summary>
        /// 巷道编号
        /// </summary>
        public string F_NumberOfRoadway { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string F_EquipmentSerialNumber { get; set; }


    }
}