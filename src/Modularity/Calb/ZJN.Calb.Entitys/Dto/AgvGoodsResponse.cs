using HSZ.JsonSerialization;

namespace ZJN.Calb.Entitys.Dto
{
    /// <summary>
    /// 取料放料接口-AGV 响应
    /// </summary>
    public class AgvGoodsResponse
    {
        public string code { get; set; } = "0";

        public string message { get; set; } = "Success";
    }

}
