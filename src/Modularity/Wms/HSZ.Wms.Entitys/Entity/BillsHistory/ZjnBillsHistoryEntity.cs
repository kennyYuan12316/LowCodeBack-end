using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 入库明细-APP端
    /// </summary>
    [SugarTable("zjn_bills_history")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBillsHistoryEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 子单据Id
        /// </summary>
        [SugarColumn(ColumnName = "F_OrderNo")]        
        public string OrderNo { get; set; }
        
        /// <summary>
        /// 单据类型
        /// </summary>
        [SugarColumn(ColumnName = "F_OrderType")]        
        public string OrderType { get; set; }
        
        /// <summary>
        /// 物料Id
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsNo")]        
        public string ProductsNo { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsName")]        
        public string ProductsName { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsType")]        
        public string ProductsType { get; set; }
        
        /// <summary>
        /// 物料规格
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsStyle")]        
        public string ProductsStyle { get; set; }
        
        /// <summary>
        /// 物料数量
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsTotal")]        
        public decimal? ProductsTotal { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsUnit")]        
        public string ProductsUnit { get; set; }
        
        /// <summary>
        /// 物料等级
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsGrade")]        
        public string ProductsGrade { get; set; }
        
        /// <summary>
        /// 物料批次
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsBach")]        
        public string ProductsBach { get; set; }
        
        /// <summary>
        /// 客户
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsUser")]        
        public string ProductsUser { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsSupplier")]        
        public string ProductsSupplier { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateUser")]        
        public string CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 物料货位
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsLocation")]        
        public string ProductsLocation { get; set; }
        
        /// <summary>
        /// 来源单号（单据主表id）
        /// </summary>
        [SugarColumn(ColumnName = "F_BiilId")]        
        public string BillId { get; set; }
        
        /// <summary>
        /// 托盘号
        /// </summary>
        [SugarColumn(ColumnName = "F_TheTray")]        
        public string TheTray { get; set; }

        /// <summary>
        /// 容器
        /// </summary>
        [SugarColumn(ColumnName = "F_TheContainer")]        
        public string TheContainer { get; set; }

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
        /// 校验状态
        /// </summary>
        [SugarColumn(ColumnName = "F_InspectionStatus")]
        public int? InspectionStatus { get; set; }
        /// <summary>
        /// 失效日期
        /// </summary>
        [SugarColumn(ColumnName = "F_ExpiryDate")]
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// 采购单号
        /// </summary>
        [SugarColumn(ColumnName = "F_PurchaseOrder")]
        public string PurchaseOrder { get; set; }

        /// <summary>
        /// 校验类型
        /// </summary>
        [SugarColumn(ColumnName = "F_TestType")]
        public int? TestType { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        [SugarColumn(ColumnName = "F_TheDateOfProduction")]
        public DateTime TheDateOfProduction { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(ColumnName = "F_IsDelete")]
        public int? IsDelete { get; set; }
        /// <summary>
        /// 出库记录   
        /// </summary>
        [SugarColumn(ColumnName = "F_BatchDeliveryQuantity")]
        public string BatchDeliveryQuantity { get; set; }

        /// <summary>
        /// 出库记录   F_QualityInspectionNumber
        /// </summary>
        [SugarColumn(ColumnName = "F_QualityInspectionNumber")]
        public string QualityInspectionNumber { get; set; }

    }
}