using HSZ.JsonSerialization;
using System.Collections.Generic;

namespace ZJN.Calb.Entitys.Dto
{
    /// <summary>
    /// 出入库指令取消接口 响应
    /// </summary>
    public class TaskCancelResponse
    {
        public string code { get; set; } = "0";

        public string message { get; set; } = "Success";
    }

}
