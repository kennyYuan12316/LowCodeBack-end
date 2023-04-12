using HSZ.Common.Core.Manager;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Dependency;
using HSZ.LinqBuilder;
using HSZ.System.Entitys.Permission;
using HSZ.WorkFlow.Entitys;
using HSZ.WorkFlow.Entitys.Dto.FlowBefore;
using HSZ.WorkFlow.Entitys.Dto.FlowLaunch;
using HSZ.WorkFlow.Entitys.Dto.FlowMonitor;
using HSZ.WorkFlow.Entitys.Model;
using HSZ.WorkFlow.Interfaces.FlowTask.Repository;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.WorkFlow.FlowTask.Repository
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：流程任务
    /// </summary>
    public class FlowTaskRepository : IFlowTaskRepository, ITransient
    {
        private readonly ISqlSugarRepository<FlowTaskEntity> _flowTaskRepository;
        private readonly ISqlSugarRepository<FlowTaskNodeEntity> _flowTaskNodeRepository;
        private readonly ISqlSugarRepository<FlowTaskOperatorEntity> _flowTaskOperatorRepository;
        private readonly ISqlSugarRepository<FlowTaskOperatorRecordEntity> _flowTaskOperatorRecordRepository;
        private readonly ISqlSugarRepository<FlowTaskCirculateEntity> _flowTaskCirculateRepository;
        private readonly ISqlSugarRepository<FlowCandidatesEntity> _flowCandidatesRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope Db;// 核心对象：拥有完整的SqlSugar全部功能

        /// <summary>
        ///
        /// </summary>
        /// <param name="flowTaskRepository"></param>
        /// <param name="flowTaskNodeRepository"></param>
        /// <param name="flowTaskOperatorRepository"></param>
        /// <param name="flowTaskOperatorRecordRepository"></param>
        /// <param name="flowTaskCirculateRepository"></param>
        /// <param name="userManager"></param>
        public FlowTaskRepository(ISqlSugarRepository<FlowTaskEntity> flowTaskRepository,
            ISqlSugarRepository<FlowTaskNodeEntity> flowTaskNodeRepository,
            ISqlSugarRepository<FlowTaskOperatorEntity> flowTaskOperatorRepository,
            ISqlSugarRepository<FlowTaskOperatorRecordEntity> flowTaskOperatorRecordRepository,
            ISqlSugarRepository<FlowTaskCirculateEntity> flowTaskCirculateRepository, ISqlSugarRepository<FlowCandidatesEntity> flowCandidatesRepository,
            IUserManager userManager)
        {
            _flowTaskRepository = flowTaskRepository;
            _flowTaskNodeRepository = flowTaskNodeRepository;
            _flowTaskOperatorRepository = flowTaskOperatorRepository;
            _flowTaskOperatorRecordRepository = flowTaskOperatorRecordRepository;
            _flowTaskCirculateRepository = flowTaskCirculateRepository;
            _flowCandidatesRepository = flowCandidatesRepository;
            _userManager = userManager;
            Db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 列表（流程监控）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        public async Task<dynamic> GetMonitorList(FlowMonitorListQuery input)
        {
            var whereLambda = LinqExpression.And<FlowMonitorListOutput>();
            if (!input.startTime.IsNullOrEmpty() && !input.endTime.IsNullOrEmpty())
            {
                var start = Ext.GetDateTime(input.startTime.ToString());
                var end = Ext.GetDateTime(input.endTime.ToString());
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.creatorTime, start, end));
            }
            if (!input.creatorUserId.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.creatorUserId == input.creatorUserId);
            if (!input.flowCategory.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowCategory == input.flowCategory);
            if (!input.creatorUserId.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.creatorUserId.Contains(input.creatorUserId));
            if (!input.flowId.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowId == input.flowId);
            if (!input.status.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.status == input.status);
            if (!input.keyword.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.enCode.Contains(input.keyword) || m.fullName.Contains(input.keyword));
            var list = await _flowTaskRepository.AsSugarClient().Queryable<FlowTaskEntity, FlowEngineEntity, UserEntity>((a, b, c) => new JoinQueryInfos(JoinType.Left, a.FlowId == b.Id, JoinType.Left, a.CreatorUserId == c.Id)).Where(a => a.Status > 0 && a.DeleteMark == null).Select((a, b, c) => new FlowMonitorListOutput()
            {
                completion = a.Completion,
                creatorTime = a.CreatorTime,
                creatorUserId = a.CreatorUserId,
                description = a.Description,
                enCode = a.EnCode,
                flowCategory = a.FlowCategory,
                flowCode = a.FlowCode,
                flowId = a.FlowId,
                flowName = b.FullName,
                formUrgent = a.FlowUrgent,
                formData = b.FormTemplateJson,
                formType = b.FormType,
                fullName = a.FullName,
                id = a.Id,
                processId = a.ProcessId,
                startTime = a.StartTime,
                thisStep = a.ThisStep,
                userName = SqlFunc.MergeString(c.RealName, "/", c.Account),
                status = a.Status,
                sortCode = a.SortCode
            }).MergeTable().Where(whereLambda).OrderBy(a => a.sortCode)
            .OrderBy(a => a.creatorTime, OrderByType.Desc)
            .ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<FlowMonitorListOutput>()
            {
                list = list.list,
                pagination = list.pagination
            };
            return PageResult<FlowMonitorListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 列表（我发起的）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        public async Task<dynamic> GetLaunchList(FlowLaunchListQuery input)
        {
            var whereLambda = LinqExpression.And<FlowLaunchListOutput>();
            whereLambda = whereLambda.And(x => x.creatorUserId == _userManager.UserId);
            if (!input.startTime.IsNullOrEmpty() && !input.endTime.IsNullOrEmpty())
            {
                var start = Ext.GetDateTime(input.startTime.ToString());
                var end = Ext.GetDateTime(input.endTime.ToString());
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.creatorTime, start, end));
            }
            if (!input.flowCategory.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowCategory == input.flowCategory);
            if (!input.flowId.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowId == input.flowId);
            if (!input.status.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.status == input.status);
            if (!input.keyword.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.enCode.Contains(input.keyword) || m.fullName.Contains(input.keyword));

            var list = await _flowTaskRepository.AsSugarClient().Queryable<FlowTaskEntity, FlowEngineEntity, UserEntity>((a, b, c) => new JoinQueryInfos(JoinType.Left, a.FlowId == b.Id, JoinType.Left, a.CreatorUserId == c.Id)).Where((a, b) => a.DeleteMark == null).Select((a, b, c) => new FlowLaunchListOutput()
            {
                completion = a.Completion,
                creatorTime = a.CreatorTime,
                creatorUserId = a.CreatorUserId,
                endTime = a.EndTime,
                description = a.Description,
                enCode = a.EnCode,
                flowCategory = a.FlowCategory,
                flowCode = a.FlowCode,
                flowId = a.FlowId,
                flowName = b.FullName,
                formData = b.FormTemplateJson,
                formType = b.FormType,
                fullName = a.FullName,
                id = a.Id,
                startTime = a.StartTime,
                thisStep = a.ThisStep,
                status = a.Status
            }).MergeTable().Where(whereLambda).OrderBy(a => a.status).OrderBy(a => a.startTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<FlowLaunchListOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 列表（待我审批）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        public async Task<dynamic> GetWaitList(FlowBeforeListQuery input)
        {
            if (_flowTaskRepository.AsSugarClient().CurrentConnectionConfig.DbType == DbType.Oracle)
            {
                return await GetWaitList_Oracle(input);
            }
            else
            {
                var whereLambda = LinqExpression.And<FlowBeforeListOutput>();
                if (!input.startTime.IsNullOrEmpty() && !input.endTime.IsNullOrEmpty())
                {
                    var start = Ext.GetDateTime(input.startTime.ToString());
                    var end = Ext.GetDateTime(input.endTime.ToString());
                    start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                    end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                    whereLambda = whereLambda.And(x => SqlFunc.Between(x.creatorTime, start, end));
                }
                if (!input.flowCategory.IsNullOrEmpty())
                    whereLambda = whereLambda.And(x => x.flowCategory == input.flowCategory);
                if (!input.flowId.IsNullOrEmpty())
                    whereLambda = whereLambda.And(x => x.flowId == input.flowId);
                if (!input.keyword.IsNullOrEmpty())
                    whereLambda = whereLambda.And(m => m.enCode.Contains(input.keyword) || m.fullName.Contains(input.keyword));
                if (!input.creatorUserId.IsNullOrEmpty())
                    whereLambda = whereLambda.And(m => m.creatorUserId.Contains(input.creatorUserId));
                //经办审核
                var list1 = _flowTaskRepository.AsSugarClient().Queryable<FlowTaskEntity, FlowTaskOperatorEntity, UserEntity, FlowEngineEntity>((a, b, c, d) =>
                new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, a.CreatorUserId == c.Id, JoinType.Left, a.FlowId == d.Id))
                    .Where((a, b, c) => a.Status == 1 && a.DeleteMark == null && b.Completion == 0 && b.State == "0"
                    && (SqlFunc.ToDate(SqlFunc.ToString(b.Description)) < DateTime.Now || b.Description == null)
                    && b.HandleId == _userManager.UserId)
                    .Select((a, b, c, d) => new FlowBeforeListOutput()
                    {
                        enCode = a.EnCode,
                        creatorUserId = a.CreatorUserId,
                        creatorTime = SqlFunc.IsNullOrEmpty(SqlFunc.ToString(b.Description)) ? b.CreatorTime : SqlFunc.ToDate(SqlFunc.ToString(b.Description)),
                        thisStep = a.ThisStep,
                        thisStepId = b.TaskNodeId,
                        flowCategory = a.FlowCategory,
                        fullName = a.FullName,
                        flowName = a.FlowName,
                        status = a.Status,
                        id = b.Id,
                        userName = SqlFunc.MergeString(c.RealName, "/", c.Account),
                        description = SqlFunc.ToString(a.Description),
                        flowCode = a.FlowCode,
                        flowId = a.FlowId,
                        processId = a.ProcessId,
                        formType = d.FormType,
                        flowUrgent = a.FlowUrgent,
                        startTime = a.CreatorTime,
                        completion = a.Completion,
                        nodeName = b.NodeName
                    });
                //委托审核
                var list2 = _flowTaskRepository.AsSugarClient().Queryable<FlowTaskEntity, FlowTaskOperatorEntity, FlowDelegateEntity, UserEntity, UserEntity, FlowEngineEntity>((a, b, c, d, e, f) =>
                new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, a.FlowId == c.FlowId
                && c.EndTime > DateTime.Now, JoinType.Left, c.CreatorUserId == d.Id, JoinType.Left,
                a.CreatorUserId == e.Id, JoinType.Left, a.FlowId == f.Id))
                    .Where((a, b, c) => a.Status == 1 && a.DeleteMark == null && b.Completion == 0 && b.State == "0"
                    && b.HandleId == c.CreatorUserId && (SqlFunc.ToDate(SqlFunc.ToString(b.Description)) < DateTime.Now || b.Description == null)
                    && c.ToUserId == _userManager.UserId && c.DeleteMark == null && c.EnabledMark == 1 && c.EndTime > DateTime.Now && c.StartTime < DateTime.Now).Select((a, b, c, d, e, f) => new FlowBeforeListOutput()
                    {
                        enCode = a.EnCode,
                        creatorUserId = a.CreatorUserId,
                        creatorTime = SqlFunc.IsNullOrEmpty(SqlFunc.ToString(b.Description)) ? b.CreatorTime : SqlFunc.ToDate(SqlFunc.ToString(b.Description)),
                        thisStep = a.ThisStep,
                        thisStepId = b.TaskNodeId,
                        flowCategory = a.FlowCategory,
                        fullName = SqlFunc.MergeString(a.FullName, "(", d.RealName, "的委托)"),
                        flowName = a.FlowName,
                        status = a.Status,
                        id = b.Id,
                        userName = SqlFunc.MergeString(e.RealName, "/", e.Account),
                        description = SqlFunc.ToString(a.Description),
                        flowCode = a.FlowCode,
                        flowId = a.FlowId,
                        processId = a.ProcessId,
                        formType = f.FormType,
                        flowUrgent = a.FlowUrgent,
                        startTime = a.CreatorTime,
                        completion = a.Completion,
                        nodeName = b.NodeName
                    });
                var output = await _flowTaskRepository.AsSugarClient().UnionAll(list1, list2).Where(whereLambda).MergeTable().OrderBy(x => x.creatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<FlowBeforeListOutput>.SqlSugarPageResult(output);
            }
        }

        /// <summary>
        /// 列表（批量待我审批）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        public async Task<dynamic> GetBatchWaitList(FlowBeforeListQuery input)
        {
            if (_flowTaskRepository.AsSugarClient().CurrentConnectionConfig.DbType == DbType.Oracle)
            {
                return await GetWaitList_Oracle(input);
            }
            else
            {
                var whereLambda = LinqExpression.And<FlowBeforeListOutput>();
                if (!input.startTime.IsNullOrEmpty() && !input.endTime.IsNullOrEmpty())
                {
                    var start = Ext.GetDateTime(input.startTime.ToString());
                    var end = Ext.GetDateTime(input.endTime.ToString());
                    start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                    end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                    whereLambda = whereLambda.And(x => SqlFunc.Between(x.creatorTime, start, end));
                }
                if (!input.flowCategory.IsNullOrEmpty())
                    whereLambda = whereLambda.And(x => x.flowCategory == input.flowCategory);
                if (!input.flowId.IsNullOrEmpty())
                    whereLambda = whereLambda.And(x => x.flowId == input.flowId);
                if (!input.keyword.IsNullOrEmpty())
                    whereLambda = whereLambda.And(m => m.enCode.Contains(input.keyword) || m.fullName.Contains(input.keyword));
                if (!input.creatorUserId.IsNullOrEmpty())
                    whereLambda = whereLambda.And(m => m.creatorUserId.Contains(input.creatorUserId));
                if (!input.nodeCode.IsNullOrEmpty())
                    whereLambda = whereLambda.And(m => m.nodeCode.Contains(input.nodeCode));
                //经办审核
                var list1 = _flowTaskRepository.AsSugarClient().Queryable<FlowTaskEntity, FlowTaskOperatorEntity, UserEntity, FlowEngineEntity, FlowTaskNodeEntity>((a, b, c, d, e) =>
                 new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, a.CreatorUserId == c.Id, JoinType.Left, a.FlowId == d.Id, JoinType.Left, b.TaskNodeId == e.Id))
                    .Where((a, b, c) => a.Status == 1 && a.DeleteMark == null && b.Completion == 0 && b.State == "0"
                    && (SqlFunc.ToDate(SqlFunc.ToString(b.Description)) < DateTime.Now || b.Description == null)
                    && b.HandleId == _userManager.UserId && a.IsBatch == 1)
                    .Select((a, b, c, d, e) => new FlowBeforeListOutput()
                    {
                        enCode = a.EnCode,
                        creatorUserId = a.CreatorUserId,
                        creatorTime = SqlFunc.IsNullOrEmpty(SqlFunc.ToString(b.Description)) ? b.CreatorTime : SqlFunc.ToDate(SqlFunc.ToString(b.Description)),
                        thisStep = a.ThisStep,
                        thisStepId = b.TaskNodeId,
                        flowCategory = a.FlowCategory,
                        fullName = a.FullName,
                        flowName = a.FlowName,
                        status = a.Status,
                        id = b.Id,
                        userName = SqlFunc.MergeString(c.RealName, "/", c.Account),
                        description = SqlFunc.ToString(a.Description),
                        flowCode = a.FlowCode,
                        flowId = a.FlowId,
                        processId = a.ProcessId,
                        formType = d.FormType,
                        flowUrgent = a.FlowUrgent,
                        startTime = a.CreatorTime,
                        completion = a.Completion,
                        nodeName = b.NodeName,
                        approversProperties = e.NodePropertyJson,
                        nodeCode = b.NodeCode
                    });
                //委托审核
                var list2 = _flowTaskRepository.AsSugarClient().Queryable<FlowTaskEntity, FlowTaskOperatorEntity, FlowDelegateEntity, UserEntity, UserEntity, FlowEngineEntity, FlowTaskNodeEntity>((a, b, c, d, e, f, g) =>
                 new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, a.FlowId == c.FlowId
                 && c.EndTime > DateTime.Now, JoinType.Left, c.CreatorUserId == d.Id, JoinType.Left,
                 a.CreatorUserId == e.Id, JoinType.Left, a.FlowId == f.Id, JoinType.Left, b.TaskNodeId == g.Id))
                    .Where((a, b, c) => a.Status == 1 && a.DeleteMark == null && b.Completion == 0 && b.State == "0" && a.IsBatch == 1
                    && b.HandleId == c.CreatorUserId && (SqlFunc.ToDate(SqlFunc.ToString(b.Description)) < DateTime.Now || b.Description == null)
                    && c.ToUserId == _userManager.UserId && c.DeleteMark == null && c.EnabledMark == 1 && c.EndTime > DateTime.Now && c.StartTime < DateTime.Now).Select((a, b, c, d, e, f, g) => new FlowBeforeListOutput()
                    {
                        enCode = a.EnCode,
                        creatorUserId = a.CreatorUserId,
                        creatorTime = SqlFunc.IsNullOrEmpty(SqlFunc.ToString(b.Description)) ? b.CreatorTime : SqlFunc.ToDate(SqlFunc.ToString(b.Description)),
                        thisStep = a.ThisStep,
                        thisStepId = b.TaskNodeId,
                        flowCategory = a.FlowCategory,
                        fullName = SqlFunc.MergeString(a.FullName, "(", d.RealName, "的委托)"),
                        flowName = a.FlowName,
                        status = a.Status,
                        id = b.Id,
                        userName = SqlFunc.MergeString(e.RealName, "/", e.Account),
                        description = SqlFunc.ToString(a.Description),
                        flowCode = a.FlowCode,
                        flowId = a.FlowId,
                        processId = a.ProcessId,
                        formType = f.FormType,
                        flowUrgent = a.FlowUrgent,
                        startTime = a.CreatorTime,
                        completion = a.Completion,
                        nodeName = b.NodeName,
                        approversProperties = g.NodePropertyJson,
                        nodeCode = b.NodeCode
                    });
                var output = await _flowTaskRepository.AsSugarClient().UnionAll(list1, list2).Where(whereLambda).MergeTable().OrderBy(x => x.creatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<FlowBeforeListOutput>.SqlSugarPageResult(output);
            }
        }

        /// <summary>
        /// oracle
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<object> GetWaitList_Oracle(FlowBeforeListQuery input)
        {
            var whereLambda = LinqExpression.And<FlowBeforeListOutput>();
            if (!input.startTime.IsNullOrEmpty() && !input.endTime.IsNullOrEmpty())
            {
                var start = Ext.GetDateTime(input.startTime.ToString());
                var end = Ext.GetDateTime(input.endTime.ToString());
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.creatorTime, start, end));
            }
            if (!input.flowCategory.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowCategory == input.flowCategory);
            if (!input.flowId.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowId == input.flowId);
            if (!input.keyword.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.enCode.Contains(input.keyword) || m.fullName.Contains(input.keyword));
            if (!input.creatorUserId.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.creatorUserId.Contains(input.creatorUserId));
            //经办审核
            var list1 = _flowTaskRepository.AsSugarClient().Queryable<FlowTaskEntity, FlowTaskOperatorEntity, UserEntity, FlowEngineEntity>((a, b, c, d) =>
            new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, a.CreatorUserId == c.Id, JoinType.Left, a.FlowId == d.Id))
                .Where((a, b, c) => a.Status == 1 && a.DeleteMark == null && b.Completion == 0 && b.State == "0"
                && (SqlFunc.Oracle_ToDate(SqlFunc.ToString(b.Description), "yyyy-mm-dd hh:mi:ss") < SqlFunc.GetDate() || b.Description == null)
                && b.HandleId == _userManager.UserId)
                .Select((a, b, c, d) => new FlowBeforeListOutput()
                {
                    enCode = a.EnCode,
                    creatorUserId = a.CreatorUserId,
                    creatorTime = SqlFunc.IsNullOrEmpty(SqlFunc.ToString(b.Description)) ? b.CreatorTime : SqlFunc.Oracle_ToDate(SqlFunc.ToString(b.Description), "yyyy-mm-dd hh24:mi:ss"),
                    thisStep = a.ThisStep,
                    thisStepId = b.TaskNodeId,
                    flowCategory = a.FlowCategory,
                    fullName = a.FullName,
                    flowName = a.FlowName,
                    status = a.Status,
                    id = b.Id,
                    userName = SqlFunc.MergeString(c.RealName, "/", c.Account),
                    description = SqlFunc.ToString(a.Description),
                    flowCode = a.FlowCode,
                    flowId = a.FlowId,
                    processId = a.ProcessId,
                    formType = d.FormType,
                    flowUrgent = a.FlowUrgent,
                    startTime = a.CreatorTime,
                    completion = a.Completion,
                    nodeName = b.NodeName
                });
            //委托审核
            var list2 = _flowTaskRepository.AsSugarClient().Queryable<FlowTaskEntity, FlowTaskOperatorEntity, FlowDelegateEntity, UserEntity, UserEntity, FlowEngineEntity>((a, b, c, d, e, f) =>
            new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, a.FlowId == c.FlowId
            && c.EndTime > SqlFunc.GetDate(), JoinType.Left, c.CreatorUserId == d.Id, JoinType.Left,
            a.CreatorUserId == e.Id, JoinType.Left, a.FlowId == f.Id))
                .Where((a, b, c) => a.Status == 1 && a.DeleteMark == null && b.Completion == 0 && b.State == "0"
                && b.HandleId == c.CreatorUserId && (SqlFunc.Oracle_ToDate(SqlFunc.ToString(b.Description), "yyyy-mm-dd hh24:mi:ss") < SqlFunc.GetDate() || b.Description == null)
                && c.ToUserId == _userManager.UserId && c.DeleteMark == null && c.EnabledMark == 1 && c.EndTime > DateTime.Now && c.StartTime < DateTime.Now).Select((a, b, c, d, e, f) => new FlowBeforeListOutput()
                {
                    enCode = a.EnCode,
                    creatorUserId = a.CreatorUserId,
                    creatorTime = SqlFunc.IsNullOrEmpty(SqlFunc.ToString(b.Description)) ? b.CreatorTime : SqlFunc.Oracle_ToDate(SqlFunc.ToString(b.Description), "yyyy-mm-dd hh24:mi:ss"),
                    thisStep = a.ThisStep,
                    thisStepId = b.TaskNodeId,
                    flowCategory = a.FlowCategory,
                    fullName = SqlFunc.MergeString(a.FullName, "(", d.RealName, "的委托)"),
                    flowName = a.FlowName,
                    status = a.Status,
                    id = b.Id,
                    userName = SqlFunc.MergeString(e.RealName, "/", e.Account),
                    description = SqlFunc.ToString(a.Description),
                    flowCode = a.FlowCode,
                    flowId = a.FlowId,
                    processId = a.ProcessId,
                    formType = f.FormType,
                    flowUrgent = a.FlowUrgent,
                    startTime = a.CreatorTime,
                    completion = a.Completion,
                    nodeName = b.NodeName
                });
            var output = await _flowTaskRepository.AsSugarClient().UnionAll(list1, list2).Where(whereLambda).MergeTable().OrderBy(x => x.creatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<FlowBeforeListOutput>.SqlSugarPageResult(output);
        }

        /// <summary>
        /// 列表（我已审批）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        public async Task<dynamic> GetTrialList(FlowBeforeListQuery input)
        {
            var list = _flowTaskRepository.AsSugarClient().Queryable<FlowTaskEntity, FlowTaskOperatorRecordEntity, FlowTaskOperatorEntity,
                UserEntity, UserEntity, FlowEngineEntity>((a, b, c, d, e, f) =>
                new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, b.TaskOperatorId == c.Id,
                JoinType.Left, c.HandleId == d.Id, JoinType.Left, a.CreatorUserId == e.Id,
                JoinType.Left, a.FlowId == f.Id)).Where((a, b, c) => b.HandleStatus < 2 && b.TaskOperatorId != null
                && b.HandleId == _userManager.UserId && b.Status >= 0).Select((a, b, c, d, e, f) => new FlowBeforeListOutput()
                {
                    enCode = a.EnCode,
                    creatorUserId = a.CreatorUserId,
                    creatorTime = b.HandleTime,
                    thisStep = a.ThisStep,
                    thisStepId = b.TaskNodeId,
                    flowCategory = a.FlowCategory,
                    fullName = b.HandleId == c.HandleId || c.Id == null ? a.FullName : SqlFunc.MergeString(a.FullName, "(", d.RealName, "的委托)"),
                    flowName = a.FlowName,
                    status = b.HandleStatus,
                    id = b.Id,
                    userName = SqlFunc.MergeString(e.RealName, "/", e.Account),
                    description = a.Description,
                    flowCode = a.FlowCode,
                    flowId = a.FlowId,
                    processId = a.ProcessId,
                    formType = f.FormType,
                    flowUrgent = a.FlowUrgent,
                    startTime = a.CreatorTime,
                });
            var whereLambda = LinqExpression.And<FlowBeforeListOutput>();
            if (!input.startTime.IsNullOrEmpty() && !input.endTime.IsNullOrEmpty())
            {
                var start = Ext.GetDateTime(input.startTime.ToString());
                var end = Ext.GetDateTime(input.endTime.ToString());
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.creatorTime, start, end));
            }
            if (!input.flowCategory.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowCategory == input.flowCategory);
            if (!input.flowId.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowId == input.flowId);
            if (!input.creatorUserId.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.creatorUserId.Contains(input.creatorUserId));
            if (!input.keyword.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.enCode.Contains(input.keyword) || m.fullName.Contains(input.keyword));
            var output = await list.MergeTable().Where(whereLambda).OrderBy(x => x.creatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<FlowBeforeListOutput>.SqlSugarPageResult(output);
        }

        /// <summary>
        /// 列表（抄送我的）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        public async Task<dynamic> GetCirculateList(FlowBeforeListQuery input)
        {
            var whereLambda = LinqExpression.And<FlowBeforeListOutput>();
            if (!input.startTime.IsNullOrEmpty() && !input.endTime.IsNullOrEmpty())
            {
                var start = Ext.GetDateTime(input.startTime.ToString());
                var end = Ext.GetDateTime(input.endTime.ToString());
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.creatorTime, start, end));
            }
            if (!input.flowCategory.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowCategory == input.flowCategory);
            if (!input.flowId.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowId == input.flowId);
            if (!input.creatorUserId.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.creatorUserId.Contains(input.creatorUserId));
            if (!input.keyword.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.enCode.Contains(input.keyword) || m.fullName.Contains(input.keyword));
            var list = await _flowTaskRepository.AsSugarClient().Queryable<FlowTaskEntity, FlowTaskCirculateEntity, UserEntity, FlowEngineEntity>((a, b, c, d) => new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, a.CreatorUserId == c.Id, JoinType.Left, a.FlowId == d.Id)).Where((a, b) => b.ObjectId == _userManager.UserId).Select((a, b, c, d) => new FlowBeforeListOutput()
            {
                enCode = a.EnCode,
                creatorUserId = a.CreatorUserId,
                creatorTime = b.CreatorTime,
                thisStep = a.ThisStep,
                thisStepId = b.TaskNodeId,
                flowCategory = a.FlowCategory,
                fullName = a.FullName,
                flowName = a.FlowName,
                status = a.Status,
                id = b.Id,
                userName = SqlFunc.MergeString(c.RealName, "/", c.Account),
                description = a.Description,
                flowCode = a.FlowCode,
                flowId = a.FlowId,
                processId = a.ProcessId,
                formType = d.FormType,
                flowUrgent = a.FlowUrgent,
                startTime = a.CreatorTime,
            }).MergeTable().Where(whereLambda).OrderBy(x => x.creatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<FlowBeforeListOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 列表（待我审批）
        /// </summary>
        /// <returns></returns>
        public async Task<List<FlowTaskEntity>> GetWaitList()
        {
            //经办审核
            var list1 = _flowTaskRepository.AsSugarClient().Queryable<FlowTaskEntity, FlowTaskOperatorEntity>((a, b) =>
            new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId)).Where((a, b) => a.Status == 1 && a.DeleteMark == null
            && b.Completion == 0 && b.State == "0" && (SqlFunc.ToDate(SqlFunc.ToString(b.Description)) < DateTime.Now || b.Description == null)
            && b.HandleId == _userManager.UserId).Select((a, b) => new FlowTaskEntity()
            {
                Id = b.Id,
                ParentId = a.ParentId,
                ProcessId = a.ProcessId,
                EnCode = a.EnCode,
                FullName = a.FullName,
                FlowUrgent = a.FlowUrgent,
                FlowId = a.FlowId,
                FlowCode = a.FlowCode,
                FlowName = a.FlowName,
                FlowCategory = a.FlowCategory,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                ThisStep = a.ThisStep,
                ThisStepId = b.TaskNodeId,
                Status = a.Status,
                Completion = a.Completion,
                CreatorTime = b.CreatorTime,
                CreatorUserId = a.CreatorUserId,
                LastModifyTime = a.LastModifyTime,
                LastModifyUserId = a.LastModifyUserId
            });
            //委托审核
            var list2 = _flowTaskRepository.AsSugarClient().Queryable<FlowTaskEntity, FlowTaskOperatorEntity, FlowDelegateEntity, UserEntity>((a, b, c, d) =>
            new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, a.FlowId == c.FlowId &&
            c.EndTime > DateTime.Now, JoinType.Left, c.CreatorUserId == d.Id)).Where((a, b, c) =>
            a.Status == 1 && a.DeleteMark == null && b.Completion == 0 && b.State == "0"
            && (SqlFunc.ToDate(SqlFunc.ToString(b.Description)) < DateTime.Now || b.Description == null)
            && b.HandleId == c.CreatorUserId && c.ToUserId == _userManager.UserId && c.DeleteMark == null && c.EndTime > DateTime.Now && c.StartTime < DateTime.Now).Select((a, b, c, d) => new FlowTaskEntity()
            {
                Id = b.Id,
                ParentId = a.ParentId,
                ProcessId = a.ProcessId,
                EnCode = a.EnCode,
                FullName = SqlFunc.MergeString(a.FullName, "(", d.RealName, "的委托)"),
                FlowUrgent = a.FlowUrgent,
                FlowId = a.FlowId,
                FlowName = a.FlowName,
                FlowCode = a.FlowCode,
                FlowCategory = a.FlowCategory,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                ThisStep = a.ThisStep,
                ThisStepId = b.TaskNodeId,
                Status = a.Status,
                Completion = a.Completion,
                CreatorTime = b.CreatorTime,
                CreatorUserId = a.CreatorUserId,
                LastModifyTime = a.LastModifyTime,
                LastModifyUserId = a.LastModifyUserId
            });
            var output = await _flowTaskRepository.AsSugarClient().UnionAll(list1, list2).MergeTable().ToListAsync();
            return output;
        }

        /// <summary>
        /// 列表（待我审批）
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetPortalWaitList()
        {
            //经办审核
            var list1 = _flowTaskRepository.AsSugarClient().Queryable<FlowTaskEntity, FlowTaskOperatorEntity, FlowEngineEntity>((a, b, c) =>
             new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, a.FlowId == c.Id)).Where((a, b) => a.Status == 1 && a.DeleteMark == null
             && b.Completion == 0 && b.State == "0" && (SqlFunc.ToDate(SqlFunc.ToString(b.Description)) < DateTime.Now || b.Description == null)
             && b.HandleId == _userManager.UserId).Select((a, b, c) => new PortalWaitListModel()
             {
                 id = b.Id,
                 fullName = a.FullName,
                 enCode = c.EnCode,
                 flowId = c.Id,
                 formType = c.FormType,
                 status = a.Status,
                 processId = a.Id,
                 taskNodeId = b.TaskNodeId,
                 taskOperatorId = b.Id,
                 creatorTime = b.CreatorTime,
                 type = 2
             });
            //委托审核
            var list2 = _flowTaskRepository.AsSugarClient().Queryable<FlowTaskEntity, FlowTaskOperatorEntity, FlowDelegateEntity, UserEntity, FlowEngineEntity>((a, b, c, d, e) =>
            new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, a.FlowId == c.FlowId &&
            c.EndTime > DateTime.Now, JoinType.Left, c.CreatorUserId == d.Id, JoinType.Left, a.FlowId == c.Id)).Where((a, b, c) =>
             a.Status == 1 && a.DeleteMark == null && b.Completion == 0 && b.State == "0"
             && (SqlFunc.ToDate(SqlFunc.ToString(b.Description)) < DateTime.Now || b.Description == null)
             && b.HandleId == c.CreatorUserId && c.ToUserId == _userManager.UserId && c.DeleteMark == null && c.EndTime > DateTime.Now && c.StartTime < DateTime.Now).Select((a, b, c, d, e) => new PortalWaitListModel()
             {
                 id = b.Id,
                 fullName = SqlFunc.MergeString(a.FullName, "(", d.RealName, "的委托)"),
                 enCode = e.EnCode,
                 flowId = e.Id,
                 formType = e.FormType,
                 status = a.Status,
                 processId = a.Id,
                 taskNodeId = b.TaskNodeId,
                 taskOperatorId = b.Id,
                 creatorTime = b.CreatorTime,
                 type = 2
             });
            var output = await _flowTaskRepository.AsSugarClient().UnionAll(list1, list2).MergeTable().ToListAsync();
            return output;
        }

        /// <summary>
        /// 列表（我已审批）
        /// </summary>
        /// <returns></returns>
        public async Task<List<FlowTaskEntity>> GetTrialList()
        {
            var list = await _flowTaskRepository.AsSugarClient().Queryable<FlowTaskEntity, FlowTaskOperatorRecordEntity, FlowTaskOperatorEntity, UserEntity>((a, b, c, d) => new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, b.TaskOperatorId == c.Id, JoinType.Left, c.HandleId == d.Id)).Where((a, b, c) => b.HandleStatus < 2 && b.TaskOperatorId != null && b.HandleId == _userManager.UserId && b.Status >= 0).Select((a, b, c, d) => new FlowTaskEntity()
            {
                Id = b.Id,
                ParentId = a.ParentId,
                ProcessId = a.ProcessId,
                EnCode = a.EnCode,
                FullName = b.HandleId == c.HandleId || c.Id == null ? a.FullName : SqlFunc.MergeString(a.FullName, "(", d.RealName, "的委托)"),
                FlowUrgent = a.FlowUrgent,
                FlowId = a.FlowId,
                FlowCode = a.FlowCode,
                FlowName = a.FlowName,
                FlowCategory = a.FlowCategory,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                ThisStep = b.NodeName,
                ThisStepId = c.TaskNodeId,
                Status = b.HandleStatus,
                Completion = a.Completion,
                CreatorTime = b.HandleTime,
                CreatorUserId = a.CreatorUserId,
                LastModifyTime = a.LastModifyTime,
                LastModifyUserId = a.LastModifyUserId
            }).MergeTable().ToListAsync();
            return list;
        }

        /// <summary>
        /// 批量流程列表
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> BatchFlowSelector()
        {
            var list = await _flowTaskRepository.AsSugarClient()
                .Queryable<FlowTaskEntity, FlowTaskOperatorEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId))
                .Where((a, b) => a.DeleteMark == null && a.IsBatch == 1 && b.HandleId == _userManager.UserId && b.Completion == 0 && a.Status == 1 && b.State == "0")
                .GroupBy((a, b) => new { a.FlowId })
                .Select((a, b) => new
                {
                    id = a.FlowId,
                    fullName = SqlFunc.MergeString(SqlFunc.Subqueryable<FlowEngineEntity>().Where(x => x.Id == a.FlowId).Select(x => x.FullName), "(", SqlFunc.AggregateCount(b.Id).ToString(), ")"),
                    count = SqlFunc.AggregateCount(b.Id)
                }).MergeTable().OrderBy(x => x.count, OrderByType.Desc)
                .ToListAsync();
            return list;
        }

        /// <summary>
        /// 任务列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<FlowTaskEntity>> GetTaskList()
        {
            return await _flowTaskRepository.AsQueryable().Where(x => x.DeleteMark == null).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToListAsync();
        }

        /// <summary>
        /// 任务列表
        /// </summary>
        /// <param name="flowId">引擎id</param>
        /// <returns></returns>
        public async Task<List<FlowTaskEntity>> GetTaskList(string flowId)
        {
            return await _flowTaskRepository.AsQueryable().Where(x => x.DeleteMark == null && x.FlowId == flowId).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToListAsync();
        }

        /// <summary>
        /// 任务列表
        /// </summary>
        /// <param name="ids">id</param>
        /// <returns></returns>
        public async Task<List<FlowTaskEntity>> GetTaskList(List<string> ids)
        {
            return await _flowTaskRepository.AsQueryable().Where(x => x.DeleteMark == null).In(x => x.Id, ids.ToArray()).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToListAsync();
        }

        /// <summary>
        /// 是否存在任务
        /// </summary>
        /// <param name="expression">id</param>
        /// <returns></returns>
        public async Task<bool> AnyFlowTask(Expression<Func<FlowTaskEntity, bool>> expression)
        {
            return await _flowTaskRepository.AsQueryable().AnyAsync(expression);
        }

        /// <summary>
        /// 任务列表
        /// </summary>
        /// <param name="expression">id</param>
        /// <returns></returns>
        public async Task<List<FlowTaskEntity>> GetTaskList(Expression<Func<FlowTaskEntity, bool>> expression)
        {
            return await _flowTaskRepository.AsQueryable().Where(expression).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToListAsync();
        }

        /// <summary>
        /// 任务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FlowTaskEntity> GetTaskInfo(string id)
        {
            return await _flowTaskRepository.GetFirstAsync(x => x.DeleteMark == null && x.Id == id);
        }

        /// <summary>
        /// 任务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FlowTaskEntity GetTaskInfoNoAsync(string id)
        {
            return _flowTaskRepository.GetFirst(x => x.DeleteMark == null && x.Id == id);
        }

        /// <summary>
        /// 任务删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> DeleteTask(FlowTaskEntity entity)
        {
            await _flowTaskNodeRepository.DeleteAsync(x => entity.Id == x.TaskId);
            await _flowTaskOperatorRepository.DeleteAsync(x => entity.Id == x.TaskId);
            await _flowTaskOperatorRecordRepository.DeleteAsync(x => entity.Id == x.TaskId);
            await _flowTaskCirculateRepository.DeleteAsync(x => entity.Id == x.TaskId);
            return await _flowTaskRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 任务删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int DeleteTaskNoAsync(FlowTaskEntity entity)
        {
            _flowTaskNodeRepository.Delete(x => entity.Id == x.TaskId);
            _flowTaskOperatorRepository.Delete(x => entity.Id == x.TaskId);
            _flowTaskOperatorRecordRepository.Delete(x => entity.Id == x.TaskId);
            _flowTaskCirculateRepository.Delete(x => entity.Id == x.TaskId);
            return _flowTaskRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommand();
        }

        /// <summary>
        /// 任务创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<FlowTaskEntity> CreateTask(FlowTaskEntity entity)
        {
            return await _flowTaskRepository.AsInsertable(entity).CallEntityMethod(m => m.Create()).ExecuteReturnEntityAsync();
        }

        /// <summary>
        /// 任务更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdateTask(FlowTaskEntity entity)
        {
            return await _flowTaskRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 判断是否有子流程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool AnySubFlowTask(string id)
        {
            return _flowTaskRepository.IsAny(x => x.ParentId == id && x.Status == 0 && x.DeleteMark == null);
        }

        /// <summary>
        /// 节点列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<List<FlowTaskNodeEntity>> GetTaskNodeList(string taskId)
        {
            return await _flowTaskNodeRepository.AsQueryable().Where(x => x.TaskId == taskId).OrderBy(x => x.SortCode).ToListAsync();
        }

        /// <summary>
        /// 节点信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FlowTaskNodeEntity> GetTaskNodeInfo(string id)
        {
            return await _flowTaskNodeRepository.GetFirstAsync(x => x.Id == id);
        }

        /// <summary>
        /// 节点删除
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<int> DeleteTaskNode(string taskId)
        {
            return await _flowTaskNodeRepository.DeleteAsync(x => x.TaskId == taskId) ? 0 : 1;
        }

        /// <summary>
        /// 节点创建
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public async Task<int> CreateTaskNode(List<FlowTaskNodeEntity> entitys)
        {
            return await _flowTaskNodeRepository.AsSugarClient().Insertable(entitys).ExecuteCommandAsync();
        }

        /// <summary>
        /// 节点更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdateTaskNode(FlowTaskNodeEntity entity)
        {
            return await _flowTaskNodeRepository.UpdateAsync(entity) ? 1 : 0;
        }

        /// <summary>
        /// 节点更新
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public async Task<int> UpdateTaskNode(List<FlowTaskNodeEntity> entitys)
        {
            return await _flowTaskNodeRepository.UpdateRangeAsync(entitys) ? 1 : 0;
        }

        /// <summary>
        /// 经办列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<List<FlowTaskOperatorEntity>> GetTaskOperatorList(string taskId)
        {
            return await _flowTaskOperatorRepository.AsQueryable().Where(x => x.TaskId == taskId).OrderBy(x => x.CreatorTime).ToListAsync();
        }

        /// <summary>
        /// 经办列表
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<List<FlowTaskOperatorEntity>> GetTaskOperatorList(Expression<Func<FlowTaskOperatorEntity, bool>> expression)
        {
            return await _flowTaskOperatorRepository.AsQueryable().Where(expression).OrderBy(x => x.CreatorTime).ToListAsync();
        }

        /// <summary>
        /// 经办列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskNodeId"></param>
        /// <returns></returns>
        public async Task<List<FlowTaskOperatorEntity>> GetTaskOperatorList(string taskId, string taskNodeId)
        {
            return await _flowTaskOperatorRepository.AsQueryable().Where(x => x.TaskId == taskId && x.TaskNodeId == taskNodeId).OrderBy(x => x.CreatorTime).ToListAsync();
        }

        /// <summary>
        /// 经办信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FlowTaskOperatorEntity> GetTaskOperatorInfo(string id)
        {
            return await _flowTaskOperatorRepository.GetFirstAsync(x => x.Id == id);
        }

        /// <summary>
        /// 经办删除
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<int> DeleteTaskOperator(string taskId)
        {
            return await _flowTaskOperatorRepository.DeleteAsync(x => x.TaskId == taskId) ? 1 : 0;
        }

        /// <summary>
        /// 经办删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<int> DeleteTaskOperator(List<string> ids)
        {
            return await _flowTaskOperatorRepository.AsSugarClient().Updateable<FlowTaskOperatorEntity>().SetColumns(x => x.State == "-1").Where(x => ids.Contains(x.Id)).ExecuteCommandAsync();
        }

        /// <summary>
        /// 经办创建
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public async Task<int> CreateTaskOperator(List<FlowTaskOperatorEntity> entitys)
        {
            return await _flowTaskOperatorRepository.AsInsertable(entitys).ExecuteCommandAsync();
        }

        /// <summary>
        /// 经办创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> CreateTaskOperator(FlowTaskOperatorEntity entity)
        {
            return await _flowTaskOperatorRepository.InsertAsync(entity) ? 1 : 0;
        }

        /// <summary>
        /// 经办更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdateTaskOperator(FlowTaskOperatorEntity entity)
        {
            return await _flowTaskOperatorRepository.UpdateAsync(entity) ? 1 : 0;
        }

        /// <summary>
        /// 经办更新
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public async Task<int> UpdateTaskOperator(List<FlowTaskOperatorEntity> entitys)
        {
            return await _flowTaskOperatorRepository.UpdateRangeAsync(entitys) ? 1 : 0;
        }

        /// <summary>
        /// 经办记录列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<List<FlowTaskOperatorRecordEntity>> GetTaskOperatorRecordList(string taskId)
        {
            return await _flowTaskOperatorRecordRepository.AsQueryable().Where(x => x.TaskId == taskId).OrderBy(o => o.HandleTime).ToListAsync();
        }

        /// <summary>
        /// 经办信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FlowTaskOperatorRecordEntity> GetTaskOperatorRecordInfo(string id)
        {
            return await _flowTaskOperatorRecordRepository.GetFirstAsync(x => x.Id == id);
        }

        /// <summary>
        /// 经办信息
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskNodeId"></param>
        /// <param name="taskOperatorId"></param>
        /// <returns></returns>
        public async Task<FlowTaskOperatorRecordEntity> GetTaskOperatorRecordInfo(string taskId, string taskNodeId, string taskOperatorId)
        {
            return await _flowTaskOperatorRecordRepository.GetFirstAsync(x => x.TaskId == taskId && x.TaskNodeId == taskNodeId && x.TaskOperatorId == taskOperatorId && x.Status != -1 && x.HandleStatus < 2);
        }

        /// <summary>
        /// 经办创建
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public async Task<int> CreateTaskOperatorRecord(List<FlowTaskOperatorRecordEntity> entitys)
        {
            return await _flowTaskOperatorRecordRepository.AsInsertable(entitys).ExecuteCommandAsync();
        }

        /// <summary>
        /// 经办创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> CreateTaskOperatorRecord(FlowTaskOperatorRecordEntity entity)
        {
            entity.Id = YitIdHelper.NextId().ToString();
            return await _flowTaskOperatorRecordRepository.InsertAsync(entity) ? 1 : 0;
        }

        /// <summary>
        /// 传阅删除
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<int> DeleteTaskCirculate(string taskId)
        {
            return await _flowTaskCirculateRepository.DeleteAsync(x => x.TaskId == taskId) ? 1 : 0;
        }

        /// <summary>
        /// 传阅创建
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public async Task<int> CreateTaskCirculate(List<FlowTaskCirculateEntity> entitys)
        {
            return await _flowTaskCirculateRepository.AsInsertable(entitys).ExecuteCommandAsync();
        }

        /// <summary>
        /// 打回流程删除所有相关数据
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task DeleteFlowTaskAllData(string taskId)
        {
            try
            {
                Db.BeginTran();
                await _flowTaskNodeRepository.AsSugarClient().Updateable<FlowTaskNodeEntity>().SetColumns(x => x.State == "-2").Where(x => x.TaskId == taskId).ExecuteCommandAsync();
                await _flowTaskOperatorRepository.AsSugarClient().Updateable<FlowTaskOperatorEntity>().SetColumns(x => x.State == "-1").Where(x => x.TaskId == taskId).ExecuteCommandAsync();
                await _flowTaskOperatorRecordRepository.AsSugarClient().Updateable<FlowTaskOperatorRecordEntity>().SetColumns(x => x.Status == -1).Where(x => x.TaskId == taskId).ExecuteCommandAsync();
                await _flowCandidatesRepository.DeleteAsync(x => x.TaskId == taskId);
                Db.CommitTran();
            }
            catch (Exception)
            {

                Db.RollbackTran();
            }
        }

        /// <summary>
        /// 获取允许删除任务列表
        /// </summary>
        /// <param name="ids">id数组</param>
        /// <returns></returns>
        public List<string> GetAllowDeleteFlowTaskList(List<string> ids)
        {
            return _flowTaskRepository.AsQueryable().In(f => f.Id, ids.ToArray()).Where(f => f.Status != 0).Where(f => f.Status != 4).Select(f => f.Id).ToList();
        }

        /// <summary>
        /// 经办记录列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskNodeIds"></param>
        /// <returns></returns>
        public List<FlowTaskOperatorRecordEntity> GetTaskOperatorRecordList(string taskId, string[] taskNodeIds)
        {
            return _flowTaskOperatorRecordRepository.AsQueryable().Where(x => x.TaskId == taskId).In(x => x.TaskNodeId, taskNodeIds).OrderBy(x => x.HandleTime).ToList();
        }

        /// <summary>
        /// 经办列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskNodeIds"></param>
        /// <returns></returns>
        public List<FlowTaskOperatorEntity> GetTaskOperatorList(string taskId, string[] taskNodeIds)
        {
            return _flowTaskOperatorRepository.AsQueryable().Where(x => x.TaskId == taskId).In(x => x.TaskNodeId, taskNodeIds).OrderBy(x => x.HandleTime).ToList();
        }

        /// <summary>
        /// 经办记录作废
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task DeleteTaskOperatorRecord(List<string> ids)
        {
            await _flowTaskOperatorRecordRepository.AsSugarClient().Updateable<FlowTaskOperatorRecordEntity>().SetColumns(it => it.Status == -1).Where(x => ids.Contains(x.Id)).ExecuteCommandAsync();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public async Task<FlowTaskOperatorEntity> GetTaskOperatorInfoByParentId(string parentId)
        {
            return await _flowTaskOperatorRepository.GetFirstAsync(x => x.ParentId == parentId && !x.State.Equals("-1"));
        }

        /// <summary>
        /// 根据分类获取审批意见
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<List<FlowBeforeRecordListModel>> GetRecordListByCategory(string taskId, string category, string type = "0")
        {
            var recordList = new List<FlowBeforeRecordListModel>();
            switch (category)
            {
                case "1":
                    return await _flowTaskOperatorRecordRepository.AsSugarClient().Queryable<FlowTaskOperatorRecordEntity, UserEntity, OrganizeEntity>((a, b, c) =>
             new JoinQueryInfos(JoinType.Left, a.HandleId == b.Id, JoinType.Left, SqlFunc.ToString(b.OrganizeId) == c.Id)).Where(a => a.TaskId == taskId)
             .WhereIF(type == "1", (a) => a.HandleStatus == 0 || a.HandleStatus == 1).Select((a, b, c) =>
                           new FlowBeforeRecordListModel()
                           {
                               id = a.Id,
                               handleId = a.Id,
                               handleOpinion = a.HandleOpinion,
                               handleStatus = a.HandleStatus,
                               handleTime = a.HandleTime,
                               userName = SqlFunc.MergeString(b.RealName, "/", b.Account),
                               category = c.Id,
                               categoryName = c.FullName,
                               operatorId = SqlFunc.Subqueryable<UserEntity>().Where(u => u.Id == a.OperatorId).Select(u => SqlFunc.MergeString(u.RealName, "/", u.Account)),
                           }).ToListAsync();
                case "2":
                    return await _flowTaskOperatorRecordRepository.AsSugarClient().Queryable<FlowTaskOperatorRecordEntity, UserEntity, UserRelationEntity, RoleEntity>((a, b, c, d) =>
              new JoinQueryInfos(JoinType.Left, a.HandleId == b.Id, JoinType.Left, b.Id == c.UserId, JoinType.Left, c.ObjectId == d.Id))
                .Where((a, b, c) => a.TaskId == taskId && c.ObjectType == "Role").WhereIF(type == "1", (a) => a.HandleStatus == 0 || a.HandleStatus == 1)
                .Select((a, b, c, d) => new FlowBeforeRecordListModel()
                {
                    id = a.Id,
                    handleId = a.Id,
                    handleOpinion = a.HandleOpinion,
                    handleStatus = a.HandleStatus,
                    handleTime = a.HandleTime,
                    userName = SqlFunc.MergeString(b.RealName, "/", b.Account),
                    category = d.Id,
                    categoryName = d.FullName,
                    operatorId = SqlFunc.Subqueryable<UserEntity>().Where(u => u.Id == a.OperatorId).Select(u => SqlFunc.MergeString(u.RealName, "/", u.Account)),
                }).ToListAsync();
                case "3":
                    return await _flowTaskOperatorRecordRepository.AsSugarClient().Queryable<FlowTaskOperatorRecordEntity, UserEntity, UserRelationEntity, PositionEntity>((a, b, c, d) =>
              new JoinQueryInfos(JoinType.Left, a.HandleId == b.Id, JoinType.Left, b.Id == c.UserId, JoinType.Left, c.ObjectId == d.Id))
                 .Where((a, b, c) => a.TaskId == taskId && c.ObjectType == "Position").WhereIF(type == "1", (a) => a.HandleStatus == 0 || a.HandleStatus == 1)
                 .Select((a, b, c, d) => new FlowBeforeRecordListModel()
                 {
                     id = a.Id,
                     handleId = a.Id,
                     handleOpinion = a.HandleOpinion,
                     handleStatus = a.HandleStatus,
                     handleTime = a.HandleTime,
                     userName = SqlFunc.MergeString(b.RealName, "/", b.Account),
                     category = d.Id,
                     categoryName = d.FullName,
                     operatorId = SqlFunc.Subqueryable<UserEntity>().Where(u => u.Id == a.OperatorId).Select(u => SqlFunc.MergeString(u.RealName, "/", u.Account)),
                 }).ToListAsync();
                default:
                    break;
            }
            return recordList;
        }

        /// <summary>
        /// 候选人创建
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public void CreateFlowCandidates(List<FlowCandidatesEntity> entitys)
        {
            _flowCandidatesRepository.AsInsertable(entitys).ExecuteCommand();
        }

        /// <summary>
        /// 候选人删除
        /// </summary>
        /// <param name="nodeId"></param>
        public void DeleteFlowCandidates(List<string> nodeId)
        {
            _flowCandidatesRepository.Delete(x => nodeId.Contains(x.TaskNodeId));
        }

        /// <summary>
        /// 候选人删除
        /// </summary>
        /// <param name="ids"></param>
        public void DeleteFlowCandidates(string[] ids)
        {
            _flowCandidatesRepository.DeleteByIds(ids);
        }

        /// <summary>
        /// 候选人删除
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="hanldId"></param>
        /// <param name="operatorId"></param>
        public void DeleteFlowCandidates(string nodeId, string hanldId, string operatorId)
        {
            _flowCandidatesRepository.Delete(x => x.TaskNodeId == nodeId && x.HandleId == hanldId && x.TaskOperatorId == operatorId);
        }

        /// <summary>
        /// 候选人获取
        /// </summary>
        /// <param name="nodeId"></param>
        public List<string> GetFlowCandidates(string nodeId)
        {
            var flowCandidates = new List<string>();
            var candidateUserIdList = _flowCandidatesRepository.GetList(x => x.TaskNodeId == nodeId).Select(x => x.Candidates).ToList();
            foreach (var item in candidateUserIdList)
            {
                flowCandidates = flowCandidates.Union(item.Split(",").ToList()).Distinct().ToList();
            }
            return flowCandidates;
        }
    }
}
