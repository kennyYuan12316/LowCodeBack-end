namespace HSZ.Common.Model.NPOI
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：Excel导出模板
    /// </summary>
    public class ExcelTemplateModel
    {
        /// <summary>
        /// 行号
        /// </summary>
        public int row { get; set; }
        /// <summary>
        /// 列号
        /// </summary>
        public int cell { get; set; }
        /// <summary>
        /// 数据值
        /// </summary>
        public string value { get; set; }
    }
}
