using HSZ.Dependency;
using System;

namespace HSZ.System.Entitys.Dto.System.SysCache
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：缓存列表输出
    /// </summary>
    [SuppressSniffer]
    public class CacheListOutput
    {
        /// <summary>
        /// 缓存名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime overdueTime { get; set; }

        /// <summary>
        /// 缓存大小
        /// </summary>
        public long cacheSize { get; set; }

    }
}
