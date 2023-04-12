using HSZ.Common.TaskResultPubilcParms;
using HSZ.Entitys.wms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Wms.Interfaces.ZjnWorkProsess
{
    /// <summary>
    /// 子任务业务处理 --- 堆垛机入库业务
    /// </summary>
    public interface IStackerInProcess
    {
        /// <summary>
        /// 堆垛机入库业务处理
        /// </summary>
        /// <param name="WmsTaskData">子任务号</param>
        /// <param name="TaskState">状态</param>
        /// <param name="parameter">重量</param>
        /// <returns></returns>
        public Task singleScTask(ZjnWmsTaskDetailsEntity WmsTaskData, int TaskState, TaskResultPubilcParameter parameter);
    }
}
