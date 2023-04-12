using System.ComponentModel;

namespace HSZ.VisualData.Entitys.Enum
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：大屏图片枚举
    /// </summary>
    public enum ScreenImgEnum
    {
        /// <summary>
        /// 背景图片
        /// </summary>
        [Description("bg")]
        BG = 0,

        /// <summary>
        /// 图片框
        /// </summary>
        [Description("border")]
        BORDER = 1,

        /// <summary>
        /// 图片
        /// </summary>
        [Description("source")]
        SOURCE = 1,

        /// <summary>
        /// banner
        /// </summary>
        [Description("banner")]
        BANNER = 3,

        /// <summary>
        /// 大屏截图
        /// </summary>
        [Description("screenShot")]
        SCREENSHOT = 4,
    }
}
