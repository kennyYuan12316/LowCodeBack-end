using HSZ.Common.Const;
using SqlSugar;
using System;

namespace ZJN.Agv.Entitys.Entity
{
    /// <summary>
    /// Agv请求物料类型
    /// </summary>
    [SugarTable("zjn_base_std_goodstype")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBaseStdGoodstypeEntity
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
        /// 物料类型关键字
        /// </summary>
        [SugarColumn(ColumnName = "F_GoodsCode")]        
        public string GoodsCode { get; set; }
        
    }
}