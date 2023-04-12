using System;

namespace HSZ.wms.Entitys.Dto.ZjnBaseLesGoods
{
    /// <summary>
    /// LES物料原始数据输入参数
    /// </summary>
    public class ZjnBaseLesGoodsListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 物料编号
        /// </summary>
        public string F_Code { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string F_xName { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        public string F_xType { get; set; }
        
        /// <summary>
        /// 基本单位
        /// </summary>
        public string F_DefaultUnit { get; set; }
        
        /// <summary>
        /// 总有效期(天）
        /// </summary>
        public int? F_ValidDays { get; set; }
        
        /// <summary>
        /// 静置时间
        /// </summary>
        public int? F_StayHours { get; set; }
        
        /// <summary>
        /// 是否批次管理 1:是；0：否
        /// </summary>
        public int? F_BatchManageFlag { get; set; }
        
        /// <summary>
        /// 规格型号
        /// </summary>
        public string F_Specification { get; set; }
        
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
    }
}