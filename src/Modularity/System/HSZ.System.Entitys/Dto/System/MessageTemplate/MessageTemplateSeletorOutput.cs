using HSZ.Dependency;
using System;

namespace HSZ.System.Entitys.Dto.MessageTemplate
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：base_message_template输入参数
    /// </summary>
    [SuppressSniffer]
    public class MessageTemplateSeletorOutput
    {
        /// <summary>
        /// 自然主键
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 分类（数据字典）
        /// </summary>
        public string category { get; set; }
        
        /// <summary>
        /// 模板名称
        /// </summary>
        public string fullName { get; set; }
        
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string templateJson { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? lastModifyTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string creatorUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

    }
}
