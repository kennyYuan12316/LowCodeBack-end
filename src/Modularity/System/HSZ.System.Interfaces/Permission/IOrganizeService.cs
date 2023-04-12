using HSZ.System.Entitys.Dto.Permission.Organize;
using HSZ.System.Entitys.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.System.Interfaces.Permission
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：机构管理
    /// </summary>
    public interface IOrganizeService
    {
        /// <summary>
        /// 是否机构主管
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<bool> GetIsManagerByUserId(string userId);

        /// <summary>
        /// 获取机构列表
        /// 提供给其他服务使用
        /// </summary>
        /// <returns></returns>
        Task<List<OrganizeEntity>> GetListAsync();

        /// <summary>
        /// 获取公司列表
        /// 提供给其他服务使用
        /// </summary>
        /// <returns></returns>
        Task<List<OrganizeEntity>> GetCompanyListAsync();

        /// <summary>
        /// 下属机构
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <param name="isAdmin">是否管理员</param>
        /// <returns></returns>
        Task<string[]> GetSubsidiary(string organizeId, bool isAdmin);

        /// <summary>
        /// 下属机构
        /// </summary>
        /// <param name="organizeId"></param>
        /// <returns></returns>
        Task<List<string>> GetSubsidiary(string organizeId);

        /// <summary>
        /// 根据节点Id获取所有子节点Id集合，包含自己
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<string>> GetChildIdListWithSelfById(string id);

        /// <summary>
        /// 获取机构成员列表
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <returns></returns>
        Task<List<OrganizeMemberListOutput>> GetOrganizeMemberList(string organizeId);

        /// <summary>
        /// 部门信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<OrganizeEntity> GetInfoById(string Id);
    }
}
