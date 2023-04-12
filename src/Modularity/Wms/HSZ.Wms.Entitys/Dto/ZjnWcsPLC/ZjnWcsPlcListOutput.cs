using System;

namespace HSZ.wms.Entitys.Dto.ZjnWcsPlc
{
    /// <summary>
    /// PLC连接表输入参数
    /// </summary>
    public class ZjnWcsPlcListOutput
    {
        /// <summary>
        /// Key
        /// </summary>
        public string PlcID { get; set; }
        
        /// <summary>
        /// 是否有效
        /// </summary>
        public string IsActive { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public string Caption { get; set; }
        
        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }
        
        /// <summary>
        /// 连接状态
        /// </summary>
        public string IsConnected { get; set; }
        
        /// <summary>
        /// 类型;1500;1200
        /// </summary>
        public int? CpuType { get; set; }
        
        /// <summary>
        /// ip
        /// </summary>
        public string IP { get; set; }
        
        /// <summary>
        /// port
        /// </summary>
        public int? Port { get; set; }
        
        /// <summary>
        /// Plc Rack
        /// </summary>
        public int? Rack { get; set; }
        
        /// <summary>
        /// Plc Sock
        /// </summary>
        public int? Sock { get; set; }
        
        /// <summary>
        /// Plc读写超时MS
        /// </summary>
        public int? TimeOut { get; set; }
        
        /// <summary>
        /// 堆垛机,DeviceID,Plc读写包用
        /// </summary>
        public string StackerID { get; set; }
        
        /// <summary>
        /// 是否堆垛机
        /// </summary>
        public string IsStacker { get; set; }
        
        /// <summary>
        /// 异常
        /// </summary>
        public string Error { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string Descrip { get; set; }
        
    }
}