using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvPdaTask
{
    /// <summary>
    /// Agv上传PDA任务输出参数
    /// </summary>
    public class ZjnBaseStdPdataskInfoOutput
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
        /// 任务类型
   
        /// </summary>
        public int? taskType { get; set; }
        
        /// <summary>
        /// 起点库位编码
   
        /// </summary>
        public string startAreaCode { get; set; }
        
        /// <summary>
        /// 起点库区编码
   
        /// </summary>
        public string startLocCode { get; set; }
        
        /// <summary>
        /// 终点库区编码
   
        /// </summary>
        public string endAreaCode { get; set; }
        
        /// <summary>
        /// 终点库位编码
   
        /// </summary>
        public string endLocCode { get; set; }
        
        /// <summary>
        /// 物料类型编号
   
        /// </summary>
        public string goodsCode { get; set; }
        
        /// <summary>
        /// 容器编码
   
        /// </summary>
        public string containerCode { get; set; }
        
        /// <summary>
        /// 数量
   
        /// </summary>
        public int? quantity { get; set; }
        
    }
}