using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ZJN.Calb.Entitys.Dto
{

    /// <summary>
    /// 取料放料接口-AGV  请求
    /// </summary>
    public class AgvGoodsRequest : AttributeExt
    {
        [Required()]
        [DisplayName("唯一标识")]
        public string requestId { get; set; }


        [Required()]
        [DisplayName("目标服务器")]
        public string toChannelId { get; set; }


        [DisplayName("客户端编码")]
        public string clientCode { get; set; }

        [DisplayName("客户端编码")]
        public string requestTime { get; set; }

        [Required()]
        [DisplayName("客户端编码")]
        public string msgType { get; set; }

        [Required()]
        [DisplayName("取放料任务编码")]
        public string taskCode { get; set; }


        [Required()]
        [DisplayName("指令类型")]
        public string instructionType { get; set; }


        [Required()]
        [DisplayName("容器目标点")]
        public string locationCode { get; set; }


        [Required()]
        [DisplayName("容器编号")]
        public string containerCode { get; set; }


        [DisplayName("容器类型")]
        public string containerCategory { get; set; }
    }
}
