using HSZ.Dependency;
using System.ComponentModel.DataAnnotations;

namespace HSZ.System.Entitys.Dto.System.ModuleButton
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：功能按钮创建输入
    /// </summary>
    [SuppressSniffer]
    public class ModuleButtonCrInput
    {
        /// <summary>
        /// 功能主键
        /// </summary>
        public string moduleId { get; set; }

        /// <summary>
        /// 按钮名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 按钮图标	
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 按钮编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 按钮状态(1-可用,0-不可用)
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 按钮说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 上级菜单
        /// </summary>
        public string parentId { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public long? sortCode { get; set; }
    }
}
