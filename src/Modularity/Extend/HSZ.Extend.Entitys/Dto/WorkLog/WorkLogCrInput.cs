using HSZ.Dependency;

namespace HSZ.Extend.Entitys.Dto.WorkLog
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class WorkLogCrInput
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 问题内容
        /// </summary>
        public string question { get; set; }
        /// <summary>
        /// 今日内容
        /// </summary>
        public string todayContent { get; set; }
        /// <summary>
        /// 明日内容
        /// </summary>
        public string tomorrowContent { get; set; }
        /// <summary>
        /// 接收人
        /// </summary>
        public string toUserId { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public string userIds { get; set; }
    }
}
