using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.zjnWcsProcessConfig
{
    /// <summary>
    /// 业务路径配置表更新输入参数
    /// </summary>
    public class ZjnWcsProcessConfigUpInput : ZjnWcsProcessConfigCrInput
    {
        /// <summary>
        /// 任务路径配置主键
        /// </summary>
        public string id { get; set; }
        
    }
}
