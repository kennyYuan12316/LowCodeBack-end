using System;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvCancelOrder
{
    /// <summary>
    /// 立库取消订单更新输入参数
    /// </summary>
    public class ZjnBaseStdCancelorderUpInput : ZjnBaseStdCancelorderCrInput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string id { get; set; }
        
    }
}
