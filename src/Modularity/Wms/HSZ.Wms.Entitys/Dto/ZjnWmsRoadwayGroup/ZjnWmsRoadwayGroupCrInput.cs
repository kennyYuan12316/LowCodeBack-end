using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsRoadwayGroup
{
    /// <summary>
    /// 巷道分组信息修改输入参数
    /// </summary>
    public class ZjnWmsRoadwayGroupCrInput
    {
        /// <summary>
        /// 巷道分组编号
        /// </summary>
        public string roadwayNo { get; set; }
        
        /// <summary>
        /// 巷道分组名称
        /// </summary>
        public string roadwayName { get; set; }
        
        /// <summary>
        /// 巷道分组描述
        /// </summary>
        public string description { get; set; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public int? enabledMark { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public List<ZjnWmsRoadwayGroupDetailsCrInput> zjnRoadwayGroupDetailsList { get; set; }
        
    }
}