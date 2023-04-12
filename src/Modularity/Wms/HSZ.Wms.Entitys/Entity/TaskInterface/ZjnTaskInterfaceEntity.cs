using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 接口配置
    /// </summary>
    [SugarTable("zjn_task_interface")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnTaskInterfaceEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 接口名称
        /// </summary>
        [SugarColumn(ColumnName = "F_NameInterface")]        
        public string NameInterface { get; set; }
        
        /// <summary>
        /// 中文
        /// </summary>
        [SugarColumn(ColumnName = "F_CnInterface")]        
        public string CnInterface { get; set; }
        
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
        /// 通讯协议
        /// </summary>
        [SugarColumn(ColumnName = "F_Communication")]        
        public string Communication { get; set; }
        
        /// <summary>
        /// 说明
        /// </summary>
        [SugarColumn(ColumnName = "F_Introduction")]        
        public string Introduction { get; set; }
        
        /// <summary>
        /// 接口提供者
        /// </summary>
        [SugarColumn(ColumnName = "F_InterfaceProvide")]        
        public string InterfaceProvide { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateUser")]        
        public string CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        [SugarColumn(ColumnName = "F_EnabledMark")]        
        public int? EnabledMark { get; set; }
        
    }
}