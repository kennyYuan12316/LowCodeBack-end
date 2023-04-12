using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnTaskInterface
{
    /// <summary>
    /// 接口配置列表查询输入
    /// </summary>
    public class ZjnTaskInterfaceListQueryInput : PageInputBase
    {
        /// <summary>
        /// 选择导出数据key
        /// </summary>
        public string selectKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int dataType { get; set; }

        /// <summary>
        /// 接口名称
        /// </summary>
        public string F_NameInterface { get; set; }
        
        /// <summary>
        /// 通讯协议
        /// </summary>
        public string F_Communication { get; set; }
        
        /// <summary>
        /// 接口提供者
        /// </summary>
        public string F_InterfaceProvide { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public int F_EnabledMark { get; set; }


    }
}