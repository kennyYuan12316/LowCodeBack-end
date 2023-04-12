using HSZ.Dependency;

namespace HSZ.Extend.Entitys.Dto.Document
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：添加文件夹
    /// </summary>
    [SuppressSniffer]
    public class DocumentCrInput
    {
        /// <summary>
        /// 文件夹名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 文档父级
        /// </summary>
        public string parentId { get; set; }
        /// <summary>
        /// 文档分类
        /// </summary>
        public int? type { get; set; }
    }
}
