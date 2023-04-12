using HSZ.Common.Filter;
using HSZ.System.Entitys.Permission;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.System.Interfaces.Permission
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：业务契约：用户关系
    /// </summary>
    public interface IUserRelationService
    {
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        Task Delete(string id);

        /// <summary>
        /// 创建用户岗位关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ids">岗位ID</param>
        /// <returns></returns>
        List<UserRelationEntity> CreateByPosition(string userId, string ids);

        /// <summary>
        /// 创建用户角色关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ids">角色ID</param>
        /// <returns></returns>
        List<UserRelationEntity> CreateByRole(string userId, string ids);

        /// <summary>
        /// 创建用户组织关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ids">组织ID</param>
        /// <returns></returns>
        List<UserRelationEntity> CreateByOrganize(string userId, string ids);

        /// <summary>
        /// 创建用户分组关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ids">分组ID</param>
        /// <returns></returns>
        List<UserRelationEntity> CreateByGroup(string userId, string ids);

        /// <summary>
        /// 创建用户关系
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Create(List<UserRelationEntity> input);

        /// <summary>
        /// 根据用户主键获取列表
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        Task<List<UserRelationEntity>> GetListByUserId(string userId);

        /// <summary>
        /// 获取岗位
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        Task<List<string>> GetPositionId(string userId);

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objId"></param>
        /// <returns></returns>
        List<string> GetUserId(string type, string objId);

        /// <summary>
        /// 获取用户(分页)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objIds"></param>
        /// <param name="pageInputBase"></param>
        /// <returns></returns>
        Task<dynamic> GetUserIdPage(List<string> userIds, List<string> objIds, PageInputBase pageInputBase);
    }
}
