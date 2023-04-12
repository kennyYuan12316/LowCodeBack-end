using HSZ.Dependency;
using HSZ.TaskScheduler.Entitys.Enum;
using System;
using System.Collections.Generic;

namespace HSZ.TaskScheduler.Entitys.Model
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class ContentModel
    {
        /// <summary>
        /// 表达式
        /// </summary>
        public string cron { get; set; }

        /// <summary>
        /// id
        /// </summary>
        public string interfaceId { get; set; }

        /// <summary>
        /// 接口名
        /// </summary>
        public string interfaceName { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public List<InterfaceParameter> parameter { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// 租户数据库名称
        /// </summary>
        public string TenantDbName { get; set; }

        public string localHostTaskId { get; set; }
    }

    [SuppressSniffer]
    public class InterfaceParameter
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string defaultValue { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }
    }
}