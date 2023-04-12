using HSZ.Dependency;

namespace HSZ.Extend.Entitys.Dto.Document
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：获取文件/文件夹信息
    /// </summary>
    [SuppressSniffer]
    public class DocumentInfoOutput
    {
        /// <summary>
        /// 父级id
        /// </summary>
        public string parentId { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public int? type { get; set; }
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 文件名/文件夹名
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 后缀名
        /// </summary>
        public string fileExtension { get; set; }
        
    }
}
