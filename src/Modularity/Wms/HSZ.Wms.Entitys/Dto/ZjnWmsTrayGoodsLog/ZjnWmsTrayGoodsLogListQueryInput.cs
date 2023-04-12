using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsTrayGoodsLog
{
    /// <summary>
    /// 托盘绑定履历表列表查询输入
    /// </summary>
    public class ZjnWmsTrayGoodsLogListQueryInput : PageInputBase
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
        /// 货物代码
        /// </summary>
        public string F_GoodsCode { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string F_TrayNo { get; set; }
        
    }
}