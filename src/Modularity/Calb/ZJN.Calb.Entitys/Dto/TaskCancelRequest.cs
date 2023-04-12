namespace ZJN.Calb.Entitys.Dto
{
    /// <summary>
    /// 出入库指令取消接口 请求
    /// </summary>
    public class TaskCancelRequest
    {
        public string instuctionNum { get; set; }

        public string orderNo { get; set; }

        public string status { get; set; }
    }
}
