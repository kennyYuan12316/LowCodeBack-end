using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnReportProductOut
{
    /// <summary>
    /// 成品出库单输出参数
    /// </summary>
    public class ZjnReportProductOutInfoOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 所属托盘
        /// </summary>
        public string tray { get; set; }
        
        /// <summary>
        /// 电芯条码
        /// </summary>
        public string batteryCode { get; set; }
        
        /// <summary>
        /// 生产单号
        /// </summary>
        public string productionOrder { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        public string productsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string productsName { get; set; }
        
        /// <summary>
        /// 生产时间
        /// </summary>
        public DateTime? productionTime { get; set; }
        
        /// <summary>
        /// 出库单
        /// </summary>
        public string outOrder { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public string businessType { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        public string batch { get; set; }
        
        /// <summary>
        /// 出库时间
        /// </summary>
        public DateTime? outTime { get; set; }
        
        /// <summary>
        /// 出库时间确认
        /// </summary>
        public DateTime? outTimeConfirm { get; set; }
        
        /// <summary>
        /// 出库站台
        /// </summary>
        public string outStation { get; set; }
        
        /// <summary>
        /// 出库数量
        /// </summary>
        public int? outQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string productsUnit { get; set; }
        
        /// <summary>
        /// 产品标识
        /// </summary>
        public string productionMark { get; set; }
        
        /// <summary>
        /// 库存标识
        /// </summary>
        public string inventoryMark { get; set; }
        
        /// <summary>
        /// 等级标识
        /// </summary>
        public string classMark { get; set; }
        
    }
}