using System;

namespace HSZ.wms.Entitys.Dto.ZjnWmsTrayGoodsLog
{
    /// <summary>
    /// 托盘绑定履历表输入参数
    /// </summary>
    public class ZjnWmsTrayGoodsLogListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 货物代码
        /// </summary>
        public string F_GoodsCode { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        public int? F_Quantity { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        public string F_Unit { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string F_TrayNo { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public string F_EnabledMark { get; set; }
        
    }
}