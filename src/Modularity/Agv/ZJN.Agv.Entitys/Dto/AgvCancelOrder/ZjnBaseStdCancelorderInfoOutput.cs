using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvCancelOrder
{
    /// <summary>
    /// 立库取消订单输出参数
    /// </summary>
    public class ZjnBaseStdCancelorderInfoOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string id { get; set; }
        
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