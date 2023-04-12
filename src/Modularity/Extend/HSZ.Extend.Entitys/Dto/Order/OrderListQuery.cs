using HSZ.Common.Filter;
using HSZ.Dependency;

namespace HSZ.Extend.Entitys.Dto.Order
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：获取订单列表入参(带分页)
    /// </summary>
    [SuppressSniffer]
    public class OrderListQuery : PageInputBase
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public long? startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public long? endTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }

    }
}
