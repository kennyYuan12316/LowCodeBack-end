using HSZ.Dependency;

namespace HSZ.Extend.Entitys.Dto.Email
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：发邮件
    /// </summary>
    [SuppressSniffer]
    public class EmailSendInput
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string subject { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        public string recipient { get; set; }
        /// <summary>
        /// 正文
        /// </summary>
        public string bodyText { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public string attachment { get; set; }
        /// <summary>
        /// 抄送人	
        /// </summary>
        public string cc { get; set; }
        /// <summary>
        /// 密送人	
        /// </summary>
        public string bcc { get; set; }
    }
}
