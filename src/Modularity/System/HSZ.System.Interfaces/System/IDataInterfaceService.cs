using HSZ.System.Entitys.System;
using HSZ.VisualDev.Entitys.Dto.VisualDev;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace HSZ.System.Interfaces.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：数据接口
    /// </summary>
    public interface IDataInterfaceService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<DataInterfaceEntity>> GetList();

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        Task<DataInterfaceEntity> GetInfo(string id);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<int> Create(DataInterfaceEntity entity);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<int> Delete(DataInterfaceEntity entity);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<int> Update(DataInterfaceEntity entity);

        /// <summary>
        /// sql接口查询
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        Task<DataTable> GetData(DataInterfaceEntity entity);

        /// <summary>
        /// 接口查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DataTable> GetData(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="tenantId"></param>
        /// <param name="input"></param>
        /// <param name="dicParameters"></param>
        /// <returns></returns>
        Task<object> GetResponseByType(string id, int type, string tenantId, VisualDevDataFieldDataListInput input = null, Dictionary<string, string> dicParameters = null);

        /// <summary>
        /// 替换参数默认值
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        void ReplaceParameterValue(DataInterfaceEntity entity, Dictionary<string, string> dic);
    }
}
