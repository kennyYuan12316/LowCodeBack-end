using HSZ.Dependency;
using System;

namespace HSZ.TaskScheduler.Entitys.Dto.TaskScheduler
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class TimeTaskTaskLogListOutput
    {
        /// <summary>
        /// 执行说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 执行结果
        /// </summary>
        public int runResult { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime? runTime { get; set; }
    }
}
