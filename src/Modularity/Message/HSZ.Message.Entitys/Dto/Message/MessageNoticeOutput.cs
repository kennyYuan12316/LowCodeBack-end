using HSZ.Dependency;
using Newtonsoft.Json;
using System;

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
    public class MessageNoticeOutput
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
        /// 发布人员
        /// </summary>
        public string creatorUser { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? lastModifyTime { get; set; }
        /// <summary>
        ///创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
        /// <summary>
        /// 状态(0-存草稿，1-已发布)
        /// </summary>
        public int? enabledMark { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int? type { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        [JsonIgnore]
        public string deleteMark {  get; set; }    
    }
}