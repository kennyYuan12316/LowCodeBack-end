using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsLocation
{
    /// <summary>
    /// 货位信息列表查询输入
    /// </summary>
    public class ZjnWmsLocationListQueryInput : PageInputBase
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
        /// 货位编号
        /// </summary>
        public string F_LocationNo { get; set; }
        
        /// <summary>
        /// 巷道编号
        /// </summary>
        public string F_AisleNo { get; set; }
        
        /// <summary>
        /// 行
        /// </summary>
        public string F_Row { get; set; }
        
        /// <summary>
        /// 列
        /// </summary>
        public string F_Cell { get; set; }
        
        /// <summary>
        /// 层
        /// </summary>
        public string F_Layer { get; set; }
        
        /// <summary>
        /// 货位状态：0空 1满 2未满 3故障 4火警 5静置中 6静置完成 7预约 8禁用
        /// </summary>
        public string F_LocationStatus { get; set; }
        
    }
}