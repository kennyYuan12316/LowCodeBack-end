using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneGoods
{
    /// <summary>
    /// 平面库物料基础信息输出参数
    /// </summary>
    public class ZjnPlaneGoodsInfoOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        public string goodsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string goodsName { get; set; }
        
        /// <summary>
        /// 物料单位（根据数据字典来）
        /// </summary>
        public string unit { get; set; }
        
        /// <summary>
        /// 物料状态 （根据数据字典来）
        /// </summary>
        public string goodsState { get; set; }
        
        /// <summary>
        /// 物料类型（根据数据字典来）
        /// </summary>
        public string goodsType { get; set; }
        
        /// <summary>
        /// 物料规格
        /// </summary>
        public string specifications { get; set; }
        
        /// <summary>
        /// 客户
        /// </summary>
        public string customerId { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        public string vendorId { get; set; }
        
        /// <summary>
        /// 检验类型（根据数据字典来）
        /// </summary>
        public string checkType { get; set; }
        
        /// <summary>
        /// 先入先出 1是 0否
        /// </summary>
        public string isFirstOut { get; set; }
        
        /// <summary>
        /// 预警周期（天）
        /// </summary>
        public int? tellDate { get; set; }
        
        /// <summary>
        /// 禁用原因
        /// </summary>
        public string disableMark { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public string isDelete { get; set; }

        /// <summary>
        /// 上限
        /// </summary>
        public string Ceiling { get; set; }

        /// <summary>
        /// 下限
        /// </summary>       
        public string TheLowerLimit { get; set; }

        public string ShelfLife { get; set; }
    }
}