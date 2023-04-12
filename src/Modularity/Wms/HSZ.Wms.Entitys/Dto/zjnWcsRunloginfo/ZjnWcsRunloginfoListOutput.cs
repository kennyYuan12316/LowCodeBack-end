using System;

namespace HSZ.wms.Entitys.Dto.ZjnWcsRunloginfo
{
    /// <summary>
    /// 运行日志输入参数
    /// </summary>
    public class ZjnWcsRunloginfoListOutput
    {
        /// <summary>
        /// (Event)事件日志表ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 托盘条码1
        /// </summary>
        public string F_ContainerBarcode1 { get; set; }
        
        /// <summary>
        /// 托盘条码2
        /// </summary>
        public string F_ContainerBarcode2 { get; set; }
        
        /// <summary>
        /// 任务号1
        /// </summary>
        public string F_TaskCode1 { get; set; }
        
        /// <summary>
        /// 任务号2
        /// </summary>
        public string F_TaskCode2 { get; set; }
        
        /// <summary>
        /// 设备号
        /// </summary>
        public string F_EquipmentCode { get; set; }
        
        /// <summary>
        /// 日志信息
        /// </summary>
        public string F_RunLog { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public string F_RunType { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
    }
}