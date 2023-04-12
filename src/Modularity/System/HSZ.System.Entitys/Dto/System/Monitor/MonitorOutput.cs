using HSZ.Common.Model.Machine;
using HSZ.Dependency;
using System;

namespace HSZ.System.Entitys.Dto.System.Monitor.Dto
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class MonitorOutput
    {
        /// <summary>
        /// 系统信息
        /// </summary>
        public SystemInfoModel system { get; set; }

        /// <summary>
        /// CPU信息
        /// </summary>
        public CpuInfoModel cpu { get; set; }

        /// <summary>
        /// 内存信息
        /// </summary>
        public MemoryInfoModel memory { get; set; }

        /// <summary>
        /// 硬盘信息
        /// </summary>
        public DiskInfoModel disk { get; set; }

        /// <summary>
        /// 服务器当时时间戳
        /// </summary>
        public DateTime? time { get; set; }
    }
}
