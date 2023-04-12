using System.Collections.Generic;

namespace ZJN.Calb.Entitys.Dto
{
  
    public class GoodsStatusUpRequest
    {
        public int ifaceId { get; set; }
        public List<GoodsStatusUpLine> lineList { get; set; }
        public string sstatus { get; set; }
        public string tstatus { get; set; }

        public string container { get; set; }
    }

    public class GoodsStatusUpLine:AttributeExt
    {
        public string lot { get; set; }
        public string materialCode { get; set; }
        public string productionCode { get; set; }

        public string cellLevel { get; set; }

        public string productionLine { get; set; }

        public string qty { get; set; }
    }




  

}
