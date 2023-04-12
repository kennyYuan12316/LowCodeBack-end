using System;

namespace HSZ.wms.Entitys.Dto.ZjnBaseCustomer
{
    /// <summary>
    /// 客户信息输入参数
    /// </summary>
    public class ZjnBaseCustomerListOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 客户编号
        /// </summary>
        public string F_CustomerNo { get; set; }
        
        /// <summary>
        /// 客户名称
        /// </summary>
        public string F_CustomerName { get; set; }
        
        /// <summary>
        /// 联系方式
        /// </summary>
        public string F_CustomerPhone { get; set; }
        
        /// <summary>
        /// 联系地址
        /// </summary>
        public string F_CustomerAddress { get; set; }
        
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string F_ContactName { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string F_Description { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? F_EnabledMark { get; set; }
        
    }
}