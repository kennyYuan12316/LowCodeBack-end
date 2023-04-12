using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsOcvbatteries
{
    /// <summary>
    /// OCV输出参数
    /// </summary>
    public class ZjnWmsOcvbatteriesInfoOutput
    {
        /// <summary>
        /// 唯一id
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 单据指令
        /// </summary>
        public string instructionNum { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public List<ZjnWmsOcvbatteriesResponsesLineInfoOutput> zjnWmsOcvbatteriesResponsesLineList { get; set; }
        
    }
}