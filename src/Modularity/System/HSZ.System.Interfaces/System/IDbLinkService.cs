using HSZ.System.Entitys.Dto.System.DbLink;
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
    /// 描 述：数据连接
    /// </summary>
    public interface IDbLinkService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<DbLinkListOutput>> GetList();

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        Task<DbLinkEntity> GetInfo(string id);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Delete(DbLinkEntity entity);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Create(DbLinkEntity entity);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Update(DbLinkEntity entity);
    }
}
