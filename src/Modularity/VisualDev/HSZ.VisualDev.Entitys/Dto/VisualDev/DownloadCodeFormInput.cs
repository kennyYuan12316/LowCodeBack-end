namespace HSZ.VisualDev.Entitys.Dto.VisualDev
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：下载代码表单输入
    /// </summary>
    public class DownloadCodeFormInput
    {
        /// <summary>
        /// 所属模块
        /// </summary>
        public string module { get; set; }

        /// <summary>
        /// 主功能名称
        /// </summary>
        public string className { get; set; }

        /// <summary>
        /// 子表名称集合
        /// </summary>
        public string subClassName { get; set; }

        /// <summary>
        /// 主功能备注
        /// </summary>
        public string description { get; set; }
    }
}
