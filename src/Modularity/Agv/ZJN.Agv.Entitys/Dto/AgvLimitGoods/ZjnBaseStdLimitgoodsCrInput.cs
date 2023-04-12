using System;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvLimitGoods
{
    /// <summary>
    /// Agv请求取放修改输入参数
    /// </summary>
    public class ZjnBaseStdLimitgoodsCrInput
    {
        /// <summary>
        /// 数据唯一标识
        /// </summary>
        public string requestId { get; set; }
        
        /// <summary>
        /// 系统标识
        /// </summary>
        public string clientCode { get; set; }
        
        /// <summary>
        /// 任务类型
        /// </summary>
        public int? taskType { get; set; }
        
        /// <summary>
        /// 库位编号
        /// </summary>
        public string locationCode { get; set; }
        
        /// <summary>
        /// 容器编码
        /// </summary>
        public string containerCode { get; set; }
        
        /// <summary>
        /// 服务地址
        /// </summary>
        public string channelId { get; set; }
        
        /// <summary>
        /// 请求发送时间
        /// </summary>
        public DateTime? requestTime { get; set; }
        
    }
}