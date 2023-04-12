using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.BaseSupplier
{
    /// <summary>
    /// 供应商管理修改输入参数
    /// </summary>
    public class ZjnBaseSupplierCrInput
    {
        /// <summary>
        /// 供应商编号
        /// </summary>
        public string supplierNo { get; set; }
        
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string supplierName { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        public string createUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 最后修改者
        /// </summary>
        public string modifiedUser { get; set; }
        
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? modifiedTime { get; set; }

        public int? IsDelete { get; set; }


    }
}