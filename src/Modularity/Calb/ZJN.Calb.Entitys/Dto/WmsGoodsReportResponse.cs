using HSZ.JsonSerialization;
using System.Collections.Generic;

namespace ZJN.Calb.Entitys.Dto
{
    /// <summary>
    /// 立库库存报表 响应
    /// </summary>
    public class WmsGoodsReportResponse:AttributeExt
    {
        public string containerCode { get; set; } = "";

        public string productCode { get; set; } = "";

        public string qty { get; set; } = "";

        public string locatorCode { get; set; } = "";
    }




}
