using System;

namespace ZJN.Agv.Entitys.Dto.AgvLimitGoods
{
    /// <summary>
    /// Agv请求取放输入参数
    /// </summary>
    public class ZjnBaseStdLimitgoodsListOutput
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
        /// 任务类型
        /// </summary>
        public int? F_TaskType { get; set; }
        
        /// <summary>
        /// 库位编号
        /// </summary>
        public string F_LocationCode { get; set; }
        
        /// <summary>
        /// 容器编码
        /// </summary>
        public string F_ContainerCode { get; set; }
        
        /// <summary>
        /// 服务地址
        /// </summary>
        public string F_ChannelId { get; set; }
        
        /// <summary>
        /// 请求发送时间
        /// </summary>
        public DateTime? F_RequestTime { get; set; }
        
    }
}