using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HSZ.System.Interfaces.Permission
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：业务契约：操作权限
    /// </summary>
    public interface IAuthorizeService
    {
        /// <summary>
        /// 当前用户模块权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="isAdmin">是否超管</param>
        /// <param name="roleIds">用户角色Ids</param>
        /// <returns></returns>
        Task<List<ModuleEntity>> GetCurrentUserModuleAuthorize(string userId, bool isAdmin, string[] roleIds);

        /// <summary>
        /// 当前用户模块按钮权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="isAdmin">是否超管</param>
        /// <param name="roleIds">用户角色Ids</param>
        /// <returns></returns>
        Task<List<ModuleButtonEntity>> GetCurrentUserButtonAuthorize(string userId, bool isAdmin, string[] roleIds);

        /// <summary>
        /// 当前用户模块列权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="isAdmin">是否超管</param>
        /// <param name="roleIds">用户角色Ids</param>
        /// <returns></returns>
        Task<List<ModuleColumnEntity>> GetCurrentUserColumnAuthorize(string userId, bool isAdmin, string[] roleIds);

        /// <summary>
        /// 当前用户模块权限资源
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="isAdmin">是否超管</param>
        /// <param name="roleIds">用户角色Ids</param>
        /// <returns></returns>
        Task<List<ModuleDataAuthorizeSchemeEntity>> GetCurrentUserResourceAuthorize(string userId, bool isAdmin, string[] roleIds);

        /// <summary>
        /// 当前用户模块表单权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isAdmin"></param>
        /// <param name="roleIds">用户角色Ids</param>
        /// <returns></returns>
        Task<List<ModuleFormEntity>> GetCurrentUserFormAuthorize(string userId, bool isAdmin, string[] roleIds);

        /// <summary>
        /// 获取权限项ids
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="itemType">项类型</param>
        /// <returns></returns>
        Task<List<string>> GetAuthorizeItemIds(string roleId, string itemType);

        /// <summary>
        /// 是否存在权限资源
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<bool> GetIsExistModuleDataAuthorizeScheme(string[] ids);

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="objectId">对象主键</param>
        /// <returns></returns>
        Task<List<AuthorizeEntity>> GetAuthorizeListByObjectId(string objectId);
    }
}
