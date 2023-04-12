using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.Database
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class DatabaseTableFieldsSelectorOutput
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string fieldName { get; set; }
    }
}
