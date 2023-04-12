using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsWarehouse
{
    /// <summary>
    /// 仓库信息列表查询输入
    /// </summary>
    public class ZjnWmsWarehouseListQueryInput : PageInputBase
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
        /// 仓库编号
        /// </summary>
        public string F_WarehouseNo { get; set; }
        
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string F_WarehouseName { get; set; }
        
        /// <summary>
        /// 区域编号
        /// </summary>
        public string F_RegionNo { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public string F_EnabledMark { get; set; }
        
    }
}