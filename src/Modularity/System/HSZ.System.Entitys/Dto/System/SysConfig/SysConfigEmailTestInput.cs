using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.SysConfig
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：测试邮箱连接输入
    /// </summary>
    [SuppressSniffer]
    public class SysConfigEmailTestInput
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// POP3服务URL
        /// </summary>
        public string pop3Host { get; set; }

        /// <summary>
        /// POP3端口
        /// </summary>
        public string pop3Port { get; set; }

        /// <summary>
        /// SMTP服务URL
        /// </summary>
        public string smtpHost { get; set; }

        /// <summary>
        /// SMTP端口
        /// </summary>
        public string smtpPort { get; set; }

        /// <summary>
        /// 是否开启SSL
        /// </summary>
        public int? ssl { get; set; }
    }
}
