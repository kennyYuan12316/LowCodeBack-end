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
    /// 子任务业务处理 --- 出库台业务
    /// </summary>
    public interface IWarehousOutProcess
    {
        /// <summary>
        /// 出库台任务
        /// </summary>
        /// <param name="WmsTaskData"></param>
        /// <param name="TaskState"></param>
        /// <returns></returns>
        public Task<ZjnWmsTaskDetailsInfoOutput> OutboundDeskTask(ZjnWmsTaskDetailsEntity WmsTaskData, int TaskState);
    }
}
