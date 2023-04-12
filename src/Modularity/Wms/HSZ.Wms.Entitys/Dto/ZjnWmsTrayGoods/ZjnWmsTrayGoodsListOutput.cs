using System;

namespace HSZ.wms.Entitys.Dto.ZjnRecordTrayGoods
{
    /// <summary>
    /// 托盘货物绑定表输入参数
    /// </summary>
    public class ZjnWmsTrayGoodsListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 货物ID
        /// </summary>
        public string F_GoodsId { get; set; }
        
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
        public int? F_EnabledMark { get; set; }

    }
}