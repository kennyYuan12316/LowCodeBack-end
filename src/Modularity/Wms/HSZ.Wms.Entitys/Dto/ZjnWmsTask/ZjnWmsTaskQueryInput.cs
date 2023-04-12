using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsTask
{
    /// <summary>
    /// 主任务列表查询输入
    /// </summary>
    public class ZjnWmsTaskQueryInput : PageInputBase
    {
        /// <summary>
        /// 选择导出数据key
        /// </summary>
        public string selectKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int dataType { get; set; }

        /// <summary>
        /// 任务号
        /// </summary>
        public string F_TaskNo { get; set; }
        
        /// <summary>
        /// 任务名称
        /// </summary>
        public string F_TaskName { get; set; }
        
        /// <summary>
        /// 任务状态描述
        /// </summary>
        public string F_TaskDescribe { get; set; }
        
    }
}