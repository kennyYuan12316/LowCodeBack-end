using HSZ.Common.Filter;
using HSZ.Message.Entitys;
using HSZ.Message.Entitys.Model.IM;
using System.Collections.Generic;

namespace HSZ.Message.Interfaces.Message
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：聊天内容
    /// </summary>
    public interface IIMContentService
    {
        /// <summary>
        /// 获取消息列表
        /// </summary>
        /// <param name="sendUserId"></param>
        /// <param name="receiveUserId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        dynamic GetMessageList(string sendUserId, string receiveUserId, PageInputBase input);

        /// <summary>
        /// 获取未读消息
        /// </summary>
        /// <param name="receiveUserId"></param>
        /// <returns></returns>
        List<IMUnreadNumModel> GetUnreadList(string receiveUserId);

        /// <summary>
        /// 获取未读消息
        /// </summary>
        /// <param name="receiveUserId">接收者</param>
        /// <returns></returns>
        int GetUnreadCount(string receiveUserId);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sendUserId">发送者</param>
        /// <param name="receiveUserId">接收者</param>
        /// <param name="message">消息内容</param>
        /// <param name="messageType">消息类型</param>
        /// <returns></returns>
        int SendMessage(string sendUserId, string receiveUserId, string message, string messageType);

        /// <summary>
        /// 已读消息
        /// </summary>
        /// <param name="sendUserId">发送者</param>
        /// <param name="receiveUserId">接收者</param>
        int ReadMessage(string sendUserId, string receiveUserId);
    }
}
