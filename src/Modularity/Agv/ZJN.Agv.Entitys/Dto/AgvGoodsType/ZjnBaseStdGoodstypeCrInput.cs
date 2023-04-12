using System;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvGoodsType
{
    /// <summary>
    /// Agv请求物料类型修改输入参数
    /// </summary>
    public class ZjnBaseStdGoodstypeCrInput
    {
        /// <summary>
        /// 数据唯一标识
        /// </summary>
        public string requestId { get; set; }
        
        /// <summary>
        /// 系统标识
        /// </summary>
        public string clientCode { get; set; }
        
        /// <summary>
        /// 服务地址
        /// </summary>
        public string channelId { get; set; }
        
        /// <summary>
        /// 请求发送时间
        /// </summary>
        public DateTime? requestTime { get; set; }
        
        /// <summary>
        /// 物料类型关键字
        /// </summary>
        public string goodsCode { get; set; }
        
    }
}