using HSZ.Common.Const;
using SqlSugar;
using System;

namespace ZJN.Agv.Entitys.Entity
{
    /// <summary>
    /// 立库取消订单
    /// </summary>
    [SugarTable("zjn_base_std_cancelorder")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBaseStdCancelorderEntity
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
        /// 外部订单ID
        /// </summary>
        [SugarColumn(ColumnName = "F_OuterOrderId")]        
        public string OuterOrderId { get; set; }
        
    }
}