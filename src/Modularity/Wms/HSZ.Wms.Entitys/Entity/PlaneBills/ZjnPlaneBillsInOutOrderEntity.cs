using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 出入库单据
    /// </summary>
    [SugarTable("zjn_plane_bills_In_out_order")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnPlaneBillsInOutOrderEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 主表ID
        /// </summary>
        [SugarColumn(ColumnName = "F_ParentId")]        
        public string ParentId { get; set; }
        
        /// <summary>
        /// 单据号
        /// </summary>
        [SugarColumn(ColumnName = "F_OrdeerNo")]        
        public string OrdeerNo { get; set; }
        
        /// <summary>
        /// 货物ID
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsNo")]        
        public string ProductsNo { get; set; }
        
        /// <summary>
        /// 货物名称
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsName")]        
        public string ProductsName { get; set; }
        
        /// <summary>
        /// 当前状态
        /// </summary>
        [SugarColumn(ColumnName = "F_OrderState")]        
        public string OrderState { get; set; }
        
        /// <summary>
        /// 货物总数
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsTotal")]        
        public decimal? ProductsTotal { get; set; }
        
        /// <summary>
        /// 差异数量
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsDifference")]        
        public decimal? ProductsDifference { get; set; }

        /// <summary>
        /// 完成数量
        /// </summary>
        [SugarColumn(ColumnName = "F_SubmitCount")]
        public decimal? SubmitCount { get; set; }

        /// <summary>
        /// 货物单位
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsUnit")]        
        public string ProductsUnit { get; set; }
        
        /// <summary>
        /// 货物批次
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsBach")]        
        public string ProductsBach { get; set; }
        
        /// <summary>
        /// 货物类型
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsType")]        
        public string ProductsType { get; set; }
        
        /// <summary>
        /// 货物规格
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsStyle")]        
        public string ProductsStyle { get; set; }
        
        /// <summary>
        /// 生产日期
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsDate")]        
        public DateTime? ProductsDate { get; set; }
        
        /// <summary>
        /// 检验类型
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsCheckType")]        
        public string ProductsCheckType { get; set; }
        
        /// <summary>
        /// 检验日期
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsCheckDate")]        
        public DateTime? ProductsCheckDate { get; set; }
        
        /// <summary>
        /// 检验结果
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsCheckResult")]        
        public string ProductsCheckResult { get; set; }
        
        /// <summary>
        /// 货物等级
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsGrade")]        
        public string ProductsGrade { get; set; }
        
        /// <summary>
        /// 客户编号
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsUser")]        
        public string ProductsUser { get; set; }
        
        /// <summary>
        /// 供应商编号
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsSupplier")]        
        public string ProductsSupplier { get; set; }
        
        /// <summary>
        /// 货位编号
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsLocation")]        
        public string ProductsLocation { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsTray")]        
        public string ProductsTray { get; set; }
        
        /// <summary>
        /// 入库日期
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsInDate")]        
        public DateTime? ProductsInDate { get; set; }
        
        /// <summary>
        /// 出库日期
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsOutDate")]        
        public DateTime? ProductsOutDate { get; set; }
        
        /// <summary>
        /// 货物备注
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsRemarks")]        
        public string ProductsRemarks { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        [SugarColumn(ColumnName = "F_EnabledMark")]        
        public int? EnabledMark { get; set; }
        
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
        /// 修改时间
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyTime")]        
        public DateTime? LastModifyTime { get; set; }
        
        /// <summary>
        /// 修改用户
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyUserId")]        
        public string LastModifyUserId { get; set; }
        
    }
}