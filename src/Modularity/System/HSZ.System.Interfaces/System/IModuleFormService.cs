using HSZ.System.Entitys.System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.System.Interfaces.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：表单权限
    /// </summary>
    public interface IModuleFormService
    {
        /// <summary>
        /// 表单权限列表
        /// </summary>
        /// <param name="moduleId">功能id</param>
        /// <returns></returns>
        Task<List<ModuleFormEntity>> GetList(string moduleId);

        /// <summary>
        /// 获取用户功能列表
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<dynamic> GetUserModuleFormList(bool isAdmin, string userId);
    }
}
