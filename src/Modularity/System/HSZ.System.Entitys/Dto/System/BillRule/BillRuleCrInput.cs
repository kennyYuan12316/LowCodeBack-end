using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.BillRule
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class BillRuleCrInput
    {
        /// <summary>
        /// 业务名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 业务编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 流水前缀
        /// </summary>
        public string prefix { get; set; }

        /// <summary>
        /// 流水日期
        /// </summary>
        public string dateFormat { get; set; }

        /// <summary>
        /// 流水位数
        /// </summary>
        public double digit { get; set; }

        /// <summary>
        /// 
        /// 流水起始
        /// </summary>
        public string startNumber { get; set; }

        /// <summary>
        /// 流水范例
        /// </summary>
        public string example { get; set; }

        /// <summary>
        /// 流水状态
        /// </summary>
        public int enabledMark { get; set; }

        /// <summary>
        /// 流水说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
