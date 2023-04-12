using System;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvLimitGoods
{
    /// <summary>
    /// Agv请求取放更新输入参数
    /// </summary>
    public class ZjnBaseStdLimitgoodsUpInput : ZjnBaseStdLimitgoodsCrInput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string id { get; set; }
        
    }
}
