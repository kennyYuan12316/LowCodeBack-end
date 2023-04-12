namespace HSZ.System.Entitys.Model.Permission.User
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：用户数据范围集合
    /// </summary>
    public class UserDataScope
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public string organizeId { get; set; }

        /// <summary>
        /// 新增
        /// </summary>
        public bool Add { get; set; }

        /// <summary>
        /// 编辑
        /// </summary>
        public bool Edit { get; set; }

        /// <summary>
        /// 删除
        /// </summary>
        public bool Delete {  get; set; }
    }
}
