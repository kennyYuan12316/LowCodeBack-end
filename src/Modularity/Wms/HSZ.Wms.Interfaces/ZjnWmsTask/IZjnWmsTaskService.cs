using HSZ.Common.Enum;
using HSZ.Common.TaskResultPubilcParms;
using HSZ.Entitys.wms;
using HSZ.UnifyResult;
using HSZ.wms.Entitys.Dto.ZjnWmsTask;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ZJN.Calb.Entitys.Dto;

namespace HSZ.wms.Interfaces.ZjnWmsTask
{
    public interface IZjnWmsTaskService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Create(ZjnWmsTaskCrInput input);

        /// <summary>
        /// 根据业务流程配置生成任务
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<ZjnWmsTaskCrInput> CreateByConfigId(string id, ZjnWmsTaskCrInput taskInput);

        /// <summary>
        /// App出库
        /// </summary>
        /// <param name="TaskCrInput">出库信息</param>
        /// <returns></returns>
        Task AppOutbound(ZjnWmsTaskCrInput TaskCrInput);

        /// <summary>
        /// Les调用此方法生成出入库任务
        /// </summary>
        /// <param name="taskWarehouseRequest"></param>
        /// <returns></returns>
        Task CreateByTaskWarehouse(TaskWarehouseRequest taskWarehouseRequest);


        /// <summary>
        /// 获取下一个节点
        /// </summary>
        /// <param name="taskDetailsId">子任务id</param>
        /// <returns></returns>
        Task<ZjnWmsTaskDetailsInfoOutput> GetNextTaskDetails(string taskDetailsId);

        /// <summary>
        /// 重置子任务
        /// </summary>
        /// <param name="id">子任务id</param>
        /// <param name="states">任务类型</param>
        /// <returns></returns>
        Task ResetTask(string id, int states = 1);

        /// <summary>
        /// 更新子任务终点目的地
        /// </summary>
        /// <param name="id">子任务id</param>
        /// <param name="siteId">站点id</param>
        /// <param name="processType">流程类型</param>
        /// <returns></returns>
        Task UpadteTaskSite(string id, string siteId,int processType);

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="id">子任务id</param>
        /// <returns></returns>
        Task CancelTask(string id);



        /// <summary>
        ///  WCS完成任务上报函数 根据传入任务类型 再分发函数进行处理
        /// </summary>
        /// <param name="TaskType">任务类型</param>
        /// <param name="TaskNo">子任务号</param>
        /// <param name="TaskState">任务完成状态</param>
        /// <param name="parameter">公共参数</param>
        /// <returns></returns>
        Task<RESTfulResult<ZjnWmsTaskDetailsInfoOutput>> TaskProcesByWcs(int TaskNo, int TaskState, TaskResultPubilcParameter parameter);

        /// <summary>
        /// WCS接收子任务回调函数
        /// </summary>
        /// <param name="TaskNo">子任务号</param>
        /// <param name="TaskState">状态</param>
        /// <returns></returns>
        Task<RESTfulResult<bool>> WCSReceivesTheSubtaskCallback(string TaskNo, int TaskState);

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="taskDetails">子任务</param>
        /// <param name="taskState">状态</param>
        /// <returns></returns>
        Task<ZjnWmsTaskDetailsInfoOutput> FinishTask(ZjnWmsTaskDetailsEntity taskDetails, int taskState, TaskResultPubilcParameter parameter=null);

        /// <summary>
        /// 获取子任务dto
        /// </summary>
        /// <param name="taskDetailsId">子任务id</param>
        /// <returns></returns>
        Task<ZjnWmsTaskDetailsInfoOutput> GetZjnWmsTaskDetailsInfoOutput(string taskDetailsId);


    }
}