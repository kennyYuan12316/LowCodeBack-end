using HSZ.Common.Filter;
using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.SysLog
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：系统日志列表入参
    /// </summary>
    [SuppressSniffer]
    public class LogListQuery : PageInputBase
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public long? startTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public long? endTime { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string ipaddress { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        public string requestMethod { get; set; }

        /// <summary>
        /// 模块名
        /// </summary>
        public string moduleName { get; set; }
    }
}
