using HSZ.Data.SqlSugar.Extensions;
using HSZ.Dependency;
using HSZ.EventBus;
using HSZ.EventHandler;
using HSZ.TaskScheduler.Entitys.Entity;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.TaskScheduler.Listener
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：定时任务监听器
    /// </summary>
    public class SpareTimeListener : ISpareTimeListener, ISingleton
    {
        private readonly IEventPublisher _eventPublisher;

        public static SqlSugarScope Db = new SqlSugarScope(new ConnectionConfig()
        {
            ConfigId = App.Configuration["ConnectionStrings:ConfigId"],
            DbType = (DbType)Enum.Parse(typeof(DbType), App.Configuration["ConnectionStrings:DBType"]),
            ConnectionString = string.Format($"{App.Configuration["ConnectionStrings:DefaultConnection"]}", App.Configuration["ConnectionStrings:DBName"]),
            IsAutoCloseConnection = true,
            ConfigureExternalServices = new ConfigureExternalServicesExtenisons()
            {
                EntityNameServiceType = typeof(SugarTable)//这个不管是不是自定义都要写，主要是用来获取所有实体
            }
        });
        /// <summary>
        /// 构造函数
        /// </summary>
        public SpareTimeListener(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }
        /// <summary>
        /// 监听所有任务
        /// </summary>
        /// <param name="executer"></param>
        /// <returns></returns>
        public async Task OnListener(SpareTimerExecuter executer)
        {
            switch (executer.Status)
            {
                // 执行开始通知
                case 0:
                    //Console.WriteLine($"{executer.Timer.WorkerName} 任务开始通知");
                    break;
                // 任务执行之前通知
                case 1:
                    //Console.WriteLine($"{executer.Timer.WorkerName} 执行之前通知");
                    break;
                // 执行成功通知
                case 2:
                // 任务执行失败通知
                case 3:
                    await RecoreTaskLog(executer);
                    break;
                // 任务执行停止通知
                case -1:
                    //Console.WriteLine($"{executer.Timer.WorkerName} 执行停止通知");
                    break;
                // 任务执行取消通知
                case -2:
                    //Console.WriteLine($"{executer.Timer.WorkerName} 执行取消通知");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="executer"></param>
        /// <returns></returns>
        private async Task RecoreTaskLog(SpareTimerExecuter executer)
        {
            var TenantId = executer.Timer.Description.Split("/")[0];
            var TenantDbName = executer.Timer.Description.Split("/")[1];
            Db.ChangeDatabase(TenantId);
            if (!Db.IsAnyConnection(TenantId))
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    DbType = (SqlSugar.DbType)Enum.Parse(typeof(SqlSugar.DbType), App.Configuration["ConnectionStrings:DBType"]),
                    ConfigId = TenantId,//设置库的唯一标识
                    IsAutoCloseConnection = true,
                    ConnectionString = string.Format($"{App.Configuration["ConnectionStrings:DefaultConnection"]}", TenantDbName)
                });
            }
            var taskEntity = await Db.Queryable<TimeTaskEntity>().FirstAsync(x => x.Id == executer.Timer.WorkerName);
            var nextRunTime = ((DateTimeOffset)SpareTime.GetCronNextOccurrence(taskEntity.ExecuteCycleJson)).DateTime;

            await _eventPublisher.PublishAsync(new TaskEventSource("Task:UpdateTask", TenantId, TenantDbName, new TimeTaskEntity()
            {
                Id = taskEntity.Id,
                NextRunTime = nextRunTime,
            }));

            await _eventPublisher.PublishAsync(new TaskLogEventSource("Log:CreateTaskLog", TenantId, TenantDbName, new TimeTaskLogEntity()
            {
                Id = YitIdHelper.NextId().ToString(),
                TaskId = executer.Timer.WorkerName,
                RunTime = DateTime.Now.AddSeconds(10),
                RunResult = executer.Status == 2 ? 0 : 1,
                Description = executer.Status == 2 ? "执行成功": "执行失败,失败原因:" + JsonConvert.SerializeObject(executer.Timer.Exception)
            }));
        }
    }
}
