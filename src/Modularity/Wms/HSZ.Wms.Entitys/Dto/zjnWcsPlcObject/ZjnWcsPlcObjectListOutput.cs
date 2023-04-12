using System;

namespace HSZ.wms.Entitys.Dto.ZjnWcsPlcObject
{
    /// <summary>
    /// PLC对象表输入参数
    /// </summary>
    public class ZjnWcsPlcObjectListOutput
    {
        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }
        
        /// <summary>
        /// Plc点位
        /// </summary>
        public string PlcPointID { get; set; }
        
        /// <summary>
        /// Plc设备
        /// </summary>
        public string PlcID { get; set; }
        
        /// <summary>
        /// DB
        /// </summary>
        public int? Db { get; set; }
        
        /// <summary>
        /// 起点
        /// </summary>
        public int? Start { get; set; }
        
        /// <summary>
        /// 长度
        /// </summary>
        public int? Length { get; set; }
        
        /// <summary>
        /// 对象名称，长
        /// </summary>
        public string ObjType { get; set; }
        
        /// <summary>
        /// 对象，json
        /// </summary>
        public string ObjValue { get; set; }
        
        /// <summary>
        /// READ,WRITE,STATUS
        /// </summary>
        public string PackType { get; set; }
        
        /// <summary>
        /// Plc包大小
        /// </summary>
        public int? PackSize { get; set; }
        
        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceID { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string Descrip { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string PlcObjID { get; set; }

        public string StackerGroup { get; set; }


    }
}