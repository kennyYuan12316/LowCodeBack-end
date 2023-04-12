using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// wms运行日志
    /// </summary>
    [SugarTable("zjn_wms_run_loginfo")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsRunLoginfoEntity
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 业务类型（函数注释）
        /// </summary>
        [SugarColumn(ColumnName = "F_TaskType")]        
        public string TaskType { get; set; }
        
        /// <summary>
        /// 方法名
        /// </summary>
        [SugarColumn(ColumnName = "F_MethodName")]        
        public string MethodName { get; set; }
        
        /// <summary>
        /// 方法参数
        /// </summary>
        [SugarColumn(ColumnName = "F_MethodParmes")]        
        public string MethodParmes { get; set; }
        
        /// <summary>
        /// 任务号
        /// </summary>
        [SugarColumn(ColumnName = "F_TaskNo")]        
        public string TaskNo { get; set; }
        
        /// <summary>
        /// 设备号
        /// </summary>
        [SugarColumn(ColumnName = "F_DeviceNo")]        
        public string DeviceNo { get; set; }
        
        /// <summary>
        /// 托盘号
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayNo")]        
        public string TrayNo { get; set; }
        
        /// <summary>
        /// 是否报错：0否  1是
        /// </summary>
        [SugarColumn(ColumnName = "F_IsBug")]        
        public int? IsBug { get; set; }
        
        /// <summary>
        /// 报错信息
        /// </summary>
        [SugarColumn(ColumnName = "F_Message")]        
        public string Message { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Case1")]        
        public string Case1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Case2")]        
        public string Case2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Case3")]        
        public string Case3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Case4")]        
        public string Case4 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Case5")]        
        public string Case5 { get; set; }
        
    }
}