using HSZ.Common.Util;

namespace HSZ.VisualDev.Entitys.Dto.VisualDev
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：在线开发下拉框输出
    /// </summary>
    public class VisualDevSelectorOutput : TreeModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public long? SortCode { get; set; }
    }
}
