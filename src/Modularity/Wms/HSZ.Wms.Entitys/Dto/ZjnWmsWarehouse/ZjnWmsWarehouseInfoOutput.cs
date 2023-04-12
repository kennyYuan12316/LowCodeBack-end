using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsWarehouse
{
    /// <summary>
    /// 仓库信息输出参数
    /// </summary>
    public class ZjnWmsWarehouseInfoOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string warehouseNo { get; set; }
        
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string warehouseName { get; set; }
        
        /// <summary>
        /// 区域编号
        /// </summary>
        public string regionNo { get; set; }
        
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