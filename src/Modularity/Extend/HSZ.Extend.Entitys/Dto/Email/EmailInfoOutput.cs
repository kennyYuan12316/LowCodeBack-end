using HSZ.Dependency;
using System;

namespace HSZ.Extend.Entitys.Dto.Email
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：获取邮件信息
    /// </summary>
    [SuppressSniffer]
    public class EmailInfoOutput
    {
        /// <summary>
        /// 邮件主题	
        /// </summary>
        public string subject { get; set; }
        /// <summary>
        /// 发件人姓名	
        /// </summary>
        public string senderName { get; set; }
        /// <summary>
        /// 发件人邮箱	
        /// </summary>
        public string sender { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime? fdate { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        public string mAccount { get; set; }
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 附件对象
        /// </summary>
        public string attachment { get; set; }
        /// <summary>
        /// 邮件内容	
        /// </summary>
        public string bodyText { get; set; }
        /// <summary>
        /// 抄送人	
        /// </summary>
        public string cC { get; set; }
        /// <summary>
        /// 密送人	
        /// </summary>
        public string bCC { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        public string recipient { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
    }

}
