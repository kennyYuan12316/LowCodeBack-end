using System;

namespace ZJN.Agv.Entitys.Dto.AgvGoodsType
{
    /// <summary>
    /// Agv请求物料类型输入参数
    /// </summary>
    public class ZjnBaseStdGoodstypeListOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 数据唯一标识
        /// </summary>
        public string F_RequestId { get; set; }
        
        /// <summary>
        /// 系统标识
        /// </summary>
        public string F_ClientCode { get; set; }
        
        /// <summary>
        /// 服务地址
        /// </summary>
        public string F_ChannelId { get; set; }
        
        /// <summary>
        /// 请求发送时间
        /// </summary>
        public DateTime? F_RequestTime { get; set; }
        
        /// <summary>
        /// 物料类型关键字
        /// </summary>
        public string F_GoodsCode { get; set; }
        
    }
}