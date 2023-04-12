using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.TaskScheduler.Entitys.Entity
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：定时任务日志
    /// </summary>
    [SugarTable("ZJN_BASE_TIME_TASK_LOG")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class TimeTaskLogEntity: EntityBase<string>
    {
        /// <summary>
        /// 定时任务主键
        /// </summary>
        [SugarColumn(ColumnName = "F_TASKID")]
        public string TaskId { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        [SugarColumn(ColumnName = "F_RUNTIME")]
        public DateTime? RunTime { get; set; }

        /// <summary>
        /// 执行结果
        /// </summary>
        [SugarColumn(ColumnName = "F_RUNRESULT")]
        public int? RunResult { get; set; }

        /// <summary>
        /// 执行说明
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
