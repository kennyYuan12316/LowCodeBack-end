using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsRunLoginfo
{
    /// <summary>
    /// wms运行日志列表查询输入
    /// </summary>
    public class ZjnWmsRunLoginfoListQueryInput : PageInputBase
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
        /// 业务类型（函数注释）
        /// </summary>
        public string F_TaskType { get; set; }
        
        /// <summary>
        /// 方法名
        /// </summary>
        public string F_MethodName { get; set; }
        
        /// <summary>
        /// 任务号
        /// </summary>
        public string F_TaskNo { get; set; }
        
        /// <summary>
        /// 设备号
        /// </summary>
        public string F_DeviceNo { get; set; }
        
        /// <summary>
        /// 托盘号
        /// </summary>
        public string F_TrayNo { get; set; }
        
    }
}