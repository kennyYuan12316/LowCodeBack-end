using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.ModuleDataAuthorize
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：功能权限数据输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleDataAuthorizeOutput
    {
        /// <summary>
        /// 资源主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 资源名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 功能主键
        /// </summary>
        public string moduleId { get; set; }
    }
}
