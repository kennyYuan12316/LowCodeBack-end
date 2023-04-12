using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnReportProductSingle
{
    /// <summary>
    /// 成品库单体列表查询输入
    /// </summary>
    public class ZjnReportProductSingleListQueryInput : PageInputBase
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
        /// 所属托盘
        /// </summary>
        public string F_Tray { get; set; }
        
        /// <summary>
        /// 电芯条码
        /// </summary>
        public string F_BatteryCode { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        public string F_Batch { get; set; }
        
        /// <summary>
        /// 入库单
        /// </summary>
        public string F_EntryOrder { get; set; }
        
    }
}