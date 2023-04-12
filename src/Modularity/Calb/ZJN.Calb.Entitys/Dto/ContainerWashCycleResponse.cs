using HSZ.JsonSerialization;

namespace ZJN.Calb.Entitys.Dto
{
    /// <summary>
    /// 容器清洗周期同步 响应
    /// </summary>
    public class ContainerWashCycleResponse : AttributeExt
    {
        public string code { get; set; } = "0";

        public string message { get; set; } = "Success";
    }


}
