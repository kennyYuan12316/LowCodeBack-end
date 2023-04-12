using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 岗位模板管理
    /// </summary>
    [SugarTable("hlt_hr_post")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class HltHrPostEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "c1")]        
        public string C1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "c2")]        
        public string C2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "c3")]        
        public string C3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
    }
}