using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsSupplier
{

    /// <summary>
    /// 供应商管理列表查询输入
    /// </summary>
    public class ZjnWmsSupplierListQueryInput : PageInputBase
    {
        /// <summary>
        /// 选择导出数据key
        /// </summary>
        public string selectKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string dataType { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public string F_SupplierNo { get; set; }
        
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string F_SupplierName { get; set; }
        
    }
}