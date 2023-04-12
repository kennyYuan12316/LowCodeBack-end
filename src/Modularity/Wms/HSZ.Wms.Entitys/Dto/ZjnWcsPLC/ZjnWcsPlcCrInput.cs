using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWcsPlc
{
    /// <summary>
    /// PLC连接表修改输入参数
    /// </summary>
    public class ZjnWcsPlcCrInput
    {

        /// <summary>
        /// Key
        /// </summary>
        public string plcId { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool isActive { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public string caption { get; set; }
        
        /// <summary>
        /// 区域
        /// </summary>
        public string region { get; set; }
        
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool isConnected { get; set; }
        
        /// <summary>
        /// 类型;1500;1200
        /// </summary>
        public int? cpuType { get; set; }
        
        /// <summary>
        /// ip
        /// </summary>
        public string ip { get; set; }
        
        /// <summary>
        /// port
        /// </summary>
        public int? port { get; set; }
        
        /// <summary>
        /// Plc Rack
        /// </summary>
        public int? rack { get; set; }
        
        /// <summary>
        /// Plc Sock
        /// </summary>
        public int? sock { get; set; }
        
        /// <summary>
        /// Plc读写超时MS
        /// </summary>
        public int? timeOut { get; set; }
        
        /// <summary>
        /// 堆垛机,DeviceID,Plc读写包用
        /// </summary>
        public string stackerId { get; set; }
        
        /// <summary>
        /// 是否堆垛机
        /// </summary>
        public bool isStacker { get; set; }
        
        /// <summary>
        /// 异常
        /// </summary>
        public string error { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string descrip { get; set; }
        
    }
}