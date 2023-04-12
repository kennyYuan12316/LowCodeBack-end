using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.SmsTemplate
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：base_sms_template修改输入参数
    /// </summary>
    [SuppressSniffer]
    public class SmsTemplateCrInput
    {
        /// <summary>
        /// 短信提供商
        /// </summary>
        public int? company { get; set; }
        
        /// <summary>
        /// 应用编号
        /// </summary>
        public string appId { get; set; }

        /// <summary>
        /// 签名内容
        /// </summary>
        public string signContent { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 模板编号
        /// </summary>
        public string templateId { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string templateName { get; set; }

    }
}
