using HSZ.System.Entitys.Entity.System;
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
    /// 描 述：字典分类
    /// </summary>
    public interface IDictionaryTypeService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<DictionaryTypeEntity>> GetList();
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DictionaryTypeEntity> GetInfo(string id);
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Create(DictionaryTypeEntity entity);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Update(DictionaryTypeEntity entity);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Delete(DictionaryTypeEntity entity);
        /// <summary>
        /// 递归获取所有分类
        /// </summary>
        /// <param name="id"></param>
        /// <param name="typeList"></param>
        /// <returns></returns>
        Task GetListAllById(string id, List<DictionaryTypeEntity> typeList);
        /// <summary>
        /// 重复判断(分类)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool IsExistType(DictionaryTypeEntity entity);
        /// <summary>
        /// 重复判断(字典)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool IsExistData(DictionaryDataEntity entity);
        /// <summary>
        /// 是否存在上级
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        bool IsExistParent(List<DictionaryTypeEntity> entities);
    }
}
