using HSZ.Common.Filter;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvPdaTask
{
    /// <summary>
    /// Agv上传PDA任务列表查询输入
    /// </summary>
    public class ZjnBaseStdPdataskListQueryInput : PageInputBase
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
        /// 任务类型
   
        /// </summary>
        public string F_TaskType { get; set; }
        
        /// <summary>
        /// 起点库位编码
   
        /// </summary>
        public string F_StartAreaCode { get; set; }
        
        /// <summary>
        /// 起点库区编码
   
        /// </summary>
        public string F_StartLocCode { get; set; }
        
        /// <summary>
        /// 终点库区编码
   
        /// </summary>
        public string F_EndAreaCode { get; set; }
        
        /// <summary>
        /// 终点库位编码
   
        /// </summary>
        public string F_EndLocCode { get; set; }
        
        /// <summary>
        /// 物料类型编号
   
        /// </summary>
        public string F_GoodsCode { get; set; }
        
        /// <summary>
        /// 容器编码
   
        /// </summary>
        public string F_ContainerCode { get; set; }
        
    }
}