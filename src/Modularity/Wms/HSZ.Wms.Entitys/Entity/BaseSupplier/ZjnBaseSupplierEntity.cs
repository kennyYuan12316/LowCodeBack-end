using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 供应商管理
    /// </summary>
    [SugarTable("zjn_base_supplier")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBaseSupplierEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 供应商编号
        /// </summary>
        [SugarColumn(ColumnName = "F_SupplierNo")]        
        public string SupplierNo { get; set; }
        
        /// <summary>
        /// 供应商名称
        /// </summary>
        [SugarColumn(ColumnName = "F_SupplierName")]        
        public string SupplierName { get; set; }
        
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
        /// 最后修改者
        /// </summary>
        [SugarColumn(ColumnName = "F_ModifiedUser")]        
        public string ModifiedUser { get; set; }
        
        /// <summary>
        /// 最后修改时间
        /// </summary>
        [SugarColumn(ColumnName = "F_ModifiedTime")]        
        public DateTime? ModifiedTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        [SugarColumn(ColumnName = "F_EnabledMark")]        
        public int? EnabledMark { get; set; }
        /// <summary>
        /// 是否删除 0未删除 1删除
        /// </summary>
        [SugarColumn(ColumnName = "F_IsDelete")]
        public int? IsDelete { get; set; }

    }
}