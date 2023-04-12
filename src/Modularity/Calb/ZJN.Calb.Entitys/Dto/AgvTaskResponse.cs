using HSZ.JsonSerialization;

namespace ZJN.Calb.Entitys.Dto
{
    /// <summary>
    /// 配送任务指令-AGV 响应
    /// </summary>
    public class AgvTaskResponse: ResultMessageHead
    {
        public string responseId { get; set; }

        public string clientCode { get; set; }


        public string requestTime { get; set; }

        public string msgType { get; set; }

        public string taskCode { get; set; }

        public string payload { get;  set; } = string.Empty;

        public void SetPayload(AgvTaskPayload oPayload)
          => payload = oPayload.Serialize();
    }





    public class AgvTaskPayload : AttributeExt
    {
        public string workflowCode { get; set; } = string.Empty;

        public int instanceId { get; set; }=0;


        public int instancePriority { get; set; }=0;


        public string containerCode { get; set; } = string.Empty;


        public string taskId { get; set; } = string.Empty;


        public string locationFrom { get; set; } = string.Empty;

        public string locationTo { get; set; } = string.Empty;
    }
}
