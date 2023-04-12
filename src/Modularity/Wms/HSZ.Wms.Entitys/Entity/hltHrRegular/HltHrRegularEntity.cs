using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 定时任务
    /// </summary>
    [SugarTable("hlt_hr_regular")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class HltHrRegularEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "i1")]        
        public string I1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "i2")]        
        public string I2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "i3")]        
        public string I3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "i4")]        
        public string I4 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "i5")]        
        public string I5 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "i6")]        
        public string I6 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
    }
}