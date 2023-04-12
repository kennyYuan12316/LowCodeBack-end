using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 在职状态管理
    /// </summary>
    [SugarTable("hlt_hr_job")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class HltHrJobEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "f1")]        
        public string F1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "f2")]        
        public string F2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "f3")]        
        public string F3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
    }
}