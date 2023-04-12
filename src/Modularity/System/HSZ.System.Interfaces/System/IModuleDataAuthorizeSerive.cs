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
    /// 描 述：数据权限
    /// </summary>
    public interface IModuleDataAuthorizeSerive
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="moduleId">功能id</param>
        /// <returns></returns>
        Task<List<ModuleDataAuthorizeEntity>> GetList(string moduleId);

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<ModuleDataAuthorizeEntity>> GetList();

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        Task<ModuleDataAuthorizeEntity> GetInfo(string id);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Create(ModuleDataAuthorizeEntity entity);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Update(ModuleDataAuthorizeEntity entity);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Delete(ModuleDataAuthorizeEntity entity);
    }
}
