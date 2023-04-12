using HSZ.Dependency;
using System;

namespace HSZ.System.Entitys.Dto.System.DbBackup
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class DbBackupListOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string fileSize { get; set; }

        /// <summary>
        /// 备份时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 文件下载地址
        /// </summary>
        public string fileUrl { get; set; }
    }
}
