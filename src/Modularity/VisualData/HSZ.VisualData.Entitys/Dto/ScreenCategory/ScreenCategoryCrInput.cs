namespace HSZ.VisualData.Entitys.Dto.ScreenCategory
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：大屏分类创建输入
    /// </summary>
    public class ScreenCategoryCrInput
    {
        /// <summary>
        /// 分类键值
        /// </summary>
        public string categoryKey { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string categoryValue { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public int isDeleted { get; set; }
    }
}
