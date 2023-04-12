using HSZ.Dependency;

namespace HSZ.Common.Const
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：Claim常量
    /// </summary>
    [SuppressSniffer]
    public class ClaimConst
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public const string CLAINM_USERID = "UserId";

        /// <summary>
        /// 用户姓名
        /// </summary>
        public const string CLAINM_REALNAME = "UserName";

        /// <summary>
        /// 账号
        /// </summary>
        public const string CLAINM_ACCOUNT = "Account";

        /// <summary>
        /// 是否超级管理
        /// </summary>
        public const string CLAINM_ADMINISTRATOR = "Administrator";

        /// <summary>
        /// 租户ID
        /// </summary>
        public const string TENANT_ID = "TenantId";

        /// <summary>
        /// 租户DbName
        /// </summary>
        public const string TENANT_DB_NAME = "TenantDbName";

        /// <summary>
        /// 单一登录方式（1：后登录踢出先登录 2：同时登录）
        /// </summary>
        public const string SINGLELOGIN = "SingleLogin";
    }
}
