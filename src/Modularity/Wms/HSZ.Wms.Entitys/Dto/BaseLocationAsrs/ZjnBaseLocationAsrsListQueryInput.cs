using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseLocationAsrs
{
    /// <summary>
    /// 立库货位信息列表查询输入
    /// </summary>
    public class ZjnBaseLocationAsrsListQueryInput : PageInputBase
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
        public string F_Row { get; set; }
        
        /// <summary>
        /// 列
        /// </summary>
        public string F_Cell { get; set; }
        
        /// <summary>
        /// 层
        /// </summary>
        public string F_Layer { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string F_TrayNo { get; set; }
        
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string F_ByWarehouse { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string F_Legion { get; set; }


    }
}