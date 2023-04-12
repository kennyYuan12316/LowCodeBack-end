using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseCustomer
{
    /// <summary>
    /// 客户信息修改输入参数
    /// </summary>
    public class ZjnBaseCustomerCrInput
    {
        /// <summary>
        /// 客户编号
        /// </summary>
        public string customerNo { get; set; }
        
        /// <summary>
        /// 客户名称
        /// </summary>
        public string customerName { get; set; }
        
        /// <summary>
        /// 联系方式
        /// </summary>
        public string customerPhone { get; set; }
        
        /// <summary>
        /// 联系地址
        /// </summary>
        public string customerAddress { get; set; }
        
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string contactName { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public string enabledMark { get; set; }
        
    }
}