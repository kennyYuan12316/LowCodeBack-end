using HSZ.JsonSerialization;

namespace ZJN.Calb.Entitys.Dto
{
    /// <summary>
    /// 取料放料接口（立库） 响应
    /// </summary>
    public class AgvGrabGoodsResponse : AttributeExt
    {
        public string code { get; set; } = "0";

        public string message { get; set; } = "Success";

        public bool success { get; set; } = true;

      
    }





}
