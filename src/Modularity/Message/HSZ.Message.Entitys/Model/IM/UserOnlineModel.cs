using System;

namespace HSZ.Message.Entitys.Model.IM
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：在线用户模型
    /// </summary>
    public class UserOnlineModel
    {
        /// <summary>
        /// 连接ID
        /// </summary>
        public string connectionId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 最后连接时间
        /// </summary>
        public DateTime lastTime { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string lastLoginIp { get; set; }

        /// <summary>
        /// 登录平台设备
        /// </summary>
        public string lastLoginPlatForm { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 租户id
        /// </summary>
        public string tenantId { get; set; }

        /// <summary>
        /// token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// 是否移动端
        /// </summary>
        public bool isMobileDevice { get; set; }
    }
}
