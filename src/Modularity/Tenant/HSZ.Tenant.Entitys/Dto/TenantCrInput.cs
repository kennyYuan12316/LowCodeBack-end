using HSZ.Dependency;

namespace HSZ.Tenant.Entitys.Dtos
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class TenantCrInput
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string companyName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public long? expiresTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
    }
}
