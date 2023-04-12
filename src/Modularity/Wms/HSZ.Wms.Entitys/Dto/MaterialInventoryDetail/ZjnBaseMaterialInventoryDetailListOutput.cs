using System;

namespace HSZ.wms.Entitys.Dto.ZjnBaseMaterialInventoryDetail
{
    /// <summary>
    /// 库存明细输入参数
    /// </summary>
    public class ZjnBaseMaterialInventoryDetailListOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string F_Description { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        public string F_ProductsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string F_ProductsName { get; set; }
        
        /// <summary>
        /// 物料数量
        /// </summary>
        public int? F_ProductsQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string F_ProductsUnit { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        public string F_Batch { get; set; }
        
        /// <summary>
        /// 质量状态
        /// </summary>
        public string F_Quality { get; set; }
        
        /// <summary>
        /// 是否冻结
        /// </summary>
        public string F_Freeze { get; set; }
        
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
        /// 所属托盘
        /// </summary>
        public string F_Tray { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? F_EntryTime { get; set; }
        
        /// <summary>
        /// 标签条码
        /// </summary>
        public string F_Label { get; set; }
        
    }
}