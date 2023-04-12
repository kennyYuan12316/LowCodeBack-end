using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 出入库单据
    /// </summary>
    [SugarTable("zjn_plane_bills")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnPlaneBillsEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateUser")]        
        public string CreateUser { get; set; }
        
        /// <summary>
        /// 单据号码
        /// </summary>
        [SugarColumn(ColumnName = "F_BillNo")]        
        public string BillNo { get; set; }
        
        /// <summary>
        /// 单据类型
        /// </summary>
        [SugarColumn(ColumnName = "F_BillType")]        
        public string BillType { get; set; }

        /// <summary>
        /// 仓库位置
        /// </summary>
        [SugarColumn(ColumnName = "F_BillWarehouse")]
        public string BillWarehouse { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        [SugarColumn(ColumnName = "F_BillState")]        
        public string BillState { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyTime")]        
        public DateTime? LastModifyTime { get; set; }
        
        /// <summary>
        /// 修改用户
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyUserId")]        
        public string LastModifyUserId { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        [SugarColumn(ColumnName = "F_EnabledMark")]        
        public int? EnabledMark { get; set; }
        
    }
}