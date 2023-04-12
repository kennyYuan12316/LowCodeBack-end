using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseBill
{
    /// <summary>
    /// 单据信息列表查询输入
    /// </summary>
    public class ZjnBaseBillListQueryInput : PageInputBase
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
        /// 单据编号
        /// </summary>
        public string F_BillNo { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public string F_Type { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public string F_EnabledMark { get; set; }
        
    }
}