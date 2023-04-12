using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsLocationGroup
{
    /// <summary>
    /// 货位分组信息修改输入参数
    /// </summary>
    public class ZjnWmsLocationGroupCrInput
    {
        /// <summary>
        /// 货位分组编号
        /// </summary>
        public string groupNo { get; set; }
        
        /// <summary>
        /// 货位分组编号
        /// </summary>
        public string groupName { get; set; }
        
        /// <summary>
        /// 货位分组描述
        /// </summary>
        public string description { get; set; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public int? enabledMark { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public List<ZjnWmsLocationGroupDetailsCrInput> zjnLocationGroupDetailsList { get; set; }
        
    }
}