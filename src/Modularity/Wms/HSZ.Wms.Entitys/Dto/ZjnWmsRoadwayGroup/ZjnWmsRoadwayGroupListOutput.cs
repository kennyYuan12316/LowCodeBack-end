using System;

namespace HSZ.wms.Entitys.Dto.ZjnWmsRoadwayGroup
{
    /// <summary>
    /// 巷道分组信息输入参数
    /// </summary>
    public class ZjnWmsRoadwayGroupListOutput
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 巷道分组编号
        /// </summary>
        public string F_roadwayNo { get; set; }
        
        /// <summary>
        /// 巷道分组名称
        /// </summary>
        public string F_roadwayName { get; set; }
        
        /// <summary>
        /// 巷道分组描述
        /// </summary>
        public string F_Description { get; set; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public string F_EnabledMark { get; set; }
        
    }
}