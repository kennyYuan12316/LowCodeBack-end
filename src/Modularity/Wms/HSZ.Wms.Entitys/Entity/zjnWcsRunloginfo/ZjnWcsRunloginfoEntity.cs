using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 运行日志
    /// </summary>
    [SugarTable("zjn_wcs_runloginfo")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWcsRunloginfoEntity
    {
        /// <summary>
        /// (Event)事件日志表ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 托盘条码1
        /// </summary>
        [SugarColumn(ColumnName = "F_ContainerBarcode1")]        
        public string ContainerBarcode1 { get; set; }
        
        /// <summary>
        /// 托盘条码2
        /// </summary>
        [SugarColumn(ColumnName = "F_ContainerBarcode2")]        
        public string ContainerBarcode2 { get; set; }
        
        /// <summary>
        /// 任务号1
        /// </summary>
        [SugarColumn(ColumnName = "F_TaskCode1")]        
        public string TaskCode1 { get; set; }
        
        /// <summary>
        /// 任务号2
        /// </summary>
        [SugarColumn(ColumnName = "F_TaskCode2")]        
        public string TaskCode2 { get; set; }
        
        /// <summary>
        /// 设备号
        /// </summary>
        [SugarColumn(ColumnName = "F_EquipmentCode")]        
        public string EquipmentCode { get; set; }
        
        /// <summary>
        /// 日志信息
        /// </summary>
        [SugarColumn(ColumnName = "F_RunLog")]        
        public string RunLog { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        [SugarColumn(ColumnName = "F_RunType")]        
        public string RunType { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
    }
}