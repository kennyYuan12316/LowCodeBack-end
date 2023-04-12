using System;

namespace ZJN.Agv.Entitys.Dto.AgvTaskStatus
{
    /// <summary>
    /// Agv上传任务状态输入参数
    /// </summary>
    public class ZjnBaseStdTaskstatusListOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 数据唯一标识
        /// </summary>
        public string F_RequestId { get; set; }
        
        /// <summary>
        /// 系统标识
        /// </summary>
        public string F_ClientCode { get; set; }
        
        /// <summary>
        /// 服务地址
        /// </summary>
        public string F_ChannelId { get; set; }
        
        /// <summary>
        /// 请求发送时间
        /// </summary>
        public DateTime? F_RequestTime { get; set; }
        
        /// <summary>
        /// 任务编码
        /// </summary>
        public string F_InstanceId { get; set; }
        
        /// <summary>
        /// 任务序号
        /// </summary>
        public int? F_TaskIndex { get; set; }
        
        /// <summary>
        /// 任务状态
        /// </summary>
        public int? F_TaskStatus { get; set; }
        
        /// <summary>
        /// AGV小车
        /// </summary>
        public string F_AgvNum { get; set; }
        
    }
}