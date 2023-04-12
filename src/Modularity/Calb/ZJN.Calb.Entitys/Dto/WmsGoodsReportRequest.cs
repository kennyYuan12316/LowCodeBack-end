namespace ZJN.Calb.Entitys.Dto
{
    /// <summary>
    /// 立库库存报表 请求
    /// </summary>
    public class WmsGoodsReportRequest:AttributeExt
    {
        public string materialCode { get; set; }

        public string materialName { get; set; }

        public string batch { get; set; }

        public string status { get; set; }

        public string locatorCode { get; set; }
    }
}
