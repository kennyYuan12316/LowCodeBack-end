using HSZ.Common.Filter;
using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.Module
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：功能列表查询
    /// </summary>
    [SuppressSniffer]
    public class ModuleListQuery : KeywordInput
    {
        /// <summary>
        /// 分类
        /// </summary>
        public string category { get; set; }
    }
}
