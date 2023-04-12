using HSZ.VisualData.Entitys.Dto.ScreenConfig;

namespace HSZ.VisualData.Entitys.Dto.Screen
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    public class ScreenCrInput
    {
        /// <summary>
        /// 
        /// </summary>
        public ScreenConfigCrInput config { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ScreenEntityCrInput visual { get; set; }
    }

    public class ScreenEntityCrInput
    {
        /// <summary>
        /// 大屏类型
        /// </summary>
        public int category { get; set; }

        /// <summary>
        /// 创建部门
        /// </summary>
        public string createDept { get; set; }

        /// <summary>
        /// 发布密码
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 大屏标题
        /// </summary>
        public string title { get; set; }
    }
}
