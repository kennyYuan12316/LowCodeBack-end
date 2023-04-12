using HSZ.Common.Const;
using SqlSugar;
using System;

namespace ZJN.Agv.Entitys.Entity
{
    /// <summary>
    /// Agv上传任务状态
    /// </summary>
    [SugarTable("zjn_base_std_taskstatus")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBaseStdTaskstatusEntity
    {
        /// <summary>
        /// F_Id
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 数据唯一标识
        /// </summary>
        [SugarColumn(ColumnName = "F_RequestId")]        
        public string RequestId { get; set; }
        
        /// <summary>
        /// 系统标识
        /// </summary>
        [SugarColumn(ColumnName = "F_ClientCode")]        
        public string ClientCode { get; set; }
        
        /// <summary>
        /// 服务地址
        /// </summary>
        [SugarColumn(ColumnName = "F_ChannelId")]        
        public string ChannelId { get; set; }
        
        /// <summary>
        /// 请求发送时间
        /// </summary>
        [SugarColumn(ColumnName = "F_RequestTime")]        
        public DateTime? RequestTime { get; set; }
        
        /// <summary>
        /// 任务编码
        /// </summary>
        [SugarColumn(ColumnName = "F_InstanceId")]        
        public string InstanceId { get; set; }
        
        /// <summary>
        /// 任务序号
        /// </summary>
        [SugarColumn(ColumnName = "F_TaskIndex")]        
        public int? TaskIndex { get; set; }
        
        /// <summary>
        /// 任务状态
        /// </summary>
        [SugarColumn(ColumnName = "F_TaskStatus")]        
        public int? TaskStatus { get; set; }
        
        /// <summary>
        /// AGV小车
        /// </summary>
        [SugarColumn(ColumnName = "F_AgvNum")]        
        public string AgvNum { get; set; }
        
    }
}