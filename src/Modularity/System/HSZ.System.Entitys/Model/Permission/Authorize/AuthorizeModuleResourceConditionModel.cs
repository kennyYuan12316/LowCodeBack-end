using System.Collections.Generic;

namespace HSZ.System.Entitys.Model.Permission.Authorize
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：数据权限条件
    /// </summary>
    public class AuthorizeModuleResourceConditionModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Logic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<AuthorizeModuleResourceConditionItemModel> Groups { get; set; }
    }
}
