using System;

namespace HSZ.wms.Entitys.Dto.ZjnWcsPlcPoint
{
    /// <summary>
    /// PLC点位表输入参数
    /// </summary>
    public class ZjnWcsPlcPointListOutput
    {
        /// <summary>
        /// 有效
        /// </summary>
        public string IsActive { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        public string Caption { get; set; }
        
        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }
        
        /// <summary>
        /// PlcId
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
        /// 是List对象
        /// </summary>
        public string IsList { get; set; }
        
        /// <summary>
        /// List对象数量
        /// </summary>
        public int? ListCount { get; set; }
        
        /// <summary>
        /// 值
        /// </summary>
        public string ObjValue { get; set; }
        
        /// <summary>
        /// 包类型（READ,WRITE,STATUS)
        /// </summary>
        public string PackType { get; set; }
        
        /// <summary>
        /// 包大小
        /// </summary>
        public int? PackSize { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Descrip { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string PlcPointID { get; set; }
        
    }
}