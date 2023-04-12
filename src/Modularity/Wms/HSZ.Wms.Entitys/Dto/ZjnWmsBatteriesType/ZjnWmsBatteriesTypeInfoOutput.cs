using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsBatteriesType
{
    /// <summary>
    /// 电芯种类静置配置输出参数
    /// </summary>
    public class ZjnWmsBatteriesTypeInfoOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 电池种类
        /// </summary>
        public int? batteriesType { get; set; }
        
        /// <summary>
        /// 静置时间（小时）
        /// </summary>
        public int? standingTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? enabledMark { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
    }
}