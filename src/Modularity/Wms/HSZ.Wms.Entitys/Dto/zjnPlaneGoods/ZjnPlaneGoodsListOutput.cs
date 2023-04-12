using System;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneGoods
{
    /// <summary>
    /// 平面库物料基础信息输入参数
    /// </summary>
    public class ZjnPlaneGoodsListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        public string F_GoodsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string F_GoodsName { get; set; }
        
        /// <summary>
        /// 物料单位（根据数据字典来）
        /// </summary>
        public int? F_Unit { get; set; }
        /// <summary>
        /// 物料单位名称（根据数据字典来）
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 物料状态 （根据数据字典来）
        /// </summary>
        public int? F_GoodsState { get; set; }

        /// <summary>
        /// 物料状态名称 （根据数据字典来）
        /// </summary>
        public string GoodsStateName { get; set; }


        /// <summary>
        /// 物料类型（根据数据字典来）
        /// </summary>
        public string  F_GoodsType { get; set; }

        /// <summary>
        /// 物料类型名称（根据数据字典来）
        /// </summary>
        public string GoodsTypeName { get; set; }

        /// <summary>
        /// 物料规格
        /// </summary>
        public string F_Specifications { get; set; }
        
        /// <summary>
        /// 客户
        /// </summary>
        public string F_CustomerId { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        public string F_VendorId { get; set; }
        
        /// <summary>
        /// 检验类型（根据数据字典来）
        /// </summary>
        public int? F_CheckType { get; set; }

        /// <summary>
        /// 检验类型名称（根据数据字典来）
        /// </summary>
        public string CheckTypeNmae { get; set; }

        /// <summary>
        /// 先入先出 1是 0否
        /// </summary>
        public int? F_IsFirstOut { get; set; }
        /// <summary>
        /// 先入先出名称 1是 0否
        /// </summary>
        public string IsFirstOutName { get; set; }

        /// <summary>
        /// 预警周期（天）
        /// </summary>
        public int? F_TellDate { get; set; }
        
        /// <summary>
        /// 禁用原因
        /// </summary>
        public string F_DisableMark { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public string isDelete { get; set; }
        // <summary>
        /// 上限
        /// </summary>        
        public string F_Ceiling { get; set; }

        /// <summary>
        /// 下限
        /// </summary>       
        public string F_TheLowerLimit { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }


        /// <summary>
        /// 创建用户
        /// </summary>
        public string CreateUser { get; set; }

        //库存数
        public string ProductsQuantity { get; set; }

        public DateTime ExpiryDate { get; set; }

        public DateTime TheDateOfProduction { get; set; }

        public string F_ShelfLife { get; set; }
    }
}