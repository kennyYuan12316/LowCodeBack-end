using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.Province
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class ProvinceCrInput
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 区域编号
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int enabledMark { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 区域上级
        /// </summary>
        public string parentId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
