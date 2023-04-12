using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.Permission.UsersCurrent
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：当前用户修改密码输入
    /// </summary>
    [SuppressSniffer]
    public class UsersCurrentActionsModifyPasswordInput
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        public string oldPassword { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 验证码时间戳
        /// </summary>
        public string timestamp { get; set; }
    }
}
