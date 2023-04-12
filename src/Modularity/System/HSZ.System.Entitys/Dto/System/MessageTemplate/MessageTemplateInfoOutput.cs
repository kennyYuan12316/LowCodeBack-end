using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.MessageTemplate
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：base_message_template输出参数
    /// </summary>
    [SuppressSniffer]
    public class MessageTemplateInfoOutput:MessageTemplateCrInput
    {
        /// <summary>
        /// 自然主键
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 短信名称
        /// </summary>
        public string smsTemplateName { get; set; }
        
    }
}
