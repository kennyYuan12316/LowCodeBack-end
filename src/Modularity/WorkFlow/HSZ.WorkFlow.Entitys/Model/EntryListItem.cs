using HSZ.Dependency;

namespace HSZ.WorkFlow.Entitys.Model
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class EntryListItem
    {
        /// <summary>
        /// 
        /// </summary>
        public decimal? amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string goodsName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string invoiceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string qty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? sortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string specifications { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string unit { get; set; }
        public string materialDemand { get; set; }
        public string proportioning { get; set; }
    }
}
