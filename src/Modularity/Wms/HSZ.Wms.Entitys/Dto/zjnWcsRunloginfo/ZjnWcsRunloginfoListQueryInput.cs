using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWcsRunloginfo
{
    /// <summary>
    /// 运行日志列表查询输入
    /// </summary>
    public class ZjnWcsRunloginfoListQueryInput : PageInputBase
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
        /// 设备号
        /// </summary>
        public string F_EquipmentCode { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public string F_RunType { get; set; }
        
    }
}