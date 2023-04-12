using HSZ.Common.TaskResultPubilcParms;
using HSZ.Entitys.wms;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Wms.Interfaces.ZjnWorkProsess
{
    /// <summary>
    /// 子任务业务处理 --- 平面设备业务
    /// </summary>
    public interface IPlaneDeviceProcess
    {
        /// <summary>
        /// 平面设备任务
        /// </summary>
        /// <param name="WmsTaskData"></param>
        /// <param name="TaskState"></param>
        /// <returns></returns>
        public Task<ZjnWmsTaskDetailsInfoOutput> PlaneDeviceTask(ZjnWmsTaskDetailsEntity WmsTaskData, int TaskState, TaskResultPubilcParameter parameter);
    }
}
