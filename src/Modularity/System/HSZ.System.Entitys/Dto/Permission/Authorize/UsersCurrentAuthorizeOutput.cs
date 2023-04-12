using HSZ.Dependency;
using HSZ.System.Entitys.Model.Permission.UsersCurrent;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Dto.Permission.Authorize
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：当前用权限输出
    /// </summary>
    [SuppressSniffer]
    public class UsersCurrentAuthorizeOutput
    {
        /// <summary>
        /// 模型
        /// </summary>
        public List<UsersCurrentAuthorizeMoldel> module { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        public List<UsersCurrentAuthorizeMoldel> column { get; set; }

        /// <summary>
        /// 按钮
        /// </summary>
        public List<UsersCurrentAuthorizeMoldel> button { get; set; }

        /// <summary>
        /// 数据权限资源
        /// </summary>
        public List<UsersCurrentAuthorizeMoldel> resource { get; set; }
    }
}
