using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// PLC点位表
    /// </summary>
    [SugarTable("zjn_wcs_plc_point")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWcsPlcPointEntity
    {
        /// <summary>
        /// 有效
        /// </summary>
        [SugarColumn(ColumnName = "IsActive")]        
        public bool IsActive { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "Caption")]        
        public string Caption { get; set; }
        
        /// <summary>
        /// 区域
        /// </summary>
        [SugarColumn(ColumnName = "Region")]        
        public string Region { get; set; }
        
        /// <summary>
        /// PlcId
        /// </summary>
        [SugarColumn(ColumnName = "PlcID")]        
        public string PlcId { get; set; }
        
        /// <summary>
        /// DB
        /// </summary>
        [SugarColumn(ColumnName = "Db")]        
        public int Db { get; set; }
        
        /// <summary>
        /// 起点
        /// </summary>
        [SugarColumn(ColumnName = "Start")]        
        public int Start { get; set; }
        
        /// <summary>
        /// 长度
        /// </summary>
        [SugarColumn(ColumnName = "Length")]        
        public int? Length { get; set; }
        
        /// <summary>
        /// 对象名称，长
        /// </summary>
        [SugarColumn(ColumnName = "ObjType")]        
        public string ObjType { get; set; }
        
        /// <summary>
        /// 是List对象
        /// </summary>
        [SugarColumn(ColumnName = "IsList")]        
        public bool IsList { get; set; }
        
        /// <summary>
        /// List对象数量
        /// </summary>
        [SugarColumn(ColumnName = "ListCount")]        
        public int ListCount { get; set; }
        
        /// <summary>
        /// 值
        /// </summary>
        [SugarColumn(ColumnName = "ObjValue")]        
        public string ObjValue { get; set; }
        
        /// <summary>
        /// 包类型（READ,WRITE,STATUS)
        /// </summary>
        [SugarColumn(ColumnName = "PackType")]        
        public string PackType { get; set; }
        
        /// <summary>
        /// 包大小
        /// </summary>
        [SugarColumn(ColumnName = "PackSize")]        
        public int? PackSize { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "Timestamp")]        
        public DateTime? Timestamp { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "Descrip")]        
        public string Descrip { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "PlcPointID", IsPrimaryKey = true)]
        public string PlcPointId { get; set; }
        
    }
}