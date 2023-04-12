using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnHardSrm
{
    /// <summary>
    /// 堆垛机信息输出参数
    /// </summary>
    public class ZjnHardSrmInfoOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 设备编号
        /// </summary>
        public string hardNo { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        public string hardName { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        public string createUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public string enabledMark { get; set; }
        
    }
}