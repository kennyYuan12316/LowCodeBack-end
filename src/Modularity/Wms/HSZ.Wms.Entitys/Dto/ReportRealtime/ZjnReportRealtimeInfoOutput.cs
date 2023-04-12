using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnReportRealtime
{
    /// <summary>
    /// 实时库存输出参数
    /// </summary>
    public class ZjnReportRealtimeInfoOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 序号
        /// </summary>
        public int? num { get; set; }
        
        /// <summary>
        /// 工厂
        /// </summary>
        public string factory { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        public string productsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string productsName { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        public string batch { get; set; }
        
        /// <summary>
        /// 质量状态
        /// </summary>
        public string quality { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string productsUnit { get; set; }
        
        /// <summary>
        /// 总数库内
        /// </summary>
        public int? sumInside { get; set; }
        
        /// <summary>
        /// 托盘数库内
        /// </summary>
        public int? trayCountInside { get; set; }
        
        /// <summary>
        /// 总数库外
        /// </summary>
        public int? sumOutside { get; set; }
        
        /// <summary>
        /// 托盘数库外
        /// </summary>
        public int? trayCountOutside { get; set; }
        
        /// <summary>
        /// 总数移动中
        /// </summary>
        public int? sumMoving { get; set; }
        
        /// <summary>
        /// 托盘数移动中
        /// </summary>
        public int? trayCountMoving { get; set; }
        
        /// <summary>
        /// 总数全部
        /// </summary>
        public int? sumTotal { get; set; }
        
        /// <summary>
        /// 托盘数全部
        /// </summary>
        public int? trayCountTotal { get; set; }
        
    }
}