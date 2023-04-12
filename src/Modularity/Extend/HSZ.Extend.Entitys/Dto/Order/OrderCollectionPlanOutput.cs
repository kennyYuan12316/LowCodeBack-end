using HSZ.Dependency;
using System;

namespace HSZ.Extend.Entitys.Dto.Order
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：获取订单列表-收款计划
    /// </summary>
    [SuppressSniffer]
    public class OrderCollectionPlanOutput
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
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
        public string receivableMoney { get; set; }
        /// <summary>
        /// 收款方式
        /// </summary>
        public string receivableMode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 收款摘要
        /// </summary>
        public string fabstract { get; set; }
    }
}
