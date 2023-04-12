using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace ZJN.Calb.Entitys.Dto
{
    /// <summary>
    /// 容器清洗周期同步 请求
    /// </summary>
    public class ContainerWashCycleRequest:AttributeExt
    {
        [Required]
        [DisplayName("请求ID")]
        public string requestId { get; set; }
        
        [Required]
        public string channelId { get; set; }


        public string clientCode { get; set; }


        public string requestTime { get; set; }


        public string cleanCycle { get; set; }
        

        public string containerTypeCode { get; set; }


        public string flowStrategy { get; set; }
    }

}
