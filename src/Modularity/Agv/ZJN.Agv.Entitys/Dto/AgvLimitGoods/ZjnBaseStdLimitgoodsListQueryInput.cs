using HSZ.Common.Filter;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvLimitGoods
{
    /// <summary>
    /// Agv请求取放列表查询输入
    /// </summary>
    public class ZjnBaseStdLimitgoodsListQueryInput : PageInputBase
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
        /// 任务类型
        /// </summary>
        public string F_TaskType { get; set; }
        
        /// <summary>
        /// 库位编号
        /// </summary>
        public string F_LocationCode { get; set; }
        
        /// <summary>
        /// 容器编码
        /// </summary>
        public string F_ContainerCode { get; set; }
        
        /// <summary>
        /// 请求发送时间
        /// </summary>
        public string F_RequestTime { get; set; }
        
    }
}