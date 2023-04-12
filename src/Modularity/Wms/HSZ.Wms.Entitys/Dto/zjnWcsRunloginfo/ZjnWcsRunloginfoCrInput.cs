using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWcsRunloginfo
{
    /// <summary>
    /// 运行日志修改输入参数
    /// </summary>
    public class ZjnWcsRunloginfoCrInput
    {
        /// <summary>
        /// 托盘条码1
        /// </summary>
        public string containerBarcode1 { get; set; }
        
        /// <summary>
        /// 托盘条码2
        /// </summary>
        public string containerBarcode2 { get; set; }
        
        /// <summary>
        /// 任务号1
        /// </summary>
        public string taskCode1 { get; set; }
        
        /// <summary>
        /// 任务号2
        /// </summary>
        public string taskCode2 { get; set; }
        
        /// <summary>
        /// 设备号
        /// </summary>
        public string equipmentCode { get; set; }
        
        /// <summary>
        /// 日志信息
        /// </summary>
        public string runLog { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public string runType { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
    }
}