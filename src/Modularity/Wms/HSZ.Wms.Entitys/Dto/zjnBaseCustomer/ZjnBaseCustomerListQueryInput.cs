using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseCustomer
{
    /// <summary>
    /// 客户信息列表查询输入
    /// </summary>
    public class ZjnBaseCustomerListQueryInput : PageInputBase
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
        /// 客户编号
        /// </summary>
        public string F_CustomerNo { get; set; }
        
        /// <summary>
        /// 客户名称
        /// </summary>
        public string F_CustomerName { get; set; }
        
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string F_ContactName { get; set; }
        
    }
}