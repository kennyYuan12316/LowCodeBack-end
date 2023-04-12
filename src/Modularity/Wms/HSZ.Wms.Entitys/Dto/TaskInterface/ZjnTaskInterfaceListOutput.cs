using System;

namespace HSZ.wms.Entitys.Dto.ZjnTaskInterface
{
    /// <summary>
    /// 接口配置输入参数
    /// </summary>
    public class ZjnTaskInterfaceListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 接口名称
        /// </summary>
        public string F_NameInterface { get; set; }
        
        /// <summary>
        /// 中文
        /// </summary>
        public string F_CnInterface { get; set; }
        
        /// <summary>
        /// 入参
        /// </summary>
        public string F_EnterParameter { get; set; }
        
        /// <summary>
        /// 出参
        /// </summary>
        public string F_OutParameter { get; set; }
        
        /// <summary>
        /// 通讯协议
        /// </summary>
        public string F_Communication { get; set; }
        
        /// <summary>
        /// 说明
        /// </summary>
        public string F_Introduction { get; set; }
        
        /// <summary>
        /// 接口提供者
        /// </summary>
        public string F_InterfaceProvide { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public string F_EnabledMark { get; set; }
        
    }
}