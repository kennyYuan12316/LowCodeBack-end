using System;

namespace HSZ.wms.Entitys.Dto.ZjnBaseChangeorder
{
    /// <summary>
    /// 变更列表输入参数
    /// </summary>
    public class ZjnBaseChangeorderListOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 变更单
        /// </summary>
        public string F_ChangeOrder { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public string F_BusinessType { get; set; }
        
        /// <summary>
        /// 物料数量
        /// </summary>
        public int? F_ProductsQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string F_ProductsUnit { get; set; }
        
        /// <summary>
        /// 位置
        /// </summary>
        public string F_Location { get; set; }
        
        /// <summary>
        /// 位置名
        /// </summary>
        public string F_LocationName { get; set; }
        
        /// <summary>
        /// 所属仓库
        /// </summary>
        public string F_WareHouse { get; set; }
        
        /// <summary>
        /// 物料编码转移前
        /// </summary>
        public string F_ProductsCodeAgo { get; set; }
        
        /// <summary>
        /// 物料名称转移前
        /// </summary>
        public string F_ProductsNameAgo { get; set; }
        
        /// <summary>
        /// 批次号转移前
        /// </summary>
        public string F_BatchAgo { get; set; }
        
        /// <summary>
        /// 库存状态转移前
        /// </summary>
        public string F_InventoryStatusAgo { get; set; }
        
        /// <summary>
        /// 物料编码转移后
        /// </summary>
        public string F_ProductsCodeAfter { get; set; }
        
        /// <summary>
        /// 物料名称转移后
        /// </summary>
        public string F_ProductsNameAfter { get; set; }
        
        /// <summary>
        /// 批次号转移后
        /// </summary>
        public string F_BatchAfter { get; set; }
        
        /// <summary>
        /// 库存状态转移后
        /// </summary>
        public string F_InventoryStatusAfter { get; set; }
        
    }
}