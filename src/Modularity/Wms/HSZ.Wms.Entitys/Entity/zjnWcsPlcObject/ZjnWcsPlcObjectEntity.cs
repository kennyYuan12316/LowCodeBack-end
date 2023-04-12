using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// PLC对象表
    /// </summary>
    [SugarTable("zjn_wcs_plc_object")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWcsPlcObjectEntity
    {
        /// <summary>
        /// 区域
        /// </summary>
        [SugarColumn(ColumnName = "Region")]        
        public string Region { get; set; }
        
        /// <summary>
        /// Plc点位
        /// </summary>
        [SugarColumn(ColumnName = "PlcPointID")]        
        public string PlcPointId { get; set; }
        
        /// <summary>
        /// Plc设备
        /// </summary>
        [SugarColumn(ColumnName = "PlcID")]        
        public string PlcId { get; set; }
        
        /// <summary>
        /// DB
        /// </summary>
        [SugarColumn(ColumnName = "Db")]        
        public int? Db { get; set; }
        
        /// <summary>
        /// 起点
        /// </summary>
        [SugarColumn(ColumnName = "Start")]        
        public int? Start { get; set; }
        
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
        /// 对象，json
        /// </summary>
        [SugarColumn(ColumnName = "ObjValue")]        
        public string ObjValue { get; set; }
        
        /// <summary>
        /// READ,WRITE,STATUS
        /// </summary>
        [SugarColumn(ColumnName = "PackType")]        
        public string PackType { get; set; }
        
        /// <summary>
        /// Plc包大小
        /// </summary>
        [SugarColumn(ColumnName = "PackSize")]        
        public int? PackSize { get; set; }
        
        /// <summary>
        /// 设备ID
        /// </summary>
        [SugarColumn(ColumnName = "DeviceID")]        
        public string DeviceId { get; set; }
        
        /// <summary>
        /// 时间
        /// </summary>
        [SugarColumn(ColumnName = "Timestamp")]        
        public DateTime? Timestamp { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "Descrip")]        
        public string Descrip { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "PlcObjID", IsPrimaryKey = true)]
        public string PlcObjId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "StackerGroup")]
        public string StackerGroup { get; set; }

    }
}