using HSZ.Dependency;
using System;

namespace HSZ.System.Entitys.Dto.System.SysLog
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：异常日记输出
    /// </summary>
    [SuppressSniffer]
    public class LogExceptionOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 异常IP
        /// </summary>
        public string ipaddress { get; set; }

        /// <summary>
        /// 异常功能
        /// </summary>
        public string moduleName { get; set; }

        /// <summary>
        /// 日志分类
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 异常描述
        /// </summary>
        public string json { get; set; }
    }
}
