using HSZ.Dependency;

namespace HSZ.WorkFlow.Entitys.Model
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class FormOperatesModel
    {
        /// <summary>
        /// 可读
        /// </summary>
        public bool read { get; set; }
        /// <summary>
        /// 字段名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 字段key
        /// </summary>
        public string id;
        /// <summary>
        /// 可写
        /// </summary>
        public bool write { get; set; }
        /// <summary>
        /// 必填
        /// </summary>
        public bool required { get; set; }
    }
}
