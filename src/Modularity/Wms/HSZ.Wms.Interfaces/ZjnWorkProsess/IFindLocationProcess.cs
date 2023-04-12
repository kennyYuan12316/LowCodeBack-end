using HSZ.Entitys.wms;
using HSZ.wms.Entitys.Dto.ZjnWmsTask;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Wms.Interfaces.ZjnWorkProsess
{
    /// <summary>
    /// 入库寻找货位类
    /// </summary>
    public interface IFindLocationProcess
    {
        /// <summary>
        /// 分配货位并生成子任务
        /// </summary>
        /// <param name="dto">子任务信息</param>
        /// <returns></returns>
        Task<ZjnWmsTaskDetailsInfoOutput> WmsAllotLocationTask(ZjnWmsTaskDetailsInfoOutput dto);

        /// <summary>
        /// 入库直接开始堆垛机任务（测试直接从入库口入库）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<ZjnWmsTaskDetailsInfoOutput> TestStackerAllot(ZjnWmsTaskDetailsInfoOutput dto);

        /// <summary>
        /// 找空托
        /// </summary>
        /// <param name="trayType">托盘类型</param>
        /// <param name="deviceId">设备号</param>
        /// <returns></returns>
        Task<ZjnWmsLocationEntity> FindEmptyTrayLocation(int trayType, string deviceId);


        /// <summary>
        /// 找消防货位，优先找距离近的消防位置
        /// </summary>
        /// <param name="location">消防货位</param>
        /// <returns></returns>
        Task<ZjnWmsTaskDetailsInfoOutput> FindFireControlLocation(ZjnWmsTaskDetailsInfoOutput dto);

        /// <summary>
        /// 找出库货位   
        /// </summary>
        /// <param name="trayCodes">托盘条码</param>
        /// <param name="materialCode">物料编码</param>
        /// <param name="qty">数量</param>
        /// <returns>货位和优先级</returns>
        Task<List<ZjnWmsTaskCrInput>> FindOutBoundLocation(ZjnWmsTaskCrInput TaskCrInput);

        /// <summary>
        /// 寻找移库货位（暂不考虑跨巷道）
        /// </summary>
        /// <param name="input">移库信息</param>
        /// <returns>终点货位编码</returns>
        Task<string> FindMoveLocation(ZjnWmsTaskCrInput input);
    }
}
