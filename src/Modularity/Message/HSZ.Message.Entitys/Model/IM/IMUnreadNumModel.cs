using HSZ.Dependency;

namespace HSZ.Message.Entitys.Model.IM
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class IMUnreadNumModel
    {
        /// <summary>
        /// 发送者Id
        /// </summary>
        public string sendUserId { get; set; }
        /// <summary>
        /// 接收者Id
        /// </summary>
        public string receiveUserId { get; set; }
        /// <summary>
        /// 未读数量
        /// </summary>
        public int unreadNum { get; set; }
        /// <summary>
        /// 默认消息
        /// </summary>
        public string defaultMessage { get; set; }
        /// <summary>
        /// 默认消息类型
        /// </summary>
        public string defaultMessageType { get; set; }
        /// <summary>
        /// 默认消息时间
        /// </summary>
        public string defaultMessageTime { get; set; }
    }
}
