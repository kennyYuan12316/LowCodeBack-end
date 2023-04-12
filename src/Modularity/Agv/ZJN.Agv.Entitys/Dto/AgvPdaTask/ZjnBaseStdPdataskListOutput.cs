using System;

namespace ZJN.Agv.Entitys.Dto.AgvPdaTask
{
    /// <summary>
    /// Agv上传PDA任务输入参数
    /// </summary>
    public class ZjnBaseStdPdataskListOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 数据唯一标识
   
        /// </summary>
        public string F_RequestId { get; set; }
        
        /// <summary>
        /// 系统标识
        /// </summary>
        public string F_ClientCode { get; set; }
        
        /// <summary>
        /// 服务地址
   
        /// </summary>
        public string F_ChannelId { get; set; }
        
        /// <summary>
        /// 请求发送时间
   
        /// </summary>
        public DateTime? F_RequestTime { get; set; }
        
        /// <summary>
        /// 任务类型
   
        /// </summary>
        public int? F_TaskType { get; set; }
        
        /// <summary>
        /// 起点库位编码
   
        /// </summary>
        public string F_StartAreaCode { get; set; }
        
        /// <summary>
        /// 起点库区编码
   
        /// </summary>
        public string F_StartLocCode { get; set; }
        
        /// <summary>
        /// 终点库区编码
   
        /// </summary>
        public string F_EndAreaCode { get; set; }
        
        /// <summary>
        /// 终点库位编码
   
        /// </summary>
        public string F_EndLocCode { get; set; }
        
        /// <summary>
        /// 物料类型编号
   
        /// </summary>
        public string F_GoodsCode { get; set; }
        
        /// <summary>
        /// 容器编码
   
        /// </summary>
        public string F_ContainerCode { get; set; }
        
        /// <summary>
        /// 数量
   
        /// </summary>
        public int? F_Quantity { get; set; }
        
    }
}