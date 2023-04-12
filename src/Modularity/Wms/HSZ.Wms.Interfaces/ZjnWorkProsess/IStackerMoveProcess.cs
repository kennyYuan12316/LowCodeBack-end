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
    /// 子任务业务处理 --- 堆垛机移库业务
    /// </summary>
    public interface IStackerMoveProcess
    {
        /// <summary>
        /// 堆垛机移库任务
        /// </summary>
        /// <param name="WmsTaskData"></param>
        /// <param name="TaskState"></param>
        /// <returns></returns>
        public Task<ZjnWmsTaskDetailsInfoOutput> StackerMoveTask(ZjnWmsTaskDetailsEntity WmsTaskData, int TaskState, TaskResultPubilcParameter parameter);
    }
}
