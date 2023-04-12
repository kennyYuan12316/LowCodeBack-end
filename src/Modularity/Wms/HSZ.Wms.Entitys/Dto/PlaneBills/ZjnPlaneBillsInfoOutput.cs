using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneBills
{
    /// <summary>
    /// 出入库单据输出参数
    /// </summary>
    public class ZjnPlaneBillsInfoOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 单据号码
        /// </summary>
        public string billNo { get; set; }
        
        /// <summary>
        /// 单据类型
        /// </summary>
        public string billType { get; set; }

        /// <summary>
        /// 仓库位置 
        /// </summary>
        public string billWarehouse { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        public string billState { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public string enabledMark { get; set; }
        
        /// <summary>
        /// 平面库单据子表--出入库单
        /// </summary>
        public List<ZjnPlaneBillsInOutOrderInfoOutput> zjnPlaneBillsInOutOrderList { get; set; }
        
    }
}