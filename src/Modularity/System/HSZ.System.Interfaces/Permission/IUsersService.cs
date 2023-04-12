using HSZ.Common.Filter;
using HSZ.System.Entitys.Model.Permission.User;
using HSZ.System.Entitys.Permission;
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
    /// 描 述：业务契约：用户信息
    /// </summary>
    public interface IUsersService
    {
        /// <summary>
        /// 获取用户信息 根据用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        UserEntity GetInfoByUserId(string userId);

        /// <summary>
        /// 获取用户信息 根据用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<UserEntity> GetInfoByUserIdAsync(string userId);

        /// <summary>
        /// 根据用户账户
        /// </summary>
        /// <param name="account">用户账户</param>
        /// <returns></returns>
        Task<UserEntity> GetInfoByAccount(string account);

        /// <summary>
        /// 获取用户信息 根据登录信息
        /// </summary>
        /// <param name="account">用户账户</param>
        /// <param name="password">用户密码</param>
        /// <returns></returns>
        Task<UserEntity> GetInfoByLogin(string account, string password);

        /// <summary>
        /// 根据用户姓名获取用户ID
        /// </summary>
        /// <param name="realName">用户姓名</param>
        /// <returns></returns>
        Task<string> GetUserIdByRealName(string realName);

        /// <summary>
        /// 获取用户名称
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<string> GetUserName(string userId);

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<UserEntity>> GetList();

        /// <summary>
        /// 用户岗位
        /// </summary>
        /// <param name="PositionIds"></param>
        /// <returns></returns>
        Task<List<PositionInfo>> GetPosition(string PositionIds);

        /// <summary>
        /// 根据id数组获取用户（分页）
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="pageInputBase"></param>
        /// <returns></returns>
        Task<dynamic> GetUserPageList(List<string> userIds, PageInputBase pageInputBase);

        /// <summary>
        /// 表达式获取用户
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<UserEntity> GetUserByExp(Expression<Func<UserEntity, bool>> expression);

        /// <summary>
        /// 表达式获取用户列表
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<List<UserEntity>> GetUserListByExp(Expression<Func<UserEntity, bool>> expression);

        /// <summary>
        /// 表达式获取指定字段的用户列表
        /// </summary>
        /// <param name="expression">where 条件表达式</param>
        /// <param name="select">select 选择字段表达式</param>
        /// <returns></returns>
        Task<List<UserEntity>> GetUserListByExp(Expression<Func<UserEntity, bool>> expression, Expression<Func<UserEntity, UserEntity>> select);
    }
}
