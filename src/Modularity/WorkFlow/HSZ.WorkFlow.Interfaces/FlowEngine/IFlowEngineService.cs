using HSZ.WorkFlow.Entitys;
using HSZ.WorkFlow.Entitys.Dto.FlowEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.WorkFlow.Interfaces.FlowEngine
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：流程引擎
    /// </summary>
    public interface IFlowEngineService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<FlowEngineEntity>> GetList();

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FlowEngineEntity> GetInfo(string id);

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="enCode">流程编码</param>
        /// <returns></returns>
        Task<FlowEngineEntity> GetInfoByEnCode(string enCode);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        Task<int> Delete(FlowEngineEntity entity);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="visibleList">可见范围</param>
        Task<FlowEngineEntity> Create(FlowEngineEntity entity, List<FlowEngineVisibleEntity> visibleList);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <param name="visibleList">可见范围</param>
        Task<int> Update(string id, FlowEngineEntity entity, List<FlowEngineVisibleEntity> visibleList);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="entity">实体对象</param>
        Task<int> Update(string id, FlowEngineEntity entity);

        /// <summary>
        /// 可见流程列表
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        Task<List<FlowEngineVisibleEntity>> GetVisibleFlowList(string userId);

        /// <summary>
        /// 可见流程列表
        /// </summary>
        /// <returns></returns>
        Task<List<FlowEngineListOutput>> GetFlowFormList();
    }
}
