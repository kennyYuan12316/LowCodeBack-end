using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseChangeorder
{
    /// <summary>
    /// 变更列表输出参数
    /// </summary>
    public class ZjnBaseChangeorderInfoOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 变更单
        /// </summary>
        public string changeOrder { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        public string createUser { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public string businessType { get; set; }
        
        /// <summary>
        /// 物料数量
        /// </summary>
        public int? productsQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string productsUnit { get; set; }
        
        /// <summary>
        /// 位置
        /// </summary>
        public string location { get; set; }
        
        /// <summary>
        /// 位置名
        /// </summary>
        public string locationName { get; set; }
        
        /// <summary>
        /// 所属仓库
        /// </summary>
        public string wareHouse { get; set; }
        
        /// <summary>
        /// 物料编码转移前
        /// </summary>
        public string productsCodeAgo { get; set; }
        
        /// <summary>
        /// 物料名称转移前
        /// </summary>
        public string productsNameAgo { get; set; }
        
        /// <summary>
        /// 批次号转移前
        /// </summary>
        public string batchAgo { get; set; }
        
        /// <summary>
        /// 库存状态转移前
        /// </summary>
        public string inventoryStatusAgo { get; set; }
        
        /// <summary>
        /// 物料编码转移后
        /// </summary>
        public string productsCodeAfter { get; set; }
        
        /// <summary>
        /// 物料名称转移后
        /// </summary>
        public string productsNameAfter { get; set; }
        
        /// <summary>
        /// 批次号转移后
        /// </summary>
        public string batchAfter { get; set; }
        
        /// <summary>
        /// 库存状态转移后
        /// </summary>
        public string inventoryStatusAfter { get; set; }
        
    }
}