using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// PLC连接表
    /// </summary>
    [SugarTable("zjn_wcs_plc")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWcsPlcEntity
    {
        /// <summary>
        /// Key
        /// </summary>
        [SugarColumn(ColumnName = "PlcID", IsPrimaryKey = true)]
        public string PlcId { get; set; }
        
        /// <summary>
        /// 是否有效
        /// </summary>
        [SugarColumn(ColumnName = "IsActive")]        
        public bool IsActive { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(ColumnName = "Caption")]        
        public string Caption { get; set; }
        
        /// <summary>
        /// 区域
        /// </summary>
        [SugarColumn(ColumnName = "Region")]        
        public string Region { get; set; }
        
        /// <summary>
        /// 连接状态
        /// </summary>
        [SugarColumn(ColumnName = "IsConnected")]
        public bool IsConnected { get; set; }
        
        /// <summary>
        /// 类型;1500;1200
        /// </summary>
        [SugarColumn(ColumnName = "CpuType")]        
        public int? CpuType { get; set; }
        
        /// <summary>
        /// ip
        /// </summary>
        [SugarColumn(ColumnName = "IP")]        
        public string Ip { get; set; }
        
        /// <summary>
        /// port
        /// </summary>
        [SugarColumn(ColumnName = "Port")]        
        public int? Port { get; set; }
        
        /// <summary>
        /// Plc Rack
        /// </summary>
        [SugarColumn(ColumnName = "Rack")]        
        public int? Rack { get; set; }
        
        /// <summary>
        /// Plc Sock
        /// </summary>
        [SugarColumn(ColumnName = "Sock")]        
        public int? Sock { get; set; }
        
        /// <summary>
        /// Plc读写超时MS
        /// </summary>
        [SugarColumn(ColumnName = "TimeOut")]        
        public int? TimeOut { get; set; }
        
        /// <summary>
        /// 堆垛机,DeviceID,Plc读写包用
        /// </summary>
        [SugarColumn(ColumnName = "StackerID")]        
        public string StackerId { get; set; }
        
        /// <summary>
        /// 是否堆垛机
        /// </summary>
        [SugarColumn(ColumnName = "IsStacker")]        
        public bool IsStacker { get; set; }
        
        /// <summary>
        /// 异常
        /// </summary>
        [SugarColumn(ColumnName = "Error")]        
        public string Error { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "Descrip")]        
        public string Descrip { get; set; }
        
    }
}