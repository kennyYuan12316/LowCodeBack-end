using System;

namespace HSZ.wms.Entitys.Dto.ZjnBaseLocationAsrs
{
    /// <summary>
    /// 立库货位信息输入参数
    /// </summary>
    public class ZjnBaseLocationAsrsListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 货位编号
        /// </summary>
        public string F_LocationNo { get; set; }
        
        /// <summary>
        /// 设备编号（堆垛机）
        /// </summary>
        public string F_DeviceNo { get; set; }
        
        /// <summary>
        /// 行
        /// </summary>
        public int? F_Row { get; set; }
        
        /// <summary>
        /// 列
        /// </summary>
        public int? F_Cell { get; set; }
        
        /// <summary>
        /// 层
        /// </summary>
        public int? F_Layer { get; set; }
        
        /// <summary>
        /// 深
        /// </summary>
        public int? F_Depth { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string F_TrayNo { get; set; }
        
        /// <summary>
        /// 上次货位状态
        /// </summary>
        public int? F_LastStatus { get; set; }
        
        /// <summary>
        /// 货位状态：0空 1满 2未满 3故障 4火警 5静置中 6静置完成 7预约 8禁用
        /// </summary>
        public string F_LocationStatus { get; set; }
        
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
        /// 仓库编号
        /// </summary>
        public string F_ByWarehouse { get; set; }

        /// <summary>
        /// 货位类型
        /// </summary>
        public string F_type { get; set; }

        public string F_Legion { get; set; }

    }
}