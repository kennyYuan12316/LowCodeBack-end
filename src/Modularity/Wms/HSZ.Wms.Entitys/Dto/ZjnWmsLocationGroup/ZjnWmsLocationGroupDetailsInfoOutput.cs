using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsLocationGroup
{
    /// <summary>
    /// 货位分组信息输出参数
    /// </summary>
    public class ZjnWmsLocationGroupDetailsInfoOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 货位分组明细编号
        /// </summary>
        public string groupDetailsNo { get; set; }
        
        /// <summary>
        /// 货位起始行
        /// </summary>
        public int? startRow { get; set; }
        
        /// <summary>
        /// 货位终止行
        /// </summary>
        public int? endRow { get; set; }
        
        /// <summary>
        /// 货位起始列
        /// </summary>
        public int? startCell { get; set; }
        
        /// <summary>
        /// 货位终止列
        /// </summary>
        public int? endCell { get; set; }
        
        /// <summary>
        /// 货位起始层
        /// </summary>
        public int? startLayer { get; set; }
        
        /// <summary>
        /// 货位终止层
        /// </summary>
        public int? endLayer { get; set; }
        
        /// <summary>
        /// 货位起始位深
        /// </summary>
        public int? startDepth { get; set; }
        
        /// <summary>
        /// 货位终止位深
        /// </summary>
        public int? endDepth { get; set; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public int? enabledMark { get; set; }
        
    }
}