using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseLesGoods
{
    /// <summary>
    /// LES物料原始数据输出参数
    /// </summary>
    public class ZjnBaseLesGoodsInfoOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 物料编号
        /// </summary>
        public string code { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string xName { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        public string xType { get; set; }
        
        /// <summary>
        /// 基本单位
        /// </summary>
        public string defaultUnit { get; set; }
        
        /// <summary>
        /// 总有效期(天）
        /// </summary>
        public int? validDays { get; set; }
        
        /// <summary>
        /// 静置时间
        /// </summary>
        public int? stayHours { get; set; }
        
        /// <summary>
        /// 是否批次管理 1:是；0：否
        /// </summary>
        public int? batchManageFlag { get; set; }
        
        /// <summary>
        /// 规格型号
        /// </summary>
        public string specification { get; set; }
        
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
    }
}