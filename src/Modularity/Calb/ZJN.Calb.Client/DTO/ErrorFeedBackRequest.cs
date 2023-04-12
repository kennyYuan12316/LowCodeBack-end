namespace ZJN.Calb.Client.DTO
{
    public class ErrorFeedBackRequest : AttributeExt
    {
        public string requestId { get; set; }
        public string toChannelId { get; set; }
        public string clientCode { get; set; }
        public string requestTime { get; set; }
        public string instuctionNum { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public object xNumber { get; set; }
        public object materialCode { get; set; }
        public object toLocator { get; set; }
    }





}