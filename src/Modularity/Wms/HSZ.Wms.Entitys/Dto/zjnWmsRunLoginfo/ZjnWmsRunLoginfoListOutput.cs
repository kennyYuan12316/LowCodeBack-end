using System;

namespace HSZ.wms.Entitys.Dto.ZjnWmsRunLoginfo
{
    /// <summary>
    /// wms运行日志输入参数
    /// </summary>
    public class ZjnWmsRunLoginfoListOutput
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 业务类型（函数注释）
        /// </summary>
        public string F_TaskType { get; set; }
        
        /// <summary>
        /// 方法名
        /// </summary>
        public string F_MethodName { get; set; }
        
        /// <summary>
        /// 方法参数
        /// </summary>
        public string F_MethodParmes { get; set; }
        
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
        
        /// <summary>
        /// 是否报错：0否  1是
        /// </summary>
        public int? F_IsBug { get; set; }
        
        /// <summary>
        /// 报错信息
        /// </summary>
        public string F_Message { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_Case1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_Case2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_Case3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_Case4 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_Case5 { get; set; }
        
    }
}