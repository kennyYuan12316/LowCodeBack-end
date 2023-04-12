using System;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvCreateOrder
{
    /// <summary>
    /// 立库下单更新输入参数
    /// </summary>
    public class ZjnBaseStdCreateorderUpInput : ZjnBaseStdCreateorderCrInput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string id { get; set; }
        
    }
}
