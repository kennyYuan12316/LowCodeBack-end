using HSZ.Common.Configuration;
using HSZ.Common.Helper;
using HSZ.Data.SqlSugar.Extensions;
using HSZ.EventBus;
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
    /// 描 述：日记事件订阅
    /// </summary>
    public class LogEventSubscriber : IEventSubscriber
    {
        private readonly ILogger<LogEventSubscriber> _logger;

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
        public LogEventSubscriber(ILogger<LogEventSubscriber> logger)
        {
            _logger = logger;
        }

        [EventSubscribe("Log:CreateReLog")]
        [EventSubscribe("Log:CreateExLog")]
        [EventSubscribe("Log:CreateVisLog")]
        [EventSubscribe("Log:CreateOpLog")]
        public async Task CreateLog(EventHandlerExecutingContext context)
        {
            var log = (LogEventSource)context.Source;
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
            await Db.Insertable(log.Entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
        }

        /// <summary>
        /// 创建任务日记
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [EventSubscribe("Log:CreateTaskLog")]
        public async Task CreateTaskLog(EventHandlerExecutingContext context)
        {
            var log = (TaskLogEventSource)context.Source;
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
            await Db.Insertable(log.Entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
        }
    }
}