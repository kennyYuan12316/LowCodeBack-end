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
    /// 描 述：菜单管理
    /// </summary>
    public interface IModuleService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<ModuleEntity>> GetList();

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        Task<ModuleEntity> GetInfo(string id);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        Task<int> Delete(ModuleEntity entity);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Create(ModuleEntity entity);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Update(ModuleEntity entity);

        /// <summary>
        /// 获取用户菜单树
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<dynamic> GetUserTreeModuleList(bool isAdmin, string userId,string type);

        /// <summary>
        /// 获取用户菜单列表
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<dynamic> GetUserModueList(bool isAdmin, string userId, string type);
    }
}
