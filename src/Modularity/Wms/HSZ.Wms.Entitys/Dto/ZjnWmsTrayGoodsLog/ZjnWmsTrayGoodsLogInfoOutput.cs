using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsTrayGoodsLog
{
    /// <summary>
    /// 托盘绑定履历表输出参数
    /// </summary>
    public class ZjnWmsTrayGoodsLogInfoOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 货物代码
        /// </summary>
        public string goodsCode { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        public int? quantity { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        public string unit { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string trayNo { get; set; }
        
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
        public int? enabledMark { get; set; }
        
    }
}