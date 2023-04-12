namespace ZJN.Calb.Client.DTO
{
    public class StatusFeedBackRequest : AttributeExt
    {
        public string requestId { get; set; }
        public string channelId { get; set; }
        public string clientCode { get; set; }
        public string requestTime { get; set; }
        public string taskNum { get; set; }
        public string taskStatus { get; set; }
        public string agvNum { get; set; }
        public string fromLocatorCode { get; set; }
        public string locatorCode { get; set; }
        public string location { get; set; }
        public string containerCode { get; set; }
    }





}