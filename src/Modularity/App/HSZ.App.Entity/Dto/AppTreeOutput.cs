using HSZ.Common.Util;
using HSZ.Dependency;
using System;

namespace HSZ.Apps.Entitys.Dto
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class AppTreeOutput:TreeModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 表单类型
        /// </summary>
        public string formType { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        public string iconBackground { get; set; }
        /// <summary>
        /// 可见类型
        /// </summary>
        public string visibleType { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string creatorUser { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
        /// <summary>
        /// 状态标识
        /// </summary>
        public int? enabledMark { get; set; }
        /// <summary>
        /// 是否常用
        /// </summary>
        public bool isData { get; set; }
    }
}
