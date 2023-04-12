using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 演示12
    /// </summary>
    [SugarTable("Table_T1")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class TableT1Entity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_T1")]        
        public string T1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_T2")]        
        public string T2 { get; set; }
        
    }
}