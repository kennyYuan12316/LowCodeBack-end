using HSZ.Common.Util;
using Newtonsoft.Json;

namespace HSZ.VisualDev.Entitys.Dto.Portal
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：门户下拉框输出
    /// </summary>
    public class PortalSelectOutput : TreeModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [JsonIgnore]
        public string sortCode { get; set; }
        
        /// <summary>
        /// 有效标记
        /// </summary>
        [JsonIgnore]
        public int enabledMark { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        [JsonIgnore]
        public string deleteMark { get; set; }

        /// <summary>
        /// 类型(0-页面设计,1-自定义路径)
        /// </summary>
        public int? type { get; set; }

        /// <summary>
        /// 静态页面路径
        /// </summary>
        public string customUrl { get; set; }

        /// <summary>
        /// 链接类型(0-页面,1-外链)
        /// </summary>
        public int? linkType { get; set; }
    }
}
