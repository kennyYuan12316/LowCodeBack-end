using HSZ.Common.Util;
using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.Module
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：功能下拉框全部输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleSelectorAllOutput : TreeModel
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 跳转地址
        /// </summary>
        public string urlAddress { get; set; }
        /// <summary>
        /// 扩展属性
        /// </summary>
        public string propertyJson { get; set; }
        /// <summary>
        /// 启用状态
        /// </summary>
        public int? enabledMark { get; set; }
    }
}
