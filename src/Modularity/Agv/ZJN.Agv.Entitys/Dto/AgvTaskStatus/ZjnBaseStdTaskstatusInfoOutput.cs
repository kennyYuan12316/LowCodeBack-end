using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvTaskStatus
{
    /// <summary>
    /// Agv上传任务状态输出参数
    /// </summary>
    public class ZjnBaseStdTaskstatusInfoOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 数据唯一标识
        /// </summary>
        public string requestId { get; set; }
        
        /// <summary>
        /// 系统标识
        /// </summary>
        public string clientCode { get; set; }
        
        /// <summary>
        /// 服务地址
        /// </summary>
        public string channelId { get; set; }
        
        /// <summary>
        /// 请求发送时间
        /// </summary>
        public DateTime? requestTime { get; set; }
        
        /// <summary>
        /// 任务编码
        /// </summary>
        public string instanceId { get; set; }
        
        /// <summary>
        /// 任务序号
        /// </summary>
        public int? taskIndex { get; set; }
        
        /// <summary>
        /// 任务状态
        /// </summary>
        public int? taskStatus { get; set; }
        
        /// <summary>
        /// AGV小车
        /// </summary>
        public string agvNum { get; set; }
        
    }
}