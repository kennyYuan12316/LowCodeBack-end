using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsBatteriesType
{
    /// <summary>
    /// 电芯种类静置配置列表查询输入
    /// </summary>
    public class ZjnWmsBatteriesTypeListQueryInput : PageInputBase
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
        /// 电池种类
        /// </summary>
        public string F_BatteriesType { get; set; }
        
        /// <summary>
        /// 静置时间（小时）
        /// </summary>
        public string F_StandingTime { get; set; }
        
    }
}