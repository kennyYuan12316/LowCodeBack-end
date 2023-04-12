using System;

namespace HSZ.wms.Entitys.Dto.ZjnTaskInterfaceApplyLog
{
    /// <summary>
    /// 接口调用履历表输入参数
    /// </summary>
    public class ZjnTaskInterfaceApplyLogListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 调用完整地址
        /// </summary>
        public string F_Address { get; set; }
        
        /// <summary>
        /// 接口名
        /// </summary>
        public string F_InterfaceName { get; set; }
        
        /// <summary>
        /// 入参
        /// </summary>
        public string F_EnterParameter { get; set; }
        
        /// <summary>
        /// 出参
        /// </summary>
        public string F_OutParameter { get; set; }
        
        /// <summary>
        /// 调用时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 消息
        /// </summary>
        public string F_Msg { get; set; }
        
    }
}