using HSZ.Dependency;

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
    public class AppDataListOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 对象id
        /// </summary>
        public string objectId { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public string objectData { get; set; }
    }
}
