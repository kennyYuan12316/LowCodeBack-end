using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 实时调用信息
    /// </summary>
    [SugarTable("hlt_hr_transfer")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class HltHrTransferEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "l1")]        
        public string L1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "l2")]        
        public string L2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "l3")]        
        public string L3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "l4")]        
        public string L4 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "l5")]        
        public string L5 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "l6")]        
        public string L6 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "l7")]        
        public DateTime? L7 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
    }
}