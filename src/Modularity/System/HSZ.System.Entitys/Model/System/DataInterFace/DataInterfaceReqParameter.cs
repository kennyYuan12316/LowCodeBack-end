using HSZ.Dependency;

namespace HSZ.System.Entitys.Model.System.DataInterFace
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class DataInterfaceReqParameter
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string defaultValue { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string dataType { get; set; }
        /// <summary>
        /// 必填
        /// </summary>
        public string required { get; set; }
        /// <summary>
        /// 注释
        /// </summary>
        public string fieldName { get; set; }
    }
}
