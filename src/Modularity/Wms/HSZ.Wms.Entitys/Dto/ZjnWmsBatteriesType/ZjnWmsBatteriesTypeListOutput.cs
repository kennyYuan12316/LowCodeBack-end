using System;

namespace HSZ.wms.Entitys.Dto.ZjnWmsBatteriesType
{
    /// <summary>
    /// 电芯种类静置配置输入参数
    /// </summary>
    public class ZjnWmsBatteriesTypeListOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 电池种类
        /// </summary>
        public int? F_BatteriesType { get; set; }
        
        /// <summary>
        /// 静置时间（小时）
        /// </summary>
        public int? F_StandingTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public string F_EnabledMark { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string F_Description { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
    }
}