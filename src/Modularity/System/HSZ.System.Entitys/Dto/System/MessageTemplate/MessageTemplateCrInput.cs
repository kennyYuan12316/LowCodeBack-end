using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.MessageTemplate
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：base_message_template修改输入参数
    /// </summary>
    [SuppressSniffer]
    public class MessageTemplateCrInput
    {
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
        /// 有效标志
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 是否站内信
        /// </summary>
        public int? isStationLetter { get; set; }

        /// <summary>
        /// 是否邮箱
        /// </summary>
        public int? isEmail { get; set; }

        /// <summary>
        /// 是否企业微信
        /// </summary>
        public int? isWecom { get; set; }

        /// <summary>
        /// 是否钉钉
        /// </summary>
        public int? isDingTalk { get; set; }

        /// <summary>
        /// 是否短信
        /// </summary>
        public int? isSms { get; set; }

        /// <summary>
        /// 短信模板ID
        /// </summary>
        public string smsId { get; set; }

        /// <summary>
        /// 模板参数JSON
        /// </summary>
        public string templateJson { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }

    }
}
