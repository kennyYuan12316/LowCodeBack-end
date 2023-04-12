using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneBills
{
    /// <summary>
    /// 出入库单据修改输入参数
    /// </summary>
    public class ZjnPlaneBillsInOutOrderCrInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 单据号
        /// </summary>
        public string ordeerNo { get; set; }
        
        /// <summary>
        /// 货物ID
        /// </summary>
        public string productsNo { get; set; }

        /// <summary>
        /// 货物名称
        /// </summary>
        public string productsName { get; set; }
        
        /// <summary>
        /// 货物总数
        /// </summary>
        public decimal? productsTotal { get; set; }

        /// <summary>
        /// 差异数量 （总数 - 完成数）
        /// </summary>
        public decimal? productsDifference { get; set; }

        /// <summary>
        /// 完成数量
        /// </summary>
        public decimal? submitCount { get; set; }
        
        /// <summary>
        /// 货物单位
        /// </summary>
        public string productsUnit { get; set; }
        
        /// <summary>
        /// 货物批次
        /// </summary>
        public string productsBach { get; set; }
        
        /// <summary>
        /// 货物类型
        /// </summary>
        public string productsType { get; set; }
        
        /// <summary>
        /// 货物规格
        /// </summary>
        public string productsStyle { get; set; }
        
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? productsDate { get; set; }
        
        /// <summary>
        /// 检验类型
        /// </summary>
        public string productsCheckType { get; set; }
        
        /// <summary>
        /// 货物等级
        /// </summary>
        public string productsGrade { get; set; }
        
        /// <summary>
        /// 客户编号
        /// </summary>
        public string productsUser { get; set; }
        
        /// <summary>
        /// 供应商编号
        /// </summary>
        public string productsSupplier { get; set; }
        
        /// <summary>
        /// 货位编号
        /// </summary>
        public string productsLocation { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string productsTray { get; set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public string orderState { get; set; }

        /// <summary>
        /// 货物备注
        /// </summary>
        public string productsRemarks { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? LastModifyTime { get; set; }

        /// <summary>
        /// 修改用户
        /// </summary>
        public string LastModifyUserId { get; set; }


}
}