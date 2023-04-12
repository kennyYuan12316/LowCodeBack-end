using System.Collections.Generic;

namespace ZJN.Calb.Client.DTO
{


    public class ContainerIntoApplyResponse : AttributeExt
    {
        public string code { get; set; }
        public string message { get; set; }
        public string instructionNum { get; set; }
        public string containerCode { get; set; }
        public string productionCode { get; set; }
        public string lot { get; set; }
        public string materialCode { get; set; }
        public List<ContainerIntoApplyRequestLine> responsesLine { get; set; }
    }

}