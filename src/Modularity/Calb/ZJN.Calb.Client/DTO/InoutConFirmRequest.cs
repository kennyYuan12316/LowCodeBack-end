using System.Collections.Generic;

namespace ZJN.Calb.Client.DTO
{




    public class InoutConFirmRequest:AttributeExt
    {
        public int id { get; set; }
        public string requestId { get; set; }
        public string operationType { get; set; }
        public string toChannelId { get; set; }
        public string clientCode { get; set; }
        public string requestTime { get; set; }
        public string instructionNum { get; set; }
        public string type { get; set; }
      
        public List<InoutConFirmLine> lines { get; set; }
    }

    public class InoutConFirmLine : AttributeExt
    {
        public int id { get; set; }
        public int confirmid { get; set; }
        public string container { get; set; }
        public string productionCode { get; set; }
        public string materialBarcode { get; set; }
        public string lot { get; set; }
        public string fromLocator { get; set; }
        public string toLocator { get; set; }
   
    }


}