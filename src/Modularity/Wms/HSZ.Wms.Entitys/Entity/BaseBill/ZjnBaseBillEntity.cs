using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 单据信息
    /// </summary>
    [SugarTable("zjn_base_bill")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBaseBillEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 单据编号
        /// </summary>
        [SugarColumn(ColumnName = "F_BillNo")]        
        public string BillNo { get; set; }
        
        /// <summary>
        /// 单据名称
        /// </summary>
        [SugarColumn(ColumnName = "F_BillName")]        
        public string BillName { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(ColumnName = "F_Type")]        
        public int? Type { get; set; }
        
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