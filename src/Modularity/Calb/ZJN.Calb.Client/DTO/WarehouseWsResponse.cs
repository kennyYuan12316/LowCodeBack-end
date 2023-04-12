namespace ZJN.Calb.Client.DTO
{
    public class WarehouseWsResponse : AttributeExt
    {
        public int id { get; set; }
        public string requestId { get; set; }
        public object operationType { get; set; }
        public string channelId { get; set; }
        public string clientCode { get; set; }
        public string requestTime { get; set; }
        public string uomCode { get; set; }
        public string weight { get; set; }
        public string containerCode { get; set; }


    }
}