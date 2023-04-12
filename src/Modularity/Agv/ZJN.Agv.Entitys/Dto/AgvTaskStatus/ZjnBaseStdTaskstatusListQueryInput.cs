using HSZ.Common.Filter;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvTaskStatus
{
    /// <summary>
    /// Agv上传任务状态列表查询输入
    /// </summary>
    public class ZjnBaseStdTaskstatusListQueryInput : PageInputBase
    {
        /// <summary>
        /// 选择导出数据key
        /// </summary>
        public string selectKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int dataType { get; set; }

        /// <summary>
        /// 数据唯一标识
        /// </summary>
        public string F_RequestId { get; set; }
        
        /// <summary>
        /// 系统标识
        /// </summary>
        public string F_ClientCode { get; set; }
        
        /// <summary>
        /// 请求发送时间
        /// </summary>
        public string F_RequestTime { get; set; }
        
        /// <summary>
        /// 任务编码
        /// </summary>
        public string F_InstanceId { get; set; }
        
        /// <summary>
        /// 任务状态
        /// </summary>
        public string F_TaskStatus { get; set; }
        
        /// <summary>
        /// AGV小车
        /// </summary>
        public string F_AgvNum { get; set; }
        
    }
}