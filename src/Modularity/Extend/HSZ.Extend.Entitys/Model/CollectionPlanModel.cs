using HSZ.Dependency;
using System;

namespace HSZ.Extend.Entitys.Model
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class CollectionPlanModel
    {
        /// <summary>
        /// 收款日期
        /// </summary>
        public DateTime? receivableDate { get; set; }
        /// <summary>
        /// 收款比率
        /// </summary>
        public decimal? receivableRate { get; set; }
        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal? receivableMoney { get; set; }
        /// <summary>
        /// 收款方式
        /// </summary>
        public string receivableMode { get; set; }
        /// <summary>
        /// 收款摘要
        /// </summary>
        public string fabstract { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 订单id
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        /// 接收状态
        /// </summary>
        public int? receivableState { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
