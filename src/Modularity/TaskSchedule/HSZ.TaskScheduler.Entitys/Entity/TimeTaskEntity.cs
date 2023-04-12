using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;
using System;

namespace HSZ.TaskScheduler.Entitys.Entity
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：定时任务
    /// </summary>
    [SugarTable("ZJN_BASE_TIME_TASK")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class TimeTaskEntity : CLDEntityBase
    {
        /// <summary>
        /// 任务编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }
        /// <summary>
        /// 执行类型：【0:Api 1:Sql 2:本地方法】
        /// </summary>
        [SugarColumn(ColumnName = "F_EXECUTETYPE")]
        public string ExecuteType { get; set; }
        /// <summary>
        /// 执行内容
        /// </summary>
        [SugarColumn(ColumnName = "F_EXECUTECONTENT")]
        public string ExecuteContent { get; set; }
        /// <summary>
        /// 执行周期
        /// </summary>
        [SugarColumn(ColumnName = "F_EXECUTECYCLEJSON")]
        public string ExecuteCycleJson { get; set; }
        /// <summary>
        /// 最后运行时间
        /// </summary>
        [SugarColumn(ColumnName = "F_LASTRUNTIME")]
        public DateTime? LastRunTime { get; set; }
        /// <summary>
        /// 下次运行时间
        /// </summary>
        [SugarColumn(ColumnName = "F_NEXTRUNTIME")]
        public DateTime? NextRunTime { get; set; }
        /// <summary>
        /// 运行次数
        /// </summary>
        [SugarColumn(ColumnName = "F_RUNCOUNT")]
        public int? RunCount { get; set; } = 0;
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }
    }
}
