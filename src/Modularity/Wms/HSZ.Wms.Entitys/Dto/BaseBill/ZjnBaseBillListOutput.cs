using System;

namespace HSZ.wms.Entitys.Dto.ZjnBaseBill
{
    /// <summary>
    /// 单据信息输入参数
    /// </summary>
    public class ZjnBaseBillListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 单据编号
        /// </summary>
        public string F_BillNo { get; set; }
        
        /// <summary>
        /// 单据名称
        /// </summary>
        public string F_BillName { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public int? F_Type { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? F_EnabledMark { get; set; }
        
    }
}