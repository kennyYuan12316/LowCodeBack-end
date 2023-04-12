using System;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneBills
{
    /// <summary>
    /// 出入库单据输入参数
    /// </summary>
    public class ZjnPlaneBillsListOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string F_Id { get; set; }
        
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
        public int? F_EnabledMark { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? F_CreateTime { get; set; }


    }
}