using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 接口调用履历表
    /// </summary>
    [SugarTable("zjn_task_interface_apply_log")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnTaskInterfaceApplyLogEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 调用完整地址
        /// </summary>
        [SugarColumn(ColumnName = "F_Address")]        
        public string Address { get; set; }
        
        /// <summary>
        /// 接口名
        /// </summary>
        [SugarColumn(ColumnName = "F_InterfaceName")]        
        public string InterfaceName { get; set; }
        
        /// <summary>
        /// 入参
        /// </summary>
        [SugarColumn(ColumnName = "F_EnterParameter")]        
        public string EnterParameter { get; set; }
        
        /// <summary>
        /// 出参
        /// </summary>
        [SugarColumn(ColumnName = "F_OutParameter")]        
        public string OutParameter { get; set; }
        
        /// <summary>
        /// 调用时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 消息
        /// </summary>
        [SugarColumn(ColumnName = "F_Msg")]        
        public string Msg { get; set; }
        
    }
}