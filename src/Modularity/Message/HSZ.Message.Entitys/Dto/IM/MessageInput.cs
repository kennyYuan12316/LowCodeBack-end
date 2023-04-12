using HSZ.Dependency;

namespace HSZ.Message.Entitys.Dto.IM
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：消息接收类
    /// </summary>
    [SuppressSniffer]
    public class MessageInput
    {
        /// <summary>
        /// 发送发送客户端ID
        /// </summary>
        public string sendClientId { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        public string method { get; set; }

        /// <summary>
        /// 移动设备
        /// </summary>
        public bool mobileDevice { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// 发送者ID
        /// </summary>
        public string toUserId { get; set; }

        /// <summary>
        /// 接收者ID
        /// </summary>
        public string formUserId { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string messageType { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public object messageContent { get; set; }

        /// <summary>
        /// 当前页数
        /// </summary>
        public int currentPage { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int pageSize { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public string sord { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }
    }
}
