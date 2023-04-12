using HSZ.Common.Filter;
using HSZ.System.Entitys.Dto.System.Province;
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
    /// 描 述：行政区划
    /// </summary>
    public interface IProvinceService
    {
        /// <summary>
        /// 获取行政区划列表
        /// </summary>
        /// <param name="nodeid">节点Id</param>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        Task<dynamic> GetList(KeywordInput input, string nodeid);

        /// <summary>
        /// 获取行政区划列表
        /// </summary>
        /// <returns></returns>
        Task<List<ProvinceEntity>> GetList();

        /// <summary>
        /// 获取行政区划下拉框数据(异步)
        /// </summary>
        /// <param name="id">当前Id</param>
        /// <returns></returns>
        Task<dynamic> GetSelector(string id,string areaId);
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        Task<dynamic> GetInfo(string id);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        Task Delete(string id);
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        Task Create(ProvinceCrInput input);
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        Task Update(string id,ProvinceUpInput input);
        /// <summary>
        /// 更新行政区划状态
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        Task ActionsState(string id);
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        Task<List<ProvinceEntity>> GetList(string id);
    }
}