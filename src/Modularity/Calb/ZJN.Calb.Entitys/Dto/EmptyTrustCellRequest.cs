namespace ZJN.Calb.Entitys.Dto
{
    /// <summary>
    /// 空托呼叫 请求
    /// </summary>
    public class EmptyTrustCellRequest: AttributeExt
    {
        public string materialCode { get; set; }

        public string locationCode { get; set; }
    }
}
