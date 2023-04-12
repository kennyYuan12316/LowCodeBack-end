using System;

namespace HSZ.wms.Entitys.Dto.ZjnWmsLocationGroup
{
    /// <summary>
    /// 货位分组信息输入参数
    /// </summary>
    public class ZjnWmsLocationGroupListOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 货位分组编号
        /// </summary>
        public string F_GroupNo { get; set; }
        
        /// <summary>
        /// 货位分组编号
        /// </summary>
        public string F_GroupName { get; set; }
        
        /// <summary>
        /// 货位分组描述
        /// </summary>
        public string F_Description { get; set; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public string F_EnabledMark { get; set; }
        
    }
}