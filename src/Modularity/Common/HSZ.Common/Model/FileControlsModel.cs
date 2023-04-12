namespace HSZ.Common.Model
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：文件控件模型
    /// </summary>
    public class FileControlsModel
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 文件ID
        /// </summary>
        public string fileId { get; set; }

        /// <summary>
        /// 下载地址
        /// </summary>
        public string url { get; set; }
    }
}