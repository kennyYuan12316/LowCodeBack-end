using System;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvCancelOrder
{
    /// <summary>
    /// 立库取消订单修改输入参数
    /// </summary>
    public class ZjnBaseStdCancelorderCrInput
    {
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 外部订单ID
        /// </summary>
        public string outerOrderId { get; set; }
        
    }
}