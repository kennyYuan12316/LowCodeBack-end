using System;

namespace ZJN.Agv.Entitys.Dto.AgvCancelOrder
{
    /// <summary>
    /// 立库取消订单输入参数
    /// </summary>
    public class ZjnBaseStdCancelorderListOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 外部订单ID
        /// </summary>
        public string F_OuterOrderId { get; set; }
        
    }
}