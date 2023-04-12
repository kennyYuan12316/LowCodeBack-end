using HSZ.JsonSerialization;
using System.Collections.Generic;

namespace ZJN.Calb.Entitys.Dto
{
    /// <summary>
    /// 出入库指令接口 响应
    /// </summary>
    public class TaskWarehouseResponse : AttributeExt
    {
        public string code { get; set; } = "0";

        public string message { get; set; } = "Success";
        public bool success { get; set; } = true;


        public string result { get; set; } = "接收出入库指令成功！";

        public long timestamp { get; set; } = 1662703222544;
    }
}
