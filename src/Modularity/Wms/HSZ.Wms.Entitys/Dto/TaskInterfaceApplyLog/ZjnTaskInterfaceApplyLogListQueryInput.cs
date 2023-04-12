using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnTaskInterfaceApplyLog
{
    /// <summary>
    /// 接口调用履历表列表查询输入
    /// </summary>
    public class ZjnTaskInterfaceApplyLogListQueryInput : PageInputBase
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
        /// 接口名
        /// </summary>
        public string F_InterfaceName { get; set; }
        
        /// <summary>
        /// 调用时间
        /// </summary>
        public string F_CreateTime { get; set; }
        
    }
}