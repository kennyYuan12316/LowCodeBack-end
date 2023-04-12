using HSZ.Dependency;
using HSZ.System.Entitys.Permission;
using HSZ.TaskScheduler;
using SqlSugar;
using System;

namespace HSZ.System.Service.Common
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：定时任务demo
    /// </summary>
    public class SpareTimeDemo : ISpareTimeWorker
    {
        /// <summary>
        /// 3秒后出勤统计
        /// </summary>
        /// <param name="timer">参数</param>
        /// <param name="count">次数</param>
        [SpareTime("* * * * * ?", "执行Sql", ExecuteType = SpareTimeExecuteTypes.Serial)]
        public void ExecSql(SpareTimer timer, long count)
        {
            //创建作用域
            Scoped.Create((factory, scope) =>
            {
                // 数据库操作
                var sqlSugarRepository = App.GetService<ISqlSugarRepository<UserEntity>>(scope.ServiceProvider);
                sqlSugarRepository.DeleteById("226890444955452677");
            });
        }

        /// <summary>
        /// 3秒后出勤统计
        /// </summary>
        /// <param name="timer">参数</param>
        /// <param name="count">次数</param>
        [SpareTime("0 0/1 * * * ?", "执行Sql1", ExecuteType = SpareTimeExecuteTypes.Serial)]
        public void ExecSql1(SpareTimer timer, long count)
        {
            //创建作用域
            Scoped.Create((factory, scope) =>
            {
                var start = DateTime.Now;
                Console.WriteLine(start.ToString("yyyy-MM-dd HH:mm:ss") + ":任务开始-----------");
                // 数据库操作
                var sqlSugarRepository = App.GetService<ISqlSugarRepository<UserEntity>>(scope.ServiceProvider);
                sqlSugarRepository.DeleteById("226890444955452677");
                var end = DateTime.Now;
                Console.WriteLine(end.ToString("yyyy-MM-dd HH:mm:ss") + ":任务结束-----------");
                Console.WriteLine($"SQL执行了：{count} 次,耗时：{(end - start).TotalMilliseconds}ms");
            });
        }
    }
}
