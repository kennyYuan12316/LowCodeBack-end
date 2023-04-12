using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsTray
{
    /// <summary>
    /// 托盘信息输出参数
    /// </summary>
    public class ZjnWmsTrayInfoOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string trayNo { get; set; }
        
        /// <summary>
        /// 托盘名称
        /// </summary>
        public string trayName { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 托盘状态
        /// </summary>
        public string trayStates { get; set; }

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

        /// <summary>
        /// 托盘属性
        /// </summary>
        public string trayAttr { get; set; }

    }
}