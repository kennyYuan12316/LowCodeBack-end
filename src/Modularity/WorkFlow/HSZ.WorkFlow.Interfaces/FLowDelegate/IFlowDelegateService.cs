using HSZ.WorkFlow.Entitys;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.WorkFlow.Interfaces.FLowDelegate
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：流程委托
    /// </summary>
    public interface IFlowDelegateService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        Task<List<FlowDelegateEntity>> GetList(string userId);

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FlowDelegateEntity> GetInfo(string id);

        /// <summary>
        /// 所有委托人
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<string>> GetDelegateUserId(string userId,string flowId);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Delete(FlowDelegateEntity entity);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Create(FlowDelegateEntity entity);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Update(FlowDelegateEntity entity);

    }
}
