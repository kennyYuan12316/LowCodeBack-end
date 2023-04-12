using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWcsPlcPoint
{
    /// <summary>
    /// PLC点位表输出参数
    /// </summary>
    public class ZjnWcsPlcPointInfoOutput
    {
        /// <summary>
        /// 有效
        /// </summary>
        public bool isActive { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        public string caption { get; set; }
        
        /// <summary>
        /// 区域
        /// </summary>
        public string region { get; set; }
        
        /// <summary>
        /// PlcId
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
        /// 是List对象
        /// </summary>
        public bool isList { get; set; }
        
        /// <summary>
        /// List对象数量
        /// </summary>
        public int? listCount { get; set; }
        
        /// <summary>
        /// 值
        /// </summary>
        public string objValue { get; set; }
        
        /// <summary>
        /// 包类型（READ,WRITE,STATUS)
        /// </summary>
        public string packType { get; set; }
        
        /// <summary>
        /// 包大小
        /// </summary>
        public int? packSize { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string descrip { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string plcPointId { get; set; }
        
    }
}