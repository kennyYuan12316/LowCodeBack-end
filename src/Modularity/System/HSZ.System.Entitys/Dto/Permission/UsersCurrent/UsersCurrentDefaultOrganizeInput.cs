using HSZ.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.System.Entitys.Dto.Permission.UsersCurrent
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：用户切换3个默认 输入
    /// </summary>
    [SuppressSniffer]
    public class UsersCurrentDefaultOrganizeInput
    {
        /// <summary>
        /// 默认切换类型：Organize：组织，Position：岗位：Role：角色
        /// </summary>
        public string majorType { get; set; }

        /// <summary>
        /// 默认切换Id（组织Id、岗位Id、角色Id）
        /// </summary>
        public string majorId { get; set; }


    }
}
