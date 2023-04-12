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
    /// 子任务业务处理 --- RGV业务
    /// </summary>
    public interface IRgvDeviceProcess
    {
        /// <summary>
        /// RGV任务开始，RGV终点接驳台如果没有扫描枪，应该从这里开始分配货位
        /// </summary>
        /// <param name="taskDetail">任务编号</param>
        /// <param name="TaskState">任务状态</param>
        /// <returns></returns>
        public Task<ZjnWmsTaskDetailsInfoOutput> RGVDetailStart(ZjnWmsTaskDetailsEntity taskDetail, int TaskState);
    }
}
