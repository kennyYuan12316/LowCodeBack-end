using HSZ.Common.Util;
using HSZ.Dependency;
using System;

namespace HSZ.System.Entitys.Dto.Permission.Organize
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：机构树列表输出
    /// </summary>
    [SuppressSniffer]
    public class OrganizeListOutput : TreeModel
    {
        /// <summary>
        /// 集团名
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; } = "icon-sz icon-sz-tree-department1";
    }
}