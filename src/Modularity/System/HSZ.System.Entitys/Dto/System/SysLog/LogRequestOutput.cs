using HSZ.Dependency;
using System;

namespace HSZ.System.Entitys.Dto.System.SysLog
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：请求日记输出
    /// </summary>
    [SuppressSniffer]
    public class LogRequestOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 请求时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 请求用户名
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 请求IP
        /// </summary>
        public string ipaddress { get; set; }

        /// <summary>
        /// 请求设备
        /// </summary>
        public string platForm { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string requestURL { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        public string requestMethod { get; set; }

        /// <summary>
        /// 请求耗时
        /// </summary>
        public int requestDuration { get; set; }
    }
}
