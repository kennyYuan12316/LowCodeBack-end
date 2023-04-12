using HSZ.Common.Util;
using System;

namespace HSZ.VisualDev.Entitys.Dto.Portal
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：获取门户列表输出
    /// </summary>
    public class PortalListOutput : TreeModel
    {
        /// <summary>
        /// 分类
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string creatorUser { get; set; }

        /// <summary>
        /// 最后修改人
        /// </summary>
        public string lastModifyUser { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? lastModifyTime { get; set; }

        /// <summary>
        /// 是否可用
        /// 0-不可用，1-可用
        /// </summary>
        public int? enabledMark { get; set; } = 0;

        /// <summary>
        /// 排序
        /// </summary>
        public string sortCode { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
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
