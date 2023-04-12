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
    public class SystemInfoModel
    {
        /// <summary>
        /// 系统
        /// </summary>
        public string os { get; set; }
        /// <summary>
        /// 运行时间
        /// </summary>
        public string day { get; set; }
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ip { get; set; }
    }
}
