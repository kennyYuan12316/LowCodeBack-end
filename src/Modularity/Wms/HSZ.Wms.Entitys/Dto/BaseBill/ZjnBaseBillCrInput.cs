using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseBill
{
    /// <summary>
    /// 单据信息修改输入参数
    /// </summary>
    public class ZjnBaseBillCrInput
    {
        /// <summary>
        /// 单据编号
        /// </summary>
        public string billNo { get; set; }
        
        /// <summary>
        /// 单据名称
        /// </summary>
        public string billName { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public int? type { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        public string createUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public string enabledMark { get; set; }
        
    }
}