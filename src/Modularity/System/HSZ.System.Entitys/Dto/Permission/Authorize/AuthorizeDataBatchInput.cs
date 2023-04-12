using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Dto.Permission.Authorize
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：批量新增权限数据输入
    /// </summary>
    [SuppressSniffer]
    public class AuthorizeDataBatchInput
    {
        /// <summary>
        /// 角色ids
        /// </summary>
        public List<string> roleIds { get; set; }

        /// <summary>
        /// 按钮权限ids
        /// </summary>
        public List<string> button { get; set; }

        /// <summary>
        /// 列表权限ids
        /// </summary>
        public List<string> column { get; set; }

        /// <summary>
        /// 表单权限ids
        /// </summary>
        public List<string> form { get; set; }

        /// <summary>
        /// 菜单权限ids
        /// </summary>
        public List<string> module { get; set; }

        /// <summary>
        /// 数据权限ids
        /// </summary>
        public List<string> resource { get; set; }

        /// <summary>
        /// 岗位ids
        /// </summary>
        public List<string> positionIds { get; set; }

        /// <summary>
        /// 用户ids
        /// </summary>
        public List<string> userIds { get; set; }
    }
}
