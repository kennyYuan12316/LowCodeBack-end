using System;

namespace HSZ.wms.Entitys.Dto.ZjnBaseOutorder
{
    /// <summary>
    /// 出货列表输入参数
    /// </summary>
    public class ZjnBaseOutorderListOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string F_Id { get; set; }
        
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
        /// 出库单
        /// </summary>
        public string F_OutOrder { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? F_OutTime { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public string F_BusinessType { get; set; }
        
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