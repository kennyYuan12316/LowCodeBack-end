using HSZ.Common.Util;
using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.Module
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：功能节点输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleNodeOutput : TreeModel
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 菜单编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 菜单分类【1-类别、2-页面】
        /// </summary>
        public int? type { get; set; }

        /// <summary>
        /// 菜单地址
        /// </summary>
        public string urlAddress { get; set; }

        /// <summary>
        /// 链接目标
        /// </summary>
        public string linkTarget { get; set; }

        /// <summary>
        /// 菜单分类：Web、App
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public string propertyJson { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
