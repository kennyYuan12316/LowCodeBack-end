using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneBills
{
    /// <summary>
    /// 出入库单据修改输入参数
    /// </summary>
    public class ZjnPlaneBillsCrInput
    {
        /// <summary>
        /// 单据号码
        /// </summary>
        public string billNo { get; set; }

        /// <summary>
        /// 主单ID
        /// </summary>
        public string parentId { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        public string billType { get; set; }
        
        /// <summary>
        /// 单据状态
        /// </summary>
        public string billState { get; set; }

        /// <summary>
        /// 仓库位置 
        /// </summary>
        public string billWarehouse { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public string enabledMark { get; set; }
        
        /// <summary>
        /// 平面库单据子表--出入库单
        /// </summary>
        public List<ZjnPlaneBillsInOutOrderCrInput> zjnPlaneBillsInOutOrderList { get; set; }
        
    }
}