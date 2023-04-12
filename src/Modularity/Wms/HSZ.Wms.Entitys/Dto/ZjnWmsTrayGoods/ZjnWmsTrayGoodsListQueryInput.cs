using HSZ.Common.Filter;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnRecordTrayGoods
{
    /// <summary>
    /// 托盘货物绑定表列表查询输入
    /// </summary>
    public class ZjnWmsTrayGoodsListQueryInput : PageInputBase
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
        /// 货物ID
        /// </summary>
        public string F_GoodsId { get; set; }
        
        /// <summary>
        /// 货物代码
        /// </summary>
        public string F_GoodsCode { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        public string F_Quantity { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        public string F_Unit { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string F_TrayNo { get; set; }


    }
}