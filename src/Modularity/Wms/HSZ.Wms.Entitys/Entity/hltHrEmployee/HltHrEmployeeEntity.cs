using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 员工信息
    /// </summary>
    [SugarTable("hlt_hr_employee")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class HltHrEmployeeEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "a1")]        
        public string A1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "a2")]        
        public string A2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "a3")]        
        public string A3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "a4")]        
        public string A4 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "a5")]        
        public string A5 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "a6")]        
        public string A6 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "a7")]        
        public string A7 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "a8")]        
        public string A8 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "a9")]        
        public string A9 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "a10")]        
        public DateTime? A10 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "aid", IsPrimaryKey = true)]
        public string Id { get; set; }
        
    }
}