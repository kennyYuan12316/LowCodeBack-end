namespace HSZ.VisualDev.Entitys.Dto.Portal
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：门户设计信息输出
    /// </summary>
    public class PortalInfoOutput
    {
        /// <summary>
        /// ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 是否可用
        /// 0-不可用，1-可用
        /// </summary>
        public int enabledMark { get; set; } = 0;

        /// <summary>
        /// 说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 表单JSON
        /// </summary>
        public string formData { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long sortCode { get; set; }

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
