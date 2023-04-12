using HSZ.Common.Filter;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvCreateOrder
{
    /// <summary>
    /// 立库下单列表查询输入
    /// </summary>
    public class ZjnBaseStdCreateorderListQueryInput : PageInputBase
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
        /// 业务关系编码
        /// </summary>
        public string F_BrCode { get; set; }
        
        /// <summary>
        /// 终点库区编码
        /// </summary>
        public string F_EndAreaCode { get; set; }
        
        /// <summary>
        /// 终点库位编码
        /// </summary>
        public string F_EndLocCode { get; set; }
        
        /// <summary>
        /// 起点库区编码
        /// </summary>
        public string F_StartAreaCode { get; set; }
        
        /// <summary>
        /// 起点库位编码
        /// </summary>
        public string F_StartLocCode { get; set; }
        
        /// <summary>
        /// 托盘编码
        /// </summary>
        public string F_TrayId { get; set; }
        
    }
}