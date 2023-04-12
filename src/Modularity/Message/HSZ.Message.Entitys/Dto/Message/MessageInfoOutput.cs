using HSZ.Dependency;
using System;
using System.Text.Json.Serialization;

namespace HSZ.Message.Entitys.Dto.Message
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class MessageInfoOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 正文内容	
        /// </summary>
        public string bodyText { get; set; }
        /// <summary>
        /// 发送人员
        /// </summary>
        public string creatorUser { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime? lastModifyTime { get; set; }
        /// <summary>
        /// 收件用户        
        /// </summary>
        public string toUserIds { get; set; }

        /// <summary>
        /// 附件        
        /// </summary>
        public string files { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int? deleteMark { get; set; }

    }
}
