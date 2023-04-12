using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 同步日志
    /// </summary>
    [SugarTable("hlt_hr_synchronize")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class HltHrSynchronizeEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "n1")]        
        public string N1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "n2")]        
        public string N2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "n3")]        
        public string N3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "n4")]        
        public string N4 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "n5")]        
        public string N5 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "n6")]        
        public string N6 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "n7")]        
        public string N7 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "n8")]        
        public string N8 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "n9")]        
        public string N9 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "n10")]        
        public DateTime? N10 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
    }
}