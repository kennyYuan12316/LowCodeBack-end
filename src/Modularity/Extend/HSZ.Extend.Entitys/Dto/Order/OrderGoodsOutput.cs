using HSZ.Dependency;

namespace HSZ.Extend.Entitys.Dto.Order
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：获取商品列表
    /// </summary>
    [SuppressSniffer]
    public class OrderGoodsOutput
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        public string specifications { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// 售价
        /// </summary>
        public double price { get; set; }
    }
}
