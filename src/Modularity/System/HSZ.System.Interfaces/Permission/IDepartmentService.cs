using HSZ.System.Entitys.Dto.Permission.Department;
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
    /// 描 述：业务契约：部门管理
    /// </summary>
    public interface IDepartmentService
    {
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <returns></returns>
        Task<List<OrganizeEntity>> GetListAsync();

        /// <summary>
        /// 部门名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetDepName(string id);

        /// <summary>
        /// 公司名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetComName(string id);

        /// <summary>
        /// 公司结构树
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetOrganizeNameTree(string id);

        /// <summary>
        /// 公司id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetCompanyId(string id);
    }
}
