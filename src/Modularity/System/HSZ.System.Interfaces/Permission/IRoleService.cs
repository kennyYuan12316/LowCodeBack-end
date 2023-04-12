using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.System.Interfaces.Permission
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：业务契约：角色信息
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// 名称
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<string> GetName(string ids);

    }
}
