using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// test_001
    /// </summary>
    [SugarTable("test _01")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class Test01Entity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "Name")]        
        public string Name { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "age")]        
        public int Age { get; set; }
        
    }
}