using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 平面库操作日志信息
    /// </summary>
    [SugarTable("zjn_wms_OperationLog")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsOperationLogEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 操作类型 1.修改 2.删除
        /// </summary>
        [SugarColumn(ColumnName = "F_Type")]        
        public int? Type { get; set; }
        
        /// <summary>
        /// 操作描述（描述在哪个业务执行）
        /// </summary>
        [SugarColumn(ColumnName = "F_Describe")]        
        public string Describe { get; set; }
        
        /// <summary>
        /// 操作路径 1.APP 2.WEB
        /// </summary>
        [SugarColumn(ColumnName = "F_WorkPath")]        
        public int? WorkPath { get; set; }
        
        /// <summary>
        /// 操作前数据
        /// </summary>
        [SugarColumn(ColumnName = "F_BeforeDate")]        
        public string BeforeDate { get; set; }
        
        /// <summary>
        /// 操作后数据
        /// </summary>
        [SugarColumn(ColumnName = "F_AfterDate")]        
        public string AfterDate { get; set; }
        
        /// <summary>
        /// 操作人
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateUser")]        
        public string CreateUser { get; set; }
        
        /// <summary>
        /// 操作时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 业务区域字段（仓库名）
        /// </summary>
        [SugarColumn(ColumnName = "F_TheBusinessArea")]        
        public string TheBusinessArea { get; set; }
        

        /// <summary>
        /// 备用字段
        /// </summary>
        [SugarColumn(ColumnName = "case2")]        
        public string Case2 { get; set; }
        
        /// <summary>
        /// 备用字段
        /// </summary>
        [SugarColumn(ColumnName = "case3")]        
        public string Case3 { get; set; }
        
        /// <summary>
        /// 备用字段
        /// </summary>
        [SugarColumn(ColumnName = "case4")]        
        public string Case4 { get; set; }
        
        /// <summary>
        /// 备用字段
        /// </summary>
        [SugarColumn(ColumnName = "case5")]        
        public string Case5 { get; set; }
        
    }
}