using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsGoods
{
    /// <summary>
    /// 平面库物料基础信息列表查询输入
    /// </summary>
    public class ZjnWmsGoodsListQueryInput : PageInputBase
    {
        /// <summary>
        /// 选择导出数据key
        /// </summary>
        public string selectKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int dataType { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string F_GoodsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string F_GoodsName { get; set; }
        
        /// <summary>
        /// 物料类型（根据数据字典来）
        /// </summary>
        public string F_GoodsType { get; set; }

        public string F_OrderType { get; set; }

        public string F_ShelfLife { get; set; }

    }
}