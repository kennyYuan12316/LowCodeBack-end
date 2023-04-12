using HSZ.Common.Util;
using HSZ.Dependency;
using System;

namespace HSZ.Extend.Entitys.Dto.ProjectGantt
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：获取项目任务树形
    /// </summary>
    [SuppressSniffer]
    public class ProjectGanttTaskTreeViewOutput : TreeModel
    {
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 完成度
        /// </summary>
        public int? schedule { get; set; }
        /// <summary>
        /// 项目id
        /// </summary>
        public string projectId { get; set; }

        /// <summary>
        /// 标记颜色
        /// </summary>
        public string signColor { get; set; }
        /// <summary>
        /// 标记
        /// </summary>
        public string sign { get; set; }
    }
}
