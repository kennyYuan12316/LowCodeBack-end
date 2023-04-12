using HSZ.Dependency;

namespace HSZ.Common.Model.Machine
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class DiskInfoModel
    {
        /// <summary>
        /// 硬盘总容量
        /// </summary>
        public string total { get; set; }
        /// <summary>
        /// 空闲硬盘
        /// </summary>
        public string available { get; set; }
        /// <summary>
        /// 已使用硬盘
        /// </summary>
        public string used { get; set; }
        /// <summary>
        /// 已使用百分比
        /// </summary>
        public string usageRate { get; set; }
    }
}
