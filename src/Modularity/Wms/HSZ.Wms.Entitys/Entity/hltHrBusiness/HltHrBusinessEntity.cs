using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 业务模板组管理
    /// </summary>
    [SugarTable("hlt_hr_business")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class HltHrBusinessEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "d1")]        
        public string D1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "d2")]        
        public string D2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "d3")]        
        public string D3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "d4")]        
        public string D4 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "d5")]        
        public string D5 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
    }
}