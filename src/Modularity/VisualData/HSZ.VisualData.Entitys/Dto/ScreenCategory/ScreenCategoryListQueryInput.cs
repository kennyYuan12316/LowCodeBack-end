namespace HSZ.VisualData.Entitys.Dto.ScreenCategory
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：大屏分类列表查询输入
    /// </summary>
    public class ScreenCategoryListQueryInput
    {
        /// <summary>
        /// 分类
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 当前页码:pageIndex
        /// </summary>
        public virtual int current { get; set; } = 1;

        /// <summary>
        /// 每页行数
        /// </summary>
        public virtual int size { get; set; } = 50;
    }
}
