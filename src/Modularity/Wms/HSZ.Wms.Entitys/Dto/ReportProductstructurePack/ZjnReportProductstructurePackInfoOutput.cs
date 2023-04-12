using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnReportProductstructurePack
{
    /// <summary>
    /// 库存结构分析（集成）输出参数
    /// </summary>
    public class ZjnReportProductstructurePackInfoOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        public string productsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string productsName { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        public string batch { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? entryTime { get; set; }
        
        /// <summary>
        /// 物料数量
        /// </summary>
        public int? productsQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string productsUnit { get; set; }
        
        /// <summary>
        /// 库龄
        /// </summary>
        public string locationAge { get; set; }
        
        /// <summary>
        /// 集成方式
        /// </summary>
        public string combineType { get; set; }
        
        /// <summary>
        /// 项目号
        /// </summary>
        public string projectNum { get; set; }
        
        /// <summary>
        /// 包装方式
        /// </summary>
        public string packingType { get; set; }
        
        /// <summary>
        /// 电池型号
        /// </summary>
        public string batteryType { get; set; }
        
        /// <summary>
        /// 电池数量
        /// </summary>
        public int? batteryCount { get; set; }
        
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
        /// 电池容量（GWh）
        /// </summary>
        public double batteryCapacity { get; set; }
        
        /// <summary>
        /// 生产时间
        /// </summary>
        public DateTime? productionTime { get; set; }
        
        /// <summary>
        /// 生产线
        /// </summary>
        public string lineNum { get; set; }
        
        /// <summary>
        /// 生产线状态
        /// </summary>
        public string lineStatus { get; set; }
        
        /// <summary>
        /// 位置
        /// </summary>
        public string location { get; set; }
        
        /// <summary>
        /// 是否冻结
        /// </summary>
        public string freeze { get; set; }
        
        /// <summary>
        /// 冻结原因
        /// </summary>
        public string freezeResult { get; set; }
        
    }
}