using HSZ.Common.Configuration;
using HSZ.Data.SqlSugar.Extensions;
using HSZ.EventBus;
using HSZ.TaskScheduler.Entitys.Entity;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Threading.Tasks;

namespace HSZ.EventHandler
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：任务事件订阅
    /// </summary>
    public class TaskEventSubscriber : IEventSubscriber
    {
        private readonly ILogger<TaskEventSubscriber> _logger;

        /// <summary>
        /// SqlSugarScope操作数据库是线程安的可以单例
        /// </summary>
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
        }, db =>
        {
            //如果用单例配置要统一写在这儿
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                if (sql.StartsWith("SELECT"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                if (sql.StartsWith("UPDATE") || sql.StartsWith("INSERT"))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (sql.StartsWith("DELETE"))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                //在控制台输出sql语句
                Console.WriteLine(SqlProfiler.ParameterFormat(sql, pars));
                Console.WriteLine();
                //App.PrintToMiniProfiler("SqlSugar", "Info", SqlProfiler.ParameterFormat(sql, pars));
            };
        });

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        public TaskEventSubscriber(ILogger<TaskEventSubscriber> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 创建任务日记
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [EventSubscribe("Task:UpdateTask")]
        public async Task UpdateTask(EventHandlerExecutingContext context)
        {
            var log = (TaskEventSource)context.Source;
            if (KeyVariable.MultiTenancy)
            {
                if (log.TenantId == null) return;
                Db.AddConnection(new ConnectionConfig()
                {
                    DbType = (DbType)Enum.Parse(typeof(DbType), App.Configuration["ConnectionStrings:DBType"]),
                    ConfigId = log.TenantId,//设置库的唯一标识
                    IsAutoCloseConnection = true,
                    ConnectionString = string.Format($"{App.Configuration["ConnectionStrings:DefaultConnection"]}", log.TenantDbName)
                });
                Db.ChangeDatabase(log.TenantId);
            }
            await Db.Updateable<TimeTaskEntity>().SetColumns(x => new TimeTaskEntity()
            {
                RunCount = x.RunCount + 1,
                LastRunTime = DateTime.Now,
                NextRunTime = log.Entity.NextRunTime,
                LastModifyUserId = "admin",
                LastModifyTime = DateTime.Now
            }).Where(x => x.Id == log.Entity.Id).ExecuteCommandAsync();
        }
    }
}