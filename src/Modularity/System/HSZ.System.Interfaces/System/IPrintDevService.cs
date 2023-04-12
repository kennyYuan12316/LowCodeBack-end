using HSZ.System.Entitys.Entity.System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.System.Interfaces.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：打印模板配置
    /// </summary>
    public interface IPrintDevService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        Task<List<PrintDevEntity>> GetList();

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        Task<PrintDevEntity> GetInfo(string id);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        Task<int> Delete(PrintDevEntity entity);

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Create(PrintDevEntity entity);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Update(PrintDevEntity entity);
    }
}
