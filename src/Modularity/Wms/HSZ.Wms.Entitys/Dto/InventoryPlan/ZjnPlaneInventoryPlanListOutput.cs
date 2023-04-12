using System;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneInventoryPlan
{
    /// <summary>
    /// 盘点计划输入参数
    /// </summary>
    public class ZjnPlaneInventoryPlanListOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 盘点单据号
        /// </summary>
        public string F_InventoryNo { get; set; }
        
        /// <summary>
        /// 盘点类型
        /// </summary>
        public string F_InventoryType { get; set; }
        
        /// <summary>
        /// 盘点计划开始时间
        /// </summary>
        public DateTime? F_StartTime { get; set; }
        
        /// <summary>
        /// 盘点计划结束时间
        /// </summary>
        public DateTime? F_EndTime { get; set; }
        
        /// <summary>
        /// 描述（备注）
        /// </summary>
        public string F_Description { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? F_EnabledMark { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 修改用户
        /// </summary>
        public string F_LastModifyUserId { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? F_LastModifyTime { get; set; }
        
    }
}