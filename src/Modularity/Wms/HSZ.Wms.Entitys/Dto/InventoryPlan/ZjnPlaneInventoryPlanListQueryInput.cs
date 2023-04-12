using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneInventoryPlan
{
    /// <summary>
    /// 盘点计划列表查询输入
    /// </summary>
    public class ZjnPlaneInventoryPlanListQueryInput : PageInputBase
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
        /// 盘点单据号
        /// </summary>
        public string F_InventoryNo { get; set; }
        
        /// <summary>
        /// 盘点类型
        /// </summary>
        public string F_InventoryType { get; set; }
        
        /// <summary>
        /// 盘点计划开始时间
        /// </summary>
        public string F_StartTime { get; set; }
        
        /// <summary>
        /// 盘点计划结束时间
        /// </summary>
        public string F_EndTime { get; set; }
        
    }
}