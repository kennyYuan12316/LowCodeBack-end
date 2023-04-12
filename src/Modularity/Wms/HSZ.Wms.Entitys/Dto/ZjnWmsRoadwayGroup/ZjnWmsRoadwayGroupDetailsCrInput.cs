using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsRoadwayGroup
{
    /// <summary>
    /// 巷道分组信息修改输入参数
    /// </summary>
    public class ZjnWmsRoadwayGroupDetailsCrInput
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 巷道编号
        /// </summary>
        public string roadwayDetailsNo { get; set; }
        
        /// <summary>
        /// 巷道名称
        /// </summary>
        public string roadwayDetailsName { get; set; }
        
        /// <summary>
        /// 巷道排序
        /// </summary>
        public int? roadwayDetailsGrade { get; set; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public int? enabledMark { get; set; }
        
    }
}