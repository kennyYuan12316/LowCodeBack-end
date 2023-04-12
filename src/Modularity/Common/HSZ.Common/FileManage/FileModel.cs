using System;

namespace HSZ.Common.FileManage
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：附件模型
    /// </summary>
    public class FileModel
    {
        public string FileId { get; set; }

        public string FileName { get; set; }

        public string FileSize { get; set; }

        public DateTime FileTime { get; set; }

        public string FileState { get; set; }

        public string FileType { get; set; }
    }
}
