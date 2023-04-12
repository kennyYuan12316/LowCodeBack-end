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
    /// 描 述：当前用户 所属组织 输出
    /// </summary>
    [SuppressSniffer]
    public class CurrentUserOrganizesOutput
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 是否默认 0：否，1：是
        /// </summary>
        public bool isDefault { get; set; } = false;

    }
}
