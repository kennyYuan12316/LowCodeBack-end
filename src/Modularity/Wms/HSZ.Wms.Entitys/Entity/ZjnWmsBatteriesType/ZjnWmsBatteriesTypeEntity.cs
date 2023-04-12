using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 电芯种类静置配置
    /// </summary>
    [SugarTable("zjn_wms_BatteriesType")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsBatteriesTypeEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 电池种类
        /// </summary>
        [SugarColumn(ColumnName = "F_BatteriesType")]        
        public int? BatteriesType { get; set; }
        
        /// <summary>
        /// 静置时间（小时）
        /// </summary>
        [SugarColumn(ColumnName = "F_StandingTime")]        
        public int? StandingTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        [SugarColumn(ColumnName = "F_EnabledMark")]        
        public int? EnabledMark { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_Description")]        
        public string Description { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateUser")]        
        public string CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
    }
}