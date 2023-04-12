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
    /// 描 述：业务契约：岗位管理
    /// </summary>
    public interface IPositionService
    {
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">获取信息</param>
        /// <returns></returns>
        Task<PositionEntity> GetInfoById(string id);

        /// <summary>
        /// 获取岗位列表
        /// </summary>
        /// <returns></returns>
        Task<List<PositionEntity>> GetListAsync();

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        string GetName(string ids);
    }
}
