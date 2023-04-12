using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 货物信息
    /// </summary>
    [SugarTable("zjn_base_goods_details")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBaseGoodsDetailsEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 货物主表id
        /// </summary>
        [SugarColumn(ColumnName = "F_ParentId")]        
        public string ParentId { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        [SugarColumn(ColumnName = "F_CreatorUserId")]        
        public string CreatorUserId { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreatorTime")]        
        public DateTime? CreatorTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        [SugarColumn(ColumnName = "F_EnabledMark")]        
        public int? EnabledMark { get; set; }
        
        /// <summary>
        /// 修改用户
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyUserId")]        
        public string LastModifyUserId { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyTime")]        
        public DateTime? LastModifyTime { get; set; }
        
        /// <summary>
        /// 货物批次
        /// </summary>
        [SugarColumn(ColumnName = "F_batch")]        
        public string Batch { get; set; }
        
        /// <summary>
        /// 货物规格
        /// </summary>
        [SugarColumn(ColumnName = "F_specifications")]        
        public string Specifications { get; set; }
        
        /// <summary>
        /// 生产日期
        /// </summary>
        [SugarColumn(ColumnName = "F_GoodsCreateData")]        
        public DateTime? GoodsCreateData { get; set; }
        
        /// <summary>
        /// 货物状态
        /// </summary>
        [SugarColumn(ColumnName = "F_GoodsState")]        
        public string GoodsState { get; set; }
        
        /// <summary>
        /// 货位ID
        /// </summary>
        [SugarColumn(ColumnName = "F_GoodsLocationNo")]        
        public string GoodsLocationNo { get; set; }
        
        /// <summary>
        /// 客户ID
        /// </summary>
        [SugarColumn(ColumnName = "F_CustomerId")]        
        public string CustomerId { get; set; }
        
        /// <summary>
        /// 托盘ID
        /// </summary>
        [SugarColumn(ColumnName = "F_PalledNo")]        
        public string PalledNo { get; set; }
        
        /// <summary>
        /// 供应商ID
        /// </summary>
        [SugarColumn(ColumnName = "F_VendorId")]        
        public string VendorId { get; set; }
        
        /// <summary>
        /// 检验日期
        /// </summary>
        [SugarColumn(ColumnName = "F_CheckDate")]        
        public DateTime? CheckDate { get; set; }
        
        /// <summary>
        /// 检验类型
        /// </summary>
        [SugarColumn(ColumnName = "F_CheckType")]        
        public string CheckType { get; set; }
        
        /// <summary>
        /// 货物等级
        /// </summary>
        [SugarColumn(ColumnName = "F_GoodsGrade")]        
        public string GoodsGrade { get; set; }
        
        /// <summary>
        /// 货物描述
        /// </summary>
        [SugarColumn(ColumnName = "F_Remarks")]        
        public string Remarks { get; set; }
        
    }
}