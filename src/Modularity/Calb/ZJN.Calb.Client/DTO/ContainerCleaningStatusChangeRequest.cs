namespace ZJN.Calb.Client.DTO
{
    public class ContainerCleaningStatusChangeRequest : AttributeExt
    {
        public string requestId { get; set; }
        public string clientCode { get; set; }
        public string channelId { get; set; }
        public string requestTime { get; set; }
        public string container { get; set; }
        public string cleaningTime { get; set; }
    }

}