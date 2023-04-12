using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWcsPlc
{
    /// <summary>
    /// PLC连接表更新输入参数
    /// </summary>
    public class ZjnWcsPlcUpInput : ZjnWcsPlcCrInput
    {
        /// <summary>
        /// Key
        /// </summary>
        public string plcId { get; set; }
        
    }
}
