namespace HSZ.VisualData.Entitys.Dto.ScreenDataSource
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    public class ScreenDataSourceCrInput
    {
        /// <summary>
        /// 驱动类
        /// </summary>
        public string driverClass { get; set; }

        ///// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 连接地址
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
    }
}
