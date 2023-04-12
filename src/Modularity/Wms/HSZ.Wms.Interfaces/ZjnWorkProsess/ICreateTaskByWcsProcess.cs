using HSZ.UnifyResult;
using HSZ.wms.Entitys.Dto.ZjnWmsTask;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Wms.Interfaces.ZjnWorkProsess
{
    /// <summary>
    /// WCS主动发起新任务
    /// </summary>
    public interface ICreateTaskByWcsProcess
    {
        /// <summary>
        /// wcs创建任务
        /// </summary>
        /// <param name="input">子任务id</param>
        /// <returns></returns>
        public Task<bool> WcsCreateTask(WcsCreateTaskInput input);
        /// <summary>
        /// 叠盘机产生任务函数
        /// </summary>
        /// <param name="Reserve1">数量</param>
        /// <param name="TrayCode">托盘码</param>
        /// <param name="DeviceCode">设备号</param>
        /// <returns></returns>
        public Task<RESTfulResult<bool>> FoldingPlateMachine(float Reserve1, string TrayCode, string DeviceCode);
    }
}
