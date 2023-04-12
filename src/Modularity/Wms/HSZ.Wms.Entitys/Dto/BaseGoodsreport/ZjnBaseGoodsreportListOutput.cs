using System;

namespace HSZ.wms.Entitys.Dto.ZjnBaseGoodsreport
{
    /// <summary>
    /// 物料列表输入参数
    /// </summary>
    public class ZjnBaseGoodsreportListOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string F_Description { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        public string F_ProductsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string F_ProductsName { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        public string F_ProductsType { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string F_ProductsUnit { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        public string F_Supplier { get; set; }
        
        /// <summary>
        /// 保质期
        /// </summary>
        public string F_ExpirationDate { get; set; }
        
        /// <summary>
        /// 预警周期
        /// </summary>
        public string F_WarningCycle { get; set; }
        
    }
}