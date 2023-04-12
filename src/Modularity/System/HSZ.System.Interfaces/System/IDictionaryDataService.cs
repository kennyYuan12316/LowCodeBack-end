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
    /// 描 述：字典数据
    /// </summary>
    public interface IDictionaryDataService
    {
        /// <summary>
        /// 获取数据字典列表
        /// </summary>
        /// <param name="dictionaryTypeId">分类id或编码</param>
        /// <returns></returns>
        Task<List<DictionaryDataEntity>> GetList(string dictionaryTypeId);
        /// <summary>
        ///获取所有数据字典列表
        /// </summary>
        /// <returns></returns>
        Task<List<DictionaryDataEntity>> GetList();
        /// <summary>
        /// 获取按钮信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        Task<DictionaryDataEntity> GetInfo(string id);
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Create(DictionaryDataEntity entity);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Update(DictionaryDataEntity entity);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Delete(DictionaryDataEntity entity);
    }
}
