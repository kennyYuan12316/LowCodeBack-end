using System;

namespace HSZ.wms.Entitys.Dto.ZjnWmsSupplier
{

    /// <summary>
    /// 供应商管理输入参数
    /// </summary>
    public class ZjnWmsSupplierListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 供应商编号
        /// </summary>
        public string F_SupplierNo { get; set; }
        
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string F_SupplierName { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 最后修改者
        /// </summary>
        public string F_ModifiedUser { get; set; }
        
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? F_ModifiedTime { get; set; }
        
    }
}