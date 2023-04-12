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
    /// 子任务业务处理 --- 堆垛机消防业务
    /// </summary>
    public interface IFireControlProcess
    {
        /// <summary>
        /// 堆垛机消防业务处理  LES信息暂未上传
        /// </summary>
        /// <param name="WmsTaskData">子任务数据</param>
        /// <param name="TaskState">状态</param>
        /// <param name="parameter">重量</param>
        /// <returns></returns>
        public Task<ZjnWmsTaskDetailsInfoOutput> FireControlStart(ZjnWmsTaskDetailsEntity WmsTaskData, int TaskState, TaskResultPubilcParameter parameter);
    }
}
