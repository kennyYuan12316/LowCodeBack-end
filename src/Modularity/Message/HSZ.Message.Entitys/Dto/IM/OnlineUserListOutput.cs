using HSZ.Dependency;

namespace HSZ.Message.Entitys.Dto.IM
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：在线用户
    /// </summary>
    [SuppressSniffer]
    public class OnlineUserListOutput
{
        /// <summary>
        /// 用户ID
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string userAccount { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public string loginTime { get; set; }

        /// <summary>
        /// 登录IP地址
        /// </summary>
        public string loginIPAddress { get; set; }

        /// <summary>
        /// 登录平台设备
        /// </summary>
        public string loginPlatForm { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public string tenantId { get; set; }
    }
}
