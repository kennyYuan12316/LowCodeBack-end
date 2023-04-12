using HSZ.Common.Util;
using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.Permission.Group
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：分组下拉框输出
    /// </summary>
    [SuppressSniffer]
    public class GroupSelectorOutput : TreeModel
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
