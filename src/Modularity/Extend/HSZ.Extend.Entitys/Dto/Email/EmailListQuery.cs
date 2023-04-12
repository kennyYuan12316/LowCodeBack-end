using HSZ.Common.Filter;
using HSZ.Dependency;

namespace HSZ.Extend.Entitys.Dto.Email
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：(带分页)获取邮件列表入参(收件箱、标星件、草稿箱、已发送)
    /// </summary>
    [SuppressSniffer]
    public class EmailListQuery : PageInputBase
    {
        /// <summary>
        /// 开始时间，时间戳
        /// </summary>
        public long? startTime { get; set; }
        /// <summary>
        /// 结束时间，时间戳
        /// </summary>
        public long? endTime { get; set; }
        /// <summary>
        ///类型
        /// </summary>
        public string type { get; set; }

    }
}
