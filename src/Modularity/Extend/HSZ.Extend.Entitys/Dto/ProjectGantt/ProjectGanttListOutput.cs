using HSZ.Dependency;
using System;
using System.Collections.Generic;

namespace HSZ.Extend.Entitys.Dto.ProjectGantt
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：获取项目管理列表(带分页)
    /// </summary>
    [SuppressSniffer]
    public class ProjectGanttListOutput
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
        public int? schedule { get; set; }
        /// <summary>
        /// 项目工期
        /// </summary>
        public double timeLimit { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 参加人员
        /// </summary>
        public string managerIds { get; set; }
        /// <summary>
        /// 参加人员信息
        /// </summary>
        public List<ManagersInfo> managersInfo { get; set; } = new List<ManagersInfo>();
    }

    public class ManagersInfo
    {
        /// <summary>
        /// 账号+名字
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string headIcon { get; set; }
    }

}
