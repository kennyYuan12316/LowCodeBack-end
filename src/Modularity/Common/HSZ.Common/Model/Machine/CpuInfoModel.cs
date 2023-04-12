using HSZ.Dependency;

namespace HSZ.Common.Model.Machine
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class CpuInfoModel
    {
        /// <summary>
        /// CPU名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 物理CPU个数
        /// </summary>
        public string package { get; set; }
        /// <summary>
        /// CPU内核个数
        /// </summary> 
        public string core { get; set; }
        /// <summary>
        /// 内核个数
        /// </summary>
        public int coreNumber { get; set; }
        /// <summary>
        /// 逻辑CPU个数
        /// </summary>
        public string logic { get; set; }
        /// <summary>
        /// CPU已用百分比
        /// </summary>
        public string used { get; set; }
        /// <summary>
        /// 未用百分比
        /// </summary>
        public string idle { get; set; }
    }
}
