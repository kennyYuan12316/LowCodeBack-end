using HSZ.Common.Filter;
using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Dto.Permission.Role
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：角色列表输入
    /// </summary>
    [SuppressSniffer]
    public class RoleListInput : PageInputBase
    {
        /// <summary>
        /// 机构ID 0 :全局，其他：组织
        /// </summary>
        public string organizeId { get; set; }

        /// <summary>
        /// 角色类型
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 组织Id集合
        /// </summary>
        public List<string> organizeIds { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        public string roleId { get; set; }
    }
}
