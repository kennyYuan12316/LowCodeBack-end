using HSZ.Dependency;

namespace HSZ.Extend.Entitys.Dto.Schedule
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：获取日程安排列表入参
    /// </summary>
    [SuppressSniffer]
    public class ScheduleListQuery
    {

        /// <summary>
        /// 开始时间
        /// </summary>
        public string startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string endTime { get; set; }

        public string dateTime { get; set; }

    }
}
