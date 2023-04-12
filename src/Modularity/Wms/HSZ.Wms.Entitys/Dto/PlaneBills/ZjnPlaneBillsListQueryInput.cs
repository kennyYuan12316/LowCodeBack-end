using HSZ.Common.Filter;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneBills
{
    /// <summary>
    /// 出入库单据列表查询输入
    /// </summary>
    public class ZjnPlaneBillsListQueryInput : PageInputBase
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
        /// 单据号码
        /// </summary>
        public string F_BillNo { get; set; }
        
        /// <summary>
        /// 单据类型
        /// </summary>
        public string F_BillType { get; set; }
        
        /// <summary>
        /// 单据状态
        /// </summary>
        public string F_BillState { get; set; }

        /// <summary>
        /// 仓库位置
        /// </summary>
        public string F_BillWarehouse { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public string F_EnabledMark { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? startTime { get; set; }


        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? endTime { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateTime { get; set; }


        


    }
}