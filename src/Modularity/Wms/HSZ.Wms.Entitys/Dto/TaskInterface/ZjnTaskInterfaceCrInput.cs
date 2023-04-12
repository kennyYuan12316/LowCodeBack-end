using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnTaskInterface
{
    /// <summary>
    /// 接口配置修改输入参数
    /// </summary>
    public class ZjnTaskInterfaceCrInput
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string nameInterface { get; set; }
        
        /// <summary>
        /// 中文
        /// </summary>
        public string cnInterface { get; set; }
        
        /// <summary>
        /// 入参
        /// </summary>
        public string enterParameter { get; set; }
        
        /// <summary>
        /// 出参
        /// </summary>
        public string outParameter { get; set; }
        
        /// <summary>
        /// 通讯协议
        /// </summary>
        public string communication { get; set; }
        
        /// <summary>
        /// 说明
        /// </summary>
        public string introduction { get; set; }
        
        /// <summary>
        /// 接口提供者
        /// </summary>
        public string interfaceProvide { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        public string createUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? enabledMark { get; set; }
        
    }
}