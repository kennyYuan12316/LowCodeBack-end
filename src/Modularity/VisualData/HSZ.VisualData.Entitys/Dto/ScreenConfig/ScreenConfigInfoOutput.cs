namespace HSZ.VisualData.Entitys.Dto.ScreenConfig
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：大屏配置详情输出
    /// </summary>
    public class ScreenConfigInfoOutput
    {
        /// <summary>
        /// 组件json
        /// </summary>
        public string component { get; set; }

        /// <summary>
        /// 配置json
        /// </summary>
        public string detail { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 	可视化表主键
        /// </summary>
        public string visualId { get; set; }
    }
}
