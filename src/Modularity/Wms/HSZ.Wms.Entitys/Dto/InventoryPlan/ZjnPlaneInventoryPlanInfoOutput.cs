using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneInventoryPlan
{
    /// <summary>
    /// 盘点计划输出参数
    /// </summary>
    public class ZjnPlaneInventoryPlanInfoOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 盘点单据号
        /// </summary>
        public string inventoryNo { get; set; }
        
        /// <summary>
        /// 盘点类型
        /// </summary>
        public string inventoryType { get; set; }
        
        /// <summary>
        /// 盘点计划开始时间
        /// </summary>
        public DateTime? startTime { get; set; }
        
        /// <summary>
        /// 盘点计划结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
        
        /// <summary>
        /// 描述（备注）
        /// </summary>
        public string description { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public string enabledMark { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        public string createUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 修改用户
        /// </summary>
        public string lastModifyUserId { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? lastModifyTime { get; set; }
        
    }
}