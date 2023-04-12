using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnTaskInterfaceApplyLog
{
    /// <summary>
    /// 接口调用履历表输出参数
    /// </summary>
    public class ZjnTaskInterfaceApplyLogInfoOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 调用完整地址
        /// </summary>
        public string address { get; set; }
        
        /// <summary>
        /// 接口名
        /// </summary>
        public string interfaceName { get; set; }
        
        /// <summary>
        /// 入参
        /// </summary>
        public string enterParameter { get; set; }
        
        /// <summary>
        /// 出参
        /// </summary>
        public string outParameter { get; set; }
        
        /// <summary>
        /// 调用时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 消息
        /// </summary>
        public string msg { get; set; }
        
    }
}