using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 同步数据类型
    /// </summary>
    [SugarTable("hlt_hr_t1")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class HltHrT1Entity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "t1")]        
        public string T1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "t2")]        
        public string T2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "t3")]        
        public string T3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "t4")]        
        public string T4 { get; set; }
        
    }
}