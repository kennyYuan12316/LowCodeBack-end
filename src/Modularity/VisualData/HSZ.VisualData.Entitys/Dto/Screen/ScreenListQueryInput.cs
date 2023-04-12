namespace HSZ.VisualData.Entitys.Dto.Screen
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：大屏列表查询输入
    /// </summary>
    public class ScreenListQueryInput
    {
        /// <summary>
        /// 大屏类型
        /// </summary>
        public int category { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int current { get; set; }

        /// <summary>
        /// 每页的数量
        /// </summary>
        public int size { get; set; }
    }
}
