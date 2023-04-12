using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ZJN.Calb.Entitys.Dto
{
    /// <summary>
    /// 配送任务指令-AGV 请求
    /// </summary>
    public class AgvTaskRequest:AttributeExt
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Required]
        [DisplayName("唯一标识")]
        public string requestId { get; set; }

        [Required]
        [DisplayName("来源服务器标识")]
        public string fromChannelId { get; set; }

        [Required]
        [DisplayName("目标服务器标识")]
        public string toChannelId { get; set; }


        [DisplayName("客户端类型")]
        public string clientCode { get; set; }

        [Required]
        [DisplayName("请求发送时间")]
        public string requestTime { get; set; }

        [Required]
        [DisplayName("任务号，AGV指令号")]
        public string taskCode { get; set; }

        [DisplayName("流程编号")]
        public string instanceId { get; set; }

        [Required]
        [DisplayName("任务类型")]
        public string taskType { get; set; }

        [DisplayName("AGV设备类型")]
        public string robotType { get; set; }

        [Required]
        [DisplayName("任务优先级")]
        public string priority { get; set; }


        [Required]
        [DisplayName("任务状态")]
        public string status { get; set; }


        [DisplayName("起始区域")]
        public string locationFrom { get; set; }


        [DisplayName("容器编号")]
        public string containerCode { get; set; }


        [DisplayName("容器类型")]
        public string containerCategory { get; set; }


        [Required]
        [DisplayName("目标区域")]
        public string locationTo { get; set; }

        [DisplayName("目标区域")]
        public string needTime { get; set; }


        [DisplayName("流转策略")]
        public string flowStrategy { get; set; }
    }
   



}
