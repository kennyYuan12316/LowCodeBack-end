using HSZ.Dependency;
using HSZ.System.Entitys.Dto.System.Module;
using HSZ.System.Entitys.Dto.System.ModuleButton;
using HSZ.System.Entitys.Dto.System.ModuleColumn;
using HSZ.System.Entitys.Dto.System.ModuleDataAuthorizeScheme;
using System.Collections.Generic;

namespace HSZ.OAuth.Service
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：当前用户模型输出
    /// </summary>
    [SuppressSniffer]
    public class CurrentUserModelOutput
    {
        /// <summary>
        /// 菜单权限
        /// </summary>
        public List<ModuleOutput> moduleList { get; set; }

        /// <summary>
        /// 按钮权限
        /// </summary>
        public List<ModuleButtonOutput> buttonList { get; set; }

        /// <summary>
        /// 列表权限
        /// </summary>
        public List<ModuleColumnOutput> columnList { get; set; }

        /// <summary>
        /// 表单权限
        /// </summary>
        public List<ModuleColumnOutput> formList { get; set; }

        /// <summary>
        /// 数据权限
        /// </summary>
        public List<ModuleDataAuthorizeSchemeOutput> resourceList { get; set; }
    }
}