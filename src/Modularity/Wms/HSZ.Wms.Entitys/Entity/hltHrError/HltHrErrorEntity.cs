using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 错误日志管理
    /// </summary>
    [SugarTable("hlt_hr_error")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class HltHrErrorEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "k1")]        
        public string K1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "k2")]        
        public string K2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "k3")]        
        public string K3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "k4")]        
        public string K4 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "k5")]        
        public string K5 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "k6")]        
        public string K6 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "k7")]        
        public string K7 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "k8")]        
        public DateTime? K8 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
    }
}