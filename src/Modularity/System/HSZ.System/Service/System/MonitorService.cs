using HSZ.Common.Util;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.System.Entitys.Dto.System.Monitor.Dto;
using HSZ.System.Interfaces.System;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Runtime.InteropServices;

namespace HSZ.System.Core.Service.Monitor.Dto
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：系统监控
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "Monitor", Order = 215)]
    [Route("api/system/[controller]")]
    public class MonitorService: IMonitorService, IDynamicApiController, ITransient
    {
        #region Get
        /// <summary>
        /// 系统监控
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public dynamic GetInfo()
        {
            var flag_Linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            var flag_Windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            MonitorOutput output = new MonitorOutput();
            if (flag_Linux)
            {
                output.system = MachineUtil.GetSystemInfo_Linux();
                output.cpu = MachineUtil.GetCpuInfo_Linux();
                output.memory = MachineUtil.GetMemoryInfo_Linux();
                output.disk = MachineUtil.GetDiskInfo_Linux();
            }
            if (flag_Windows)
            {
                output.system = MachineUtil.GetSystemInfo_Windows();
                output.cpu = MachineUtil.GetCpuInfo_Windows();
                output.memory = MachineUtil.GetMemoryInfo_Windows();
                output.disk = MachineUtil.GetDiskInfo_Windows();
            }
            output.time = DateTime.Now;
            return output;
        }
        #endregion
    }
}
