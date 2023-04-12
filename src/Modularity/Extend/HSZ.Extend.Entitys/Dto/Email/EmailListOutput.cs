using HSZ.Dependency;
using System;

namespace HSZ.Extend.Entitys.Dto.Email
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：(带分页)获取邮件列表(收件箱、标星件、草稿箱、已发送)
    /// </summary>
    [SuppressSniffer]
    public class EmailListOutput
    {
        /// <summary>
        /// 是否已读(1-已读，0-未) inBox,star
        /// </summary>
        public int? isRead { get; set; }
        /// <summary>
        /// 时间  inBox,star
        /// </summary>
        public DateTime? fdate { get; set; }
        /// <summary>
        /// 主键ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string subject { get; set; }
        /// <summary>
        /// 是否标星(1-是,0-否)inBox,star
        /// </summary>
        public int? starred { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public string attachment { get; set; }
        /// <summary>
        /// 发件人 inBox,star
        /// </summary>
        public string sender { get; set; }
        /// <summary>
        /// 收件人 draft，sent
        /// </summary>
        public string recipient { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
    }
}
