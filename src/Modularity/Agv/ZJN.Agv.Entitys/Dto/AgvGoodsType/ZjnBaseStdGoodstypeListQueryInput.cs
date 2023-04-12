using HSZ.Common.Filter;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvGoodsType
{
    /// <summary>
    /// Agv请求物料类型列表查询输入
    /// </summary>
    public class ZjnBaseStdGoodstypeListQueryInput : PageInputBase
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
        /// 数据唯一标识
        /// </summary>
        public string F_RequestId { get; set; }
        
        /// <summary>
        /// 请求发送时间
        /// </summary>
        public string F_RequestTime { get; set; }
        
        /// <summary>
        /// 物料类型关键字
        /// </summary>
        public string F_GoodsCode { get; set; }
        
    }
}