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
    /// 描 述：功能按钮
    /// </summary>
    public interface IModuleButtonService
    {
        /// <summary>
        /// 获取按钮权限列表
        /// </summary>
        /// <param name="moduleId">功能id</param>
        /// <returns></returns>
        Task<List<ModuleButtonEntity>> GetList(string moduleId);

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<ModuleButtonEntity>> GetList();

        /// <summary>
        /// 获取按钮信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        Task<ModuleButtonEntity> GetInfo(string id);

        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Create(ModuleButtonEntity entity);

        /// <summary>
        /// 添加按钮批量
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        Task<int> Create(List<ModuleButtonEntity> entitys);

        /// <summary>
        /// 修改按钮
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Update(ModuleButtonEntity entity);
        /// <summary>
        /// 删除按钮
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Delete(ModuleButtonEntity entity);

        /// <summary>
        /// 获取用户功能按钮
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<dynamic> GetUserModuleButtonList(bool isAdmin, string userId);
    }
}
