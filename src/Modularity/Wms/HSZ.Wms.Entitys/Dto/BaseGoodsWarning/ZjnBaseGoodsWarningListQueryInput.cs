using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseGoodsWarning
{
    /// <summary>
    /// 物料库存预警列表查询输入
    /// </summary>
    public class ZjnBaseGoodsWarningListQueryInput : PageInputBase
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
        public string F_ProductsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string F_ProductsName { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        public string F_Batch { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        public string F_EntryTime { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        public string F_ProductsType { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        public string F_ProductsSupplier { get; set; }
        
        /// <summary>
        /// 失效时间
        /// </summary>
        public string F_FailureTime { get; set; }
        
        /// <summary>
        /// 保质期预警
        /// </summary>
        public string F_WarningTime { get; set; }
        
    }
}