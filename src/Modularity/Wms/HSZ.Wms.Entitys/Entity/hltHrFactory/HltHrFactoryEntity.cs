using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 厂区管理
    /// </summary>
    [SugarTable("hlt_hr_factory")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class HltHrFactoryEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "e1")]        
        public string E1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "e2")]        
        public string E2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "e3")]        
        public string E3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
    }
}