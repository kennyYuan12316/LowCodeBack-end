using HSZ.Dependency;

namespace HSZ.Extend.Entitys.Dto.DocumentPreview
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：预览文档
    /// </summary>
    [SuppressSniffer]
    public class DocumentPreviewPreviewOutput
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string filePath { get; set; }

    }
}
