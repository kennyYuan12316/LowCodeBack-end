using HSZ.System.Entitys.Model.Permission.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.System.Interfaces.Permission
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：分级管理
    /// </summary>
    public interface IOrganizeAdministratorService
    {
        /// <summary>
        /// 获取用户数据范围
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<UserDataScope>> GetUserDataScope(string userId);
    }
}
