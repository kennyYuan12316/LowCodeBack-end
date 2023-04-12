using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsAisle
{
    /// <summary>
    /// 巷道信息列表查询输入
    /// </summary>
    public class ZjnWmsAisleListQueryInput : PageInputBase
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
        /// 巷道编号
        /// </summary>
        public string F_AisleNo { get; set; }
        
        /// <summary>
        /// 巷道名称
        /// </summary>
        public string F_AisleName { get; set; }
        
        /// <summary>
        /// 区域编号
        /// </summary>
        public string F_RegionNo { get; set; }
        
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string F_WarehouseNo { get; set; }
        
        /// <summary>
        /// 堆垛机编号
        /// </summary>
        public string F_StackerNo { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public string F_EnabledMark { get; set; }


        public int? F_IsDelete { get; set; }


    }
}