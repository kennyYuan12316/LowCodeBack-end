using HSZ.Dependency;
using System;

namespace HSZ.Extend.Entitys.Dto.ProjectGantt
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：添加项目
    /// </summary>
    [SuppressSniffer]
    public class ProjectGanttCrInput
    {
        /// <summary>
        /// 项目编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 项目状态(1-进行中，2-已暂停)
        /// </summary>
        public int? state { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// 完成进度
        /// </summary>
        public double? schedule { get; set; }
        /// <summary>
        /// 项目工期
        /// </summary>
        public int? timeLimit { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 参与人员
        /// </summary>
        public string managerIds { get; set; }
        /// <summary>
        /// 项目描述
        /// </summary>
        public string description { get; set; }
    }
}
