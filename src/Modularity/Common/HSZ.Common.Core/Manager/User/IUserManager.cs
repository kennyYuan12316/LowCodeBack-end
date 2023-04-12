using HSZ.System.Entitys.Model.Permission.User;
using HSZ.System.Entitys.Permission;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.Common.Core.Manager
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：用户管理抽象
    /// </summary>
    public interface IUserManager
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// 租户ID
        /// </summary>
        string TenantId { get; }

        /// <summary>
        /// 租户数据库名称
        /// </summary>
        string TenantDbName { get; }

        /// <summary>
        /// 用户账号
        /// </summary>
        string Account { get; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        string RealName { get; }

        /// <summary>
        /// 当前用户 ToKen
        /// </summary>
        string ToKen { get; }

        /// <summary>
        /// 是否管理员
        /// </summary>
        bool IsAdministrator { get; }

        /// <summary>
        /// 用户信息
        /// </summary>
        UserEntity User { get; }

        /// <summary>
        /// 获取用户登录信息
        /// </summary>
        /// <returns></returns>
        Task<UserInfo> GetUserInfo();

        /// <summary>
        /// 获取数据条件
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        Task<List<IConditionalModel>> GetConditionAsync<T>(string moduleId, string primaryKey = "F_Id", bool isDataPermissions = true) where T : new();

        /// <summary>
        /// 获取数据条件(在线开发专用)
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        List<IConditionalModel> GetCondition<T>(string primaryKey, string moduleId, bool isDataPermissions = true) where T : new();

        /// <summary>
        /// 获取角色名称 根据 角色Ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<string> GetRoleNameByIds(string ids);

        /// <summary>
        /// 根据角色Ids和组织Id 获取组织下的角色以及全局角色
        /// </summary>
        /// <param name="roleIds">角色Id集合</param>
        /// <param name="organizeId">组织Id</param>
        /// <returns></returns>
        public Task<List<string>> GetUserOrgRoleIds(string roleIds, string organizeId);
    }
}
