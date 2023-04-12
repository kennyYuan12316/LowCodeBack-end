using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 测试22222
    /// </summary>
    [SugarTable("hlt_t")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class HltTEntity
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
        
    }
}