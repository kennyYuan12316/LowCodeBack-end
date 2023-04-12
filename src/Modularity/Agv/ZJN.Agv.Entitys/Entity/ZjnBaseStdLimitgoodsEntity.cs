using HSZ.Common.Const;
using SqlSugar;
using System;

namespace ZJN.Agv.Entitys.Entity
{
    /// <summary>
    /// Agv请求取放
    /// </summary>
    [SugarTable("zjn_base_std_limitgoods")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBaseStdLimitgoodsEntity
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
        /// 任务类型
        /// </summary>
        [SugarColumn(ColumnName = "F_TaskType")]        
        public int? TaskType { get; set; }
        
        /// <summary>
        /// 库位编号
        /// </summary>
        [SugarColumn(ColumnName = "F_LocationCode")]        
        public string LocationCode { get; set; }
        
        /// <summary>
        /// 容器编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ContainerCode")]        
        public string ContainerCode { get; set; }
        
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
        
    }
}