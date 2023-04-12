using System.Collections.Generic;
using System.Drawing;

namespace HSZ.Common.Model.NPOI
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：Excel导出配置
    /// </summary>
    public class ExcelConfig
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 标题字号
        /// </summary>
        public short TitlePoint { get; set; }
        /// <summary>
        /// 标题高度
        /// </summary>
        public short TitleHeight { get; set; }
        /// <summary>
        /// 标题字体
        /// </summary>
        public string TitleFont { get; set; }
        /// <summary>
        /// 字体景色
        /// </summary>
        public Color ForeColor { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        public Color Background { get; set; }
        /// <summary>
        /// 列头字号
        /// </summary>
        public short HeadPoint { get; set; }
        /// <summary>
        /// 列标题高度
        /// </summary>
        public short HeadHeight { get; set; }
        /// <summary>
        /// 列头字体
        /// </summary>
        public string HeadFont { get; set; }
        /// <summary>
        /// 是否按内容长度来适应表格宽度
        /// </summary>
        public bool IsAllSizeColumn { get; set; }
        /// <summary>
        /// 列设置
        /// </summary>
        public List<ExcelColumnModel> ColumnModel { get; set; }
    }
}
