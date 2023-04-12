using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseOutorder
{
    /// <summary>
    /// 出货列表输出参数
    /// </summary>
    public class ZjnBaseOutorderInfoOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        public string productsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string productsName { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        public string batch { get; set; }
        
        /// <summary>
        /// 所属仓库
        /// </summary>
        public string wareHouse { get; set; }
        
        /// <summary>
        /// 出库单
        /// </summary>
        public string outOrder { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? outTime { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public string businessType { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        public string createUser { get; set; }
        
        /// <summary>
        /// 物料状态
        /// </summary>
        public string productsStatus { get; set; }
        
    }
}