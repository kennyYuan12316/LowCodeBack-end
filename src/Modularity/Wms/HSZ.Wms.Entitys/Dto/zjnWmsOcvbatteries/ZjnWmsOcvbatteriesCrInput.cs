using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsOcvbatteries
{
    /// <summary>
    /// OCV修改输入参数
    /// </summary>
    public class ZjnWmsOcvbatteriesCrInput
    {
        /// <summary>
        /// 单据指令
        /// </summary>
        public string instructionNum { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public List<ZjnWmsOcvbatteriesResponsesLineCrInput> zjnWmsOcvbatteriesResponsesLineList { get; set; }
        
    }
}