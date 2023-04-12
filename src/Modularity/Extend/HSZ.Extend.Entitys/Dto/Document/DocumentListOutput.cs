using HSZ.Dependency;
using System;

namespace HSZ.Extend.Entitys.Dto.Document
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：获取知识管理列表（全部文档）
    /// </summary>
    [SuppressSniffer]
    public class DocumentListOutput
    {
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? creatorTime { get; set; }
        /// <summary>
        /// 是否分享
        /// </summary>
        public int? isShare { get; set; }
        /// <summary>
        /// 类型(0-文件夹，1-文件)
        /// </summary>
        public int? type { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        public string fileSize { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 后缀名
        /// </summary>
        public string fileExtension { get; set; }
        /// <summary>
        /// 父级Id
        /// </summary>
        public string parentId { get; set; }
    }
}
