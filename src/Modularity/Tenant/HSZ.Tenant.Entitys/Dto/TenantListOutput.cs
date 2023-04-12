using HSZ.Dependency;
using System;

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
    public class TenantListOutput
    {
        public string id { get; set; }
        public string enCode { get; set; }
        public string fullName { get; set; }
        public string companyName { get; set; }
        public DateTime? creatorTime { get; set; }
        public DateTime? expiresTime { get; set; }
        public string description { get; set; }
    }
}
