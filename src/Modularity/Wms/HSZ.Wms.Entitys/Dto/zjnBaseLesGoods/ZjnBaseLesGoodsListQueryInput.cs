using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseLesGoods
{
    /// <summary>
    /// LES物料原始数据列表查询输入
    /// </summary>
    public class ZjnBaseLesGoodsListQueryInput : PageInputBase
    {
        /// <summary>
        /// 选择导出数据key
        /// </summary>
        public string selectKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int dataType { get; set; }

        /// <summary>
        /// 物料编号
        /// </summary>
        public string F_Code { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string F_xName { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        public string F_xType { get; set; }
        
        /// <summary>
        /// 静置时间
        /// </summary>
        public string F_StayHours { get; set; }
        
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public string F_CreateTime { get; set; }
        
    }
}