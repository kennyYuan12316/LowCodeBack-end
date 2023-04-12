using Newtonsoft.Json;
using System;

namespace HSZ.Message.Entitys.Dto.ImReply
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：聊天会话对象ID
    /// </summary>
    public class ImReplyObjectIdOutput
    {
        /// <summary>
        /// 对象id
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 最新时间
        /// </summary>
        public DateTime? latestDate { get; set; }
    }
}
