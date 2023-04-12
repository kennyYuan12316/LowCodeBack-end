using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 部门信息
    /// </summary>
    [SugarTable("hlt_hr_department")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class HltHrDepartmentEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "b1")]        
        public string B1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "b2")]        
        public string B2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "b3")]        
        public string B3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
    }
}