using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnReportProductEntry
{
    /// <summary>
    /// 成品入库单修改输入参数
    /// </summary>
    public class ZjnReportProductEntryCrInput
    {
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
        /// 入库单
        /// </summary>
        public string entryOrder { get; set; }
        
        /// <summary>
        /// 组盘时间
        /// </summary>
        public DateTime? comboTime { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? entryTime { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public string businessType { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        public string batch { get; set; }
        
        /// <summary>
        /// 物料数量
        /// </summary>
        public int? productsQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string productsUnit { get; set; }
        
        /// <summary>
        /// 质量状态
        /// </summary>
        public string quality { get; set; }
        
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
        
        /// <summary>
        /// 电压
        /// </summary>
        public double voltage { get; set; }
        
        /// <summary>
        /// 安时
        /// </summary>
        public double ah { get; set; }
        
        /// <summary>
        /// 交流电阻
        /// </summary>
        public double acr { get; set; }
        
        /// <summary>
        /// 直流电阻
        /// </summary>
        public double dcr { get; set; }
        
        /// <summary>
        /// K值
        /// </summary>
        public double kValue { get; set; }
        
        /// <summary>
        /// 生产线
        /// </summary>
        public string lineNum { get; set; }
        
        /// <summary>
        /// 通道号
        /// </summary>
        public int? channelNum { get; set; }
        
    }
}