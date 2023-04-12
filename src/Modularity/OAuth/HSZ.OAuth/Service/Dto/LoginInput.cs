using HSZ.Dependency;
using System.ComponentModel.DataAnnotations;

namespace HSZ.OAuth.Service.Dto
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：登录输入参数
    /// </summary>
    [SuppressSniffer]
    public class LoginInput
    {
        /// <summary>
        /// 用户名
        /// </summary>
        /// <example>admin</example>
        [Required(ErrorMessage = "用户名不能为空")]
        public string account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        /// <example>e10adc3949ba59abbe56e057f20f883e</example>
        [Required(ErrorMessage = "密码不能为空")]
        public string password { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 验证码时间戳
        /// </summary>
        public string timestamp { get; set; }

        /// <summary>
        /// 判断是否需要验证码
        /// </summary>
        public string origin { get; set; }
    }
}