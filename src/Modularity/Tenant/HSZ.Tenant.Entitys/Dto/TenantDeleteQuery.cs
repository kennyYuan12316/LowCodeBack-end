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
    public class TenantDeleteQuery
    {
        /// <summary>
        /// (0-不删除，1-删除)是否删除租户数据库
        /// </summary>
        public int? isClear { get; set; }
    }
}
