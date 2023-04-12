using HSZ.Entitys.wms;
using HSZ.wms.Entitys.Dto.ZjnWmsTask;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using HSZ.WorkFlow.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.zjnWcsProcessConfig
{
    /// <summary>
    /// 业务路径配置表 json模板输出数据
    /// </summary>
    public class ZjnWcsProcessConfigJsonOutput
    {

        /// <summary>
        /// 业务路径数据
        /// </summary>
        public ZjnWmsTaskCrInput zjnTaskInfoOutput { get; set; }

        /// <summary>
        /// 子任务数据
        /// </summary>
        public List<ZjnWmsTaskDetailsInfoOutput> ZjnTaskListDetailsList { get; set; } = new List<ZjnWmsTaskDetailsInfoOutput>();

        public string FlowTemplateJson { get; set; }
    }
}