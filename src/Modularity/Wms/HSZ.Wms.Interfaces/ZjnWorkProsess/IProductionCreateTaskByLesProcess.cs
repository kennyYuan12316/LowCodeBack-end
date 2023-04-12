using HSZ.Common.TaskResultPubilcParms;
using HSZ.Entitys.wms;
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
    /// Les接口处理类>>>>一体化出入库任务创建、出入库申请、出入库确认、任务取消 by ljt
    /// </summary>
    public interface IProductionCreateTaskByLesProcess
    {
        /// <summary>
        /// 一体化出入库任务
        /// </summary>
        /// <param name="json">Les回传信息</param>
        /// <returns></returns>
        public Task  CreateTaskByLesInit(string json);

    }
}
