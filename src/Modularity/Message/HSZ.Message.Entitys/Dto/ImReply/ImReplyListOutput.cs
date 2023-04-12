using Newtonsoft.Json;
using System;

namespace HSZ.Message.Entitys.Dto.ImReply
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：聊天会话列表输出
    /// </summary>
    public class ImReplyListOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id;

        /// <summary>
        /// 发送者
        /// </summary>
        [JsonIgnore]
        public string sendUserId { get; set; }

        /// <summary>
        /// 接受者
        /// </summary>
        [JsonIgnore]
        public string userId;

        /// <summary>
        /// 名称
        /// </summary>
        public string realName;

        /// <summary>
        /// 头像
        /// </summary>
        public string headIcon;

        /// <summary>
        /// 最新消息
        /// </summary>
        public string latestMessage;

        /// <summary>
        /// 最新时间
        /// </summary>
        public DateTime latestDate;

        /// <summary>
        /// 未读消息
        /// </summary>
        public int unreadMessage;

        /// <summary>
        /// 消息类型
        /// </summary>
        public string messageType;

        /// <summary>
        /// 账号
        /// </summary>
        public string account;
    }
}
