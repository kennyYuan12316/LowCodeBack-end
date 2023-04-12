using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseGoods
{
    /// <summary>
    /// 货物信息输出参数
    /// </summary>
    public class ZjnBaseGoodsInfoOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 货物ID
        /// </summary>
        public string goodsId { get; set; }
        
        /// <summary>
        /// 货物代码
        /// </summary>
        public string goodsCode { get; set; }

        /// <summary>
        /// 货物类型
        /// </summary>
        public string goodstype { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string goodsName { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string unit { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string trayNo { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 主表唯一id
        /// </summary>
        public string parentId { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public int? hsz_zjn_base_goods_details_hsz_enabledMark { get; set; }
        
        /// <summary>
        /// 货物批次
        /// </summary>
        public string hsz_zjn_base_goods_details_hsz_batch { get; set; }
        
        /// <summary>
        /// 货物规格
        /// </summary>
        public string hsz_zjn_base_goods_details_hsz_specifications { get; set; }
        
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? hsz_zjn_base_goods_details_hsz_goodsCreateData { get; set; }
        
        /// <summary>
        /// 货物状态
        /// </summary>
        public string hsz_zjn_base_goods_details_hsz_goodsState{ get; set; }
        
        /// <summary>
        /// 货位ID
        /// </summary>
        public string hsz_zjn_base_goods_details_hsz_goodsLocationNo { get; set; }
        
        /// <summary>
        /// 客户ID
        /// </summary>
        public string hsz_zjn_base_goods_details_hsz_customerId { get; set; }
        
        /// <summary>
        /// 托盘ID
        /// </summary>
        public string hsz_zjn_base_goods_details_hsz_palledNo { get; set; }
        
        /// <summary>
        /// 供应商ID
        /// </summary>
        public string hsz_zjn_base_goods_details_hsz_vendorId { get; set; }
        
        /// <summary>
        /// 检验日期
        /// </summary>
        public DateTime? hsz_zjn_base_goods_details_hsz_checkDate { get; set; }
        
        /// <summary>
        /// 检验类型
        /// </summary>
        public string hsz_zjn_base_goods_details_hsz_checkType { get; set; }
        
        /// <summary>
        /// 货物等级
        /// </summary>
        public string hsz_zjn_base_goods_details_hsz_goodsGrade{ get; set; }
        
        /// <summary>
        /// 货物描述
        /// </summary>
        public string hsz_zjn_base_goods_details_hsz_remarks { get; set; }
        
    }
}