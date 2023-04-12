using HSZ.Dependency;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HSZ.Message.Entitys.Model.IM
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：WebSocket客户端信息
    /// </summary>
    [SuppressSniffer]
    public class WebSocketClient
    {
        /// <summary>
        /// 连接Id
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadIcon { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 登录IP
        /// </summary>
        public string LoginIpAddress { get; set; }

        /// <summary>
        /// 登录设备
        /// </summary>
        public string LoginPlatForm { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public string LoginTime { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// 移动端
        /// </summary>
        public bool IsMobileDevice { get; set; }

        /// <summary>
        /// 单一登录方式（1：后登录踢出先登录 2：同时登录）
        /// </summary>
        public string SingleLogin { get; set; }

        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// WebSocket对象
        /// </summary>
        public WebSocket WebSocket { get; set; }
    }
}
