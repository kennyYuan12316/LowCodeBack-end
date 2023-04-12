using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 映射管理
    /// </summary>
    [SugarTable("hlt_hr_shineupon")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class HltHrShineuponEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "h1")]        
        public string H1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "h2")]        
        public string H2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "h3")]        
        public string H3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "h4")]        
        public string H4 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "h5")]        
        public string H5 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "h6")]        
        public DateTime? H6 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
    }
}