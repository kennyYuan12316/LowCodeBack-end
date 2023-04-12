using HSZ.Dependency;
using HSZ.TaskScheduler.Entitys.Enum;
using System;

namespace HSZ.TaskScheduler.Entitys.Model
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class TaskMethodInfo
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 方法所属类的Type对象
        /// </summary>
        public Type DeclaringType { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 只执行一次
        /// </summary>
        public bool DoOnce { get; set; } = false;

        /// <summary>
        /// 立即执行（默认等待启动）
        /// </summary>
        public bool StartNow { get; set; } = false;

        /// <summary>
        /// 执行类型(并行、列队)
        /// </summary>
        public SpareTimeExecuteTypes ExecuteType { get; set; }

        /// <summary>
        /// 执行间隔时间（单位秒）
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Cron表达式
        /// </summary>
        public string cron { get; set; }

        /// <summary>
        /// 定时器类型
        /// </summary>
        public SpareTimeTypes TimerType { get; set; }

        /// <summary>
        /// 请求url
        /// </summary>
        public string RequestUrl { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        /// <example>2</example>
        public RequestTypeEnum RequestType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
