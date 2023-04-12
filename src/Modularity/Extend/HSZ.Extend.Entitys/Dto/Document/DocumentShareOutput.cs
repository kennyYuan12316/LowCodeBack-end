using HSZ.Dependency;
using System;

namespace HSZ.Extend.Entitys.Dto.Document
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：知识管理（我的共享列表）
    /// </summary>
    [SuppressSniffer]
    public class DocumentShareOutput
    {
        /// <summary>
        /// 共享日期
        /// </summary>
        public DateTime? shareTime { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        public string fileSize { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public int? type { get; set; }
        /// <summary>
        /// 后缀名
        /// </summary>
        public string fileExtension { get; set; }
    }
}
