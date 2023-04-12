using HSZ.VisualData.Entitys.Dto.ScreenConfig;

namespace HSZ.VisualData.Entitys.Dto.Screen
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：大屏修改输入
    /// </summary>
    public class ScreenUpInput
    {
        /// <summary>
        /// 
        /// </summary>
        public ScreenConfigUpInput config { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ScreenEntityUpInput visual { get; set; }
    }

    /// <summary>
    /// 大屏实体修改输入
    /// </summary>
    public class ScreenEntityUpInput : ScreenEntityCrInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 业务状态
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 背景图片
        /// </summary>
        public string backgroundUrl { get; set; }
    }
}
