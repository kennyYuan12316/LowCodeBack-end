using HSZ.Common.Util;
using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.PrintDev
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class PrintDevSelectorOutput:TreeModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 流程分类(数据字典-工作流-流程分类)
        /// </summary>
        public string category { get; set; }
    }
}
