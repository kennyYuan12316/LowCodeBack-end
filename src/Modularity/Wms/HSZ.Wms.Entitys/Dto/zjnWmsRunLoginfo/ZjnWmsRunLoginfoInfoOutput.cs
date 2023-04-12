using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsRunLoginfo
{
    /// <summary>
    /// wms运行日志输出参数
    /// </summary>
    public class ZjnWmsRunLoginfoInfoOutput
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 业务类型（函数注释）
        /// </summary>
        public string taskType { get; set; }
        
        /// <summary>
        /// 方法名
        /// </summary>
        public string methodName { get; set; }
        
        /// <summary>
        /// 方法参数
        /// </summary>
        public string methodParmes { get; set; }
        
        /// <summary>
        /// 任务号
        /// </summary>
        public string taskNo { get; set; }
        
        /// <summary>
        /// 设备号
        /// </summary>
        public string deviceNo { get; set; }
        
        /// <summary>
        /// 托盘号
        /// </summary>
        public string trayNo { get; set; }
        
        /// <summary>
        /// 是否报错：0否  1是
        /// </summary>
        public string isBug { get; set; }
        
        /// <summary>
        /// 报错信息
        /// </summary>
        public string message { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string case1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string case2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string case3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string case4 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string case5 { get; set; }
        
    }
}