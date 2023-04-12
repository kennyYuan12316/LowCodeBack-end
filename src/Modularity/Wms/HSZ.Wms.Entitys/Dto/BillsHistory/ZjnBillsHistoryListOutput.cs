using System;

namespace HSZ.wms.Entitys.Dto.ZjnBillsHistory
{
    /// <summary>
    /// 入库明细-APP端输入参数
    /// </summary>
    public class ZjnBillsHistoryListOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 子单据Id
        /// </summary>
        public string F_OrderNo { get; set; }
        
        /// <summary>
        /// 单据类型
        /// </summary>
        public string F_OrderType { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string F_ProductsName { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        public string F_ProductsType { get; set; }
        
        /// <summary>
        /// 物料规格
        /// </summary>
        public string F_ProductsStyle { get; set; }
        
        /// <summary>
        /// 物料数量
        /// </summary>
        public decimal? F_ProductsTotal { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string F_ProductsUnit { get; set; }
        
        /// <summary>
        /// 物料等级
        /// </summary>
        public string F_ProductsGrade { get; set; }
        
        /// <summary>
        /// 物料批次
        /// </summary>
        public string F_ProductsBach { get; set; }
        
        /// <summary>
        /// 客户
        /// </summary>
        public string F_ProductsUser { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        public string F_ProductsSupplier { get; set; }
        
        /// <summary>
        /// 物料货位
        /// </summary>
        public string F_ProductsLocation { get; set; }

        /// <summary>
        /// 托盘号
        /// </summary>
        public string F_TheTray { get; set; }
        /// <summary>
        /// 容器号
        /// </summary>
        public string F_TheContainer { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string F_CreateUser { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string F_LastModifyUserId { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? F_LastModifyTime { get; set; }


        /// <summary>
        /// 校验状态
        /// </summary>

        public int? F_InspectionStatus { get; set; }
        /// <summary>
        /// 失效日期
        /// </summary>

        public DateTime? F_ExpiryDate { get; set; }

        /// <summary>
        /// 采购单号
        /// </summary>
        public string F_PurchaseOrder { get; set; }

        /// <summary>
        /// 校验类型
        /// </summary>
        public int? F_TestType { get; set; }

        public int? F_IsDelete { get; set; }

        public DateTime? F_TheDateOfProduction { get; set; }

        public string F_ProductsNo { get; set; }

        public string F_BatchDeliveryQuantity { get; set; }
    }
}