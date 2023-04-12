using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWcsPlcObject
{
    /// <summary>
    /// PLC对象表修改输入参数
    /// </summary>
    public class ZjnWcsPlcObjectCrInput
    {
        /// <summary>
        /// 区域
        /// </summary>
        public string region { get; set; }
        
        /// <summary>
        /// Plc点位
        /// </summary>
        public string plcPointId { get; set; }
        
        /// <summary>
        /// Plc设备
        /// </summary>
        public string plcId { get; set; }
        
        /// <summary>
        /// DB
        /// </summary>
        public int? db { get; set; }
        
        /// <summary>
        /// 起点
        /// </summary>
        public int? start { get; set; }
        
        /// <summary>
        /// 长度
        /// </summary>
        public int? length { get; set; }
        
        /// <summary>
        /// 对象名称，长
        /// </summary>
        public string objType { get; set; }
        
        /// <summary>
        /// 对象，json
        /// </summary>
        public string objValue { get; set; }
        
        /// <summary>
        /// READ,WRITE,STATUS
        /// </summary>
        public string packType { get; set; }
        
        /// <summary>
        /// Plc包大小
        /// </summary>
        public int? packSize { get; set; }
        
        /// <summary>
        /// 设备ID
        /// </summary>
        public string deviceId { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string descrip { get; set; }
        
        
        /// <summary>
        /// 设备分组
        /// </summary>
        public string stackerGroup { get; set; }
        
    }
}