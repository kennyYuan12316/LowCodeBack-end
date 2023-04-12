using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnHardList
{
    /// <summary>
    /// 设备信息列表查询输入
    /// </summary>
    public class ZjnHardListListQueryInput : PageInputBase
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
        /// 设备编号
        /// </summary>
        public string F_HardNo { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        public string F_HardName { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public string F_Type { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        public string F_Status { get; set; }
        
        /// <summary>
        /// 0真实 1虚拟
        /// </summary>
        public string F_FictitiousHard { get; set; }
        
    }
}