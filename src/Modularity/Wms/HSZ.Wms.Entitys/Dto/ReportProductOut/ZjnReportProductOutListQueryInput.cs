using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnReportProductOut
{
    /// <summary>
    /// 成品出库单列表查询输入
    /// </summary>
    public class ZjnReportProductOutListQueryInput : PageInputBase
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
        /// 物料编码
        /// </summary>
        public string F_ProductsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string F_ProductsName { get; set; }
        
        /// <summary>
        /// 出库单
        /// </summary>
        public string F_OutOrder { get; set; }
        
    }
}