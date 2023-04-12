using HSZ.TaskScheduler.Entitys.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.TaskScheduler.Interfaces.TaskScheduler
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：定时任务
    /// </summary>
    public interface ITimeTaskService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<TimeTaskEntity>> GetList();

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tenantDic">租户信息</param>
        /// <returns></returns>
        Task<TimeTaskEntity> GetInfo(string id, Dictionary<string, string> tenantDic = null);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Delete(TimeTaskEntity entity);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<TimeTaskEntity> Create(TimeTaskEntity entity);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Update(TimeTaskEntity entity, Dictionary<string, string> tenantDic = null);

        /// <summary>
        /// 执行记录
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        int CreateTaskLog(TimeTaskLogEntity entity, Dictionary<string, string> tenantDic = null);

        /// <summary>
        /// 启动自启动任务
        /// </summary>
        void StartTimerJob();

        /// <summary>
        /// 存在
        /// </summary>
        /// <returns></returns>
        public bool IsAny(string id);
    }
}
