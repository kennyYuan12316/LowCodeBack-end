using HSZ.Common.Configuration;
using HSZ.Common.Const;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.LinqBuilder;
using HSZ.System.Interfaces.System;
using HSZ.TaskScheduler.Entitys.Dto.TaskScheduler;
using HSZ.TaskScheduler.Entitys.Entity;
using HSZ.TaskScheduler.Entitys.Enum;
using HSZ.TaskScheduler.Entitys.Model;
using HSZ.TaskScheduler.Interfaces.TaskScheduler;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HSZ.TaskScheduler.TaskScheduler
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：定时任务
    /// </summary>
    [ApiDescriptionSettings(Tag = "TaskScheduler", Name = "scheduletask", Order = 220)]
    [Route("api/[controller]")]
    public class TimeTaskService : ITimeTaskService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<TimeTaskEntity> _timeTaskRepository;
        private readonly ISqlSugarRepository<TimeTaskLogEntity> _timeTaskLogRepository;
        private readonly IDataInterfaceService _dataInterfaceService;
        private readonly IUserManager _userManager;
        private readonly ICacheManager _cacheManager;


        /// <summary>
        /// 初始化一个<see cref="TimeTaskService"/>类型的新实例
        /// </summary>
        public TimeTaskService(ISqlSugarRepository<TimeTaskEntity> timeTaskRepository,
            ISqlSugarRepository<TimeTaskLogEntity> timeTaskLogRepository,
            IUserManager userManager,
            IDataInterfaceService dataInterfaceService,
            ICacheManager cacheManager)
        {
            _timeTaskRepository = timeTaskRepository;
            _timeTaskLogRepository = timeTaskLogRepository;
            _userManager = userManager;
            _dataInterfaceService = dataInterfaceService;
            _cacheManager = cacheManager;
        }

        #region Get

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] PageInputBase input)
        {
            var queryWhere = LinqExpression.And<TimeTaskEntity>().And(x => x.DeleteMark == null);
            if (!string.IsNullOrEmpty(input.keyword))
                queryWhere = queryWhere.And(m => m.FullName.Contains(input.keyword) || m.EnCode.Contains(input.keyword));
            var list = await _timeTaskRepository.AsQueryable().Where(queryWhere).OrderBy(x => x.CreatorTime, OrderByType.Desc)
                .OrderByIF(!string.IsNullOrEmpty(input.keyword), t => t.LastModifyTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<TimeTaskListOutput>()
            {
                list = list.list.Adapt<List<TimeTaskListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<TimeTaskListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 列表（执行记录）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <param name="id">任务Id</param>
        /// <returns></returns>
        [HttpGet("{id}/TaskLog")]
        public async Task<dynamic> GetTaskLogList([FromQuery] TaskLogInput input, string id)
        {
            var whereLambda = LinqExpression.And<TimeTaskLogEntity>().And(x => x.TaskId == id);
            if (input.runResult.IsNotEmptyOrNull())
            {
                whereLambda = whereLambda.And(x => x.RunResult == input.runResult);
            }
            if (!input.startTime.IsNullOrEmpty() && !input.endTime.IsNullOrEmpty())
            {
                var start = Ext.GetDateTime(input.startTime.ToString());
                var end = Ext.GetDateTime(input.endTime.ToString());
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.RunTime, start, end));
            }
            var list = await _timeTaskLogRepository.AsQueryable().Where(whereLambda).OrderBy(x => x.RunTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<TimeTaskTaskLogListOutput>()
            {
                list = list.list.Adapt<List<TimeTaskTaskLogListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<TimeTaskTaskLogListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("Info/{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var data = await GetInfo(id);
            var output = data.Adapt<TimeTaskInfoOutput>();
            return output;
        }

        /// <summary>
        /// 本地方法
        /// </summary>
        /// <returns></returns>
        [HttpGet("TaskMethods")]
        public async Task<dynamic> GetTaskMethodSelector()
        {
            return await GetTaskMethods();
        }
        #endregion

        #region Post

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] TimeTaskCrInput input)
        {
            if (await _timeTaskRepository.IsAnyAsync(x => x.EnCode == input.enCode && x.DeleteMark == null) || await _timeTaskRepository.IsAnyAsync(x => x.FullName == input.fullName && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.COM1004);
            var comtentModel = input.executeContent.Deserialize<ContentModel>();
            comtentModel.TenantId = _userManager.TenantId;
            comtentModel.TenantDbName = _userManager.TenantDbName;
            var entity = input.Adapt<TimeTaskEntity>();
            entity.ExecuteContent = comtentModel.Serialize();
            entity.ExecuteCycleJson = comtentModel.cron;
            var result = await Create(entity);
            _ = result ?? throw HSZException.Oh(ErrorCode.COM1000);

            // 添加到任务调度里
            AddTimerJob(result);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] TimeTaskUpInput input)
        {
            if (await _timeTaskRepository.IsAnyAsync(x => x.Id != id && x.EnCode == input.enCode && x.DeleteMark == null) || await _timeTaskRepository.IsAnyAsync(x => x.Id != id && x.FullName == input.fullName && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.COM1004);
            var entityOld = await GetInfo(id);
            // 先从调度器里取消
            SpareTime.Cancel(id);
            var entityNew = input.Adapt<TimeTaskEntity>();
            entityNew.RunCount = entityOld.RunCount;
            entityNew.EnabledMark = entityOld.EnabledMark;
            var comtentModel = input.executeContent.Deserialize<ContentModel>();
            comtentModel.TenantId = _userManager.TenantId;
            comtentModel.TenantDbName = _userManager.TenantDbName;
            entityNew.ExecuteContent = comtentModel.Serialize();
            entityNew.ExecuteCycleJson = comtentModel.cron;
            var isOk = await Update(entityNew);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1001);

            // 再添加到任务调度里
            if (entityNew.EnabledMark == 1)
            {
                AddTimerJob(entityNew);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await GetInfo(id);
            if (entity == null)
                throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await Delete(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1002);
            // 从调度器里取消
            SpareTime.Cancel(entity.Id);
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/Stop")]
        public async Task Stop(string id)
        {
            var entity = await GetInfo(id);
            entity.EnabledMark = 0;
            var isOk = await Update(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1003);
            SpareTime.Stop(entity.Id);
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/Enable")]
        public async Task Enable(string id)
        {
            var entity = await GetInfo(id);
            entity.EnabledMark = 1;
            var isOk = await Update(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1003);

            var comtentModel = entity.ExecuteContent.Deserialize<ContentModel>();
            comtentModel.TenantId = _userManager.TenantId;
            comtentModel.TenantDbName = _userManager.TenantDbName;
            entity.ExecuteContent = comtentModel.Serialize();
            var timer = SpareTime.GetWorkers().ToList().Find(u => u.WorkerName == id);
            if (timer == null)
            {
                AddTimerJob(entity);
            }
            else
            {
                // 如果 StartNow 为 flase , 执行 AddTimerJob 并不会启动任务
                SpareTime.Start(entity.Id);
            }
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 新增定时任务
        /// </summary>
        /// <param name="input"></param>
        [NonAction]
        public async void AddTimerJob(TimeTaskEntity input)
        {
            Action<SpareTimer, long> action = null;
            var comtentModel = input.ExecuteContent.Deserialize<ContentModel>();
            input.ExecuteCycleJson = comtentModel.cron;
            switch (input.ExecuteType)
            {
                case "3":
                    // 查询符合条件的任务方法
                    var taskMethod = GetTaskMethods()?.Result.FirstOrDefault(m => m.id == comtentModel.localHostTaskId);
                    if (taskMethod == null) break;
                    // 创建任务对象
                    var typeInstance = Activator.CreateInstance(taskMethod.DeclaringType);
                    // 创建委托
                    action = (Action<SpareTimer, long>)Delegate.CreateDelegate(typeof(Action<SpareTimer, long>), typeInstance, taskMethod.MethodName);
                    break;
                default:
                    action = async (timer, count) =>
                    {
                        var msg = await PerformJob(input);
                    };
                    break;
            }
            if (action == null) return;
            SpareTime.Do(comtentModel.cron, action, input.Id, comtentModel.TenantId + "/" + comtentModel.TenantDbName, true, executeType: SpareTimeExecuteTypes.Parallel);
        }

        /// <summary>
        /// 启动自启动任务
        /// </summary>
        [NonAction]
        public void StartTimerJob()
        {
            //非多租户模式启动自启任务
            if (!KeyVariable.MultiTenancy)
            {
                var list = _timeTaskRepository.AsQueryable().Where(x => x.DeleteMark == null && x.EnabledMark == 1).ToList();
                //查询数据库现有开启的定时任务列表
                list.ForEach(AddTimerJob);
            }
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tenantDic"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<TimeTaskEntity> GetInfo(string id, Dictionary<string, string> tenantDic = null)
        {
            return await _timeTaskRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<TimeTaskEntity>> GetList()
        {
            return await _timeTaskRepository.AsQueryable().Where(x => x.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public bool IsAny(string id)
        {
            return _timeTaskRepository.IsAny(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<TimeTaskEntity> Create(TimeTaskEntity entity)
        {
            return await _timeTaskRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
        }

        /// <summary>
        /// 新增日志
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tenantDic"></param>
        /// <returns></returns>
        [NonAction]
        public int CreateTaskLog(TimeTaskLogEntity entity, Dictionary<string, string> tenantDic = null)
        {
            return _timeTaskLogRepository.Insert(entity) ? 0 : 1;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(TimeTaskEntity entity)
        {
            return await _timeTaskRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tenantDic"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(TimeTaskEntity entity, Dictionary<string, string> tenantDic = null)
        {
            return await _timeTaskRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 根据类型执行任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<string> PerformJob(TimeTaskEntity entity)
        {
            try
            {
                var model = entity.ExecuteContent.Deserialize<ContentModel>();
                var parameters = model.parameter.ToDictionary(key => key.field, value => value.value.IsNotEmptyOrNull() ? value.value : value.defaultValue);
                await _dataInterfaceService.GetResponseByType(model.interfaceId, 3, model.TenantId, null, parameters);
                return "";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        /// <summary>
        /// 获取所有本地任务
        /// </summary>
        /// <returns></returns>
        private async Task<List<TaskMethodInfo>> GetTaskMethods()
        {
            var taskMethods = await _cacheManager.GetAsync<List<TaskMethodInfo>>(CommonConst.CACHE_KEY_TIMER_JOB);
            if (taskMethods != null) return taskMethods;
            // 获取所有本地任务方法，必须有spareTimeAttribute特性
            taskMethods = App.EffectiveTypes
                .Where(u => u.IsClass && !u.IsInterface && !u.IsAbstract && typeof(ISpareTimeWorker).IsAssignableFrom(u))
                .SelectMany(u => u.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.IsDefined(typeof(SpareTimeAttribute), false) &&
                       m.GetParameters().Length == 2 &&
                       m.GetParameters()[0].ParameterType == typeof(SpareTimer) &&
                       m.GetParameters()[1].ParameterType == typeof(long) && m.ReturnType == typeof(void))
                .Select(m =>
                {
                    // 默认获取第一条任务特性
                    var spareTimeAttribute = m.GetCustomAttribute<SpareTimeAttribute>();
                    return new TaskMethodInfo
                    {
                        id = $"{m.DeclaringType.Name}/{m.Name}",
                        fullName = spareTimeAttribute.WorkerName,
                        RequestUrl = $"{m.DeclaringType.Name}/{m.Name}",
                        cron = spareTimeAttribute.CronExpression,
                        DoOnce = spareTimeAttribute.DoOnce,
                        ExecuteType = spareTimeAttribute.ExecuteType,
                        Interval = (int)spareTimeAttribute.Interval / 1000,
                        StartNow = spareTimeAttribute.StartNow,
                        RequestType = RequestTypeEnum.Run,
                        Remark = spareTimeAttribute.Description,
                        TimerType = string.IsNullOrEmpty(spareTimeAttribute.CronExpression) ? SpareTimeTypes.Interval : SpareTimeTypes.Cron,
                        MethodName = m.Name,
                        DeclaringType = m.DeclaringType
                    };
                })).ToList();
            await _cacheManager.SetAsync(CommonConst.CACHE_KEY_TIMER_JOB, taskMethods);
            return taskMethods;
        }
        #endregion
    }
}
