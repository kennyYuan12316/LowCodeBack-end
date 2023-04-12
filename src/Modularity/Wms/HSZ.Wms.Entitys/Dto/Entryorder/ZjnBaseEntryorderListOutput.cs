using System;

namespace HSZ.wms.Entitys.Dto.ZjnBaseEntryorder
{
    /// <summary>
    /// 收货列表输入参数
    /// </summary>
    public class ZjnBaseEntryorderListOutput
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
        /// 更新时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        public string F_ProductsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string F_ProductsName { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        public string F_Batch { get; set; }
        
        /// <summary>
        /// 所属仓库
        /// </summary>
        public string F_WareHouse { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public string F_BusinessType { get; set; }
        
        /// <summary>
        /// 入库单
        /// </summary>
        public string F_EntryOrder { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? F_EntryTime { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 物料状态
        /// </summary>
        public string F_ProductsStatus { get; set; }
        
    }
}