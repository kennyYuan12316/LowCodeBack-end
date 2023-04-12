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
    public class ProvinceSelectorOutput
    {
        /// <summary>
        /// 是否为子节点
        /// </summary>
        public bool isLeaf { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }

    }
}
