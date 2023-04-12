using System;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvGoodsType
{
    /// <summary>
    /// Agv请求物料类型更新输入参数
    /// </summary>
    public class ZjnBaseStdGoodstypeUpInput : ZjnBaseStdGoodstypeCrInput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string id { get; set; }
        
    }
}
