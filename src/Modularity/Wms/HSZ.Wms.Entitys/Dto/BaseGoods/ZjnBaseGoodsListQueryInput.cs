using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseGoods
{
    /// <summary>
    /// 货物信息列表查询输入
    /// </summary>
    public class ZjnBaseGoodsListQueryInput : PageInputBase
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
        public string a_F_GoodsId { get; set; }
        
        /// <summary>
        /// 货物代码
        /// </summary>
        public string a_F_GoodsCode { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public string a_CreateTime { get; set; }

        /// <summary>
        /// 货物批次
        /// </summary>
        public string a1_F_batch { get; set; }


        /// <summary>
        /// 生产日期
        /// </summary>
        public string a1_F_GoodsCreateData { get; set; }
        
    }
}