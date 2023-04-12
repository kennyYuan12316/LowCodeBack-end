using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsOcvbatteries
{
    /// <summary>
    /// OCV列表查询输入
    /// </summary>
    public class ZjnWmsOcvbatteriesListQueryInput : PageInputBase
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
        /// 单据指令
        /// </summary>
        public string F_InstructionNum { get; set; }
        
    }
}