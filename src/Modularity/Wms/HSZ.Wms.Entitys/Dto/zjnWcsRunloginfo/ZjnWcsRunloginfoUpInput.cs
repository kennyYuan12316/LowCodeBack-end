using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWcsRunloginfo
{
    /// <summary>
    /// 运行日志更新输入参数
    /// </summary>
    public class ZjnWcsRunloginfoUpInput : ZjnWcsRunloginfoCrInput
    {
        /// <summary>
        /// (Event)事件日志表ID
        /// </summary>
        public string id { get; set; }
        
    }
}
