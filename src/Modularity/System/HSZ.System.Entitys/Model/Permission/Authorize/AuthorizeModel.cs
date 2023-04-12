using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Model.Permission.Authorize
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：权限集合模型
    /// </summary>
    [SuppressSniffer]
    public class AuthorizeModel
    {
        /// <summary>
        /// 功能
        /// </summary>
        public List<AuthorizeModuleModel> ModuleList { get; set; }

        /// <summary>
        /// 按钮
        /// </summary>
        public List<AuthorizeModuleButtonModel> ButtonList { get; set; }

        /// <summary>
        /// 视图
        /// </summary>
        public List<AuthorizeModuleColumnModel> ColumnList { get; set; }

        /// <summary>
        /// 表单
        /// </summary>
        public List<AuthorizeModuleFormModel> FormList { get; set; }

        /// <summary>
        /// 资源
        /// </summary>
        public List<AuthorizeModuleResourceModel> ResourceList { get; set; }
    }
}
