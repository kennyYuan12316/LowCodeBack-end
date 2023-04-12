using HSZ.Common.Filter;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvCancelOrder
{
    /// <summary>
    /// 立库取消订单列表查询输入
    /// </summary>
    public class ZjnBaseStdCancelorderListQueryInput : PageInputBase
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
        /// 更新时间
        /// </summary>
        public string F_CreateTime { get; set; }
        
        /// <summary>
        /// 外部订单ID
        /// </summary>
        public string F_OuterOrderId { get; set; }
        
    }
}