using System;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Dto.System.SysConfig
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：用户信息输出
    /// </summary>
    public class AdminUserListOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string realName { get; set; }
    }
}
