using HSZ.Common.Util;
using HSZ.Dependency;

namespace HSZ.Extend.Entitys.Dto.Document
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：获取知识管理列表（文件夹树）
    /// </summary>
    [SuppressSniffer]
    public class DocumentFolderTreeOutput : TreeModel
    {
        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string fullName { get; set; }
    }
}
