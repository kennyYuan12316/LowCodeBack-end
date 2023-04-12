using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails
{
    /// <summary>
    /// 子任务修改输入参数
    /// </summary>
    public class ZjnWmsTaskDetailsCrInput
    {
        /// <summary>
        /// 子任务id
        /// </summary>
        public string taskDetailsId { get; set; }
        
        /// <summary>
        /// 子任务名称
        /// </summary>
        public string taskDetailsName { get; set; }
        
        /// <summary>
        /// 主任务id
        /// </summary>
        public string taskId { get; set; }
        
        /// <summary>
        /// 起点工位
        /// </summary>
        public string taskDetailsStart { get; set; }
        
        /// <summary>
        /// 终点工位
        /// </summary>
        public string taskDetailsEnd { get; set; }
        
        /// <summary>
        /// 子任务状态
        /// </summary>
        public int? taskDetailsStates { get; set; }
        
        /// <summary>
        /// 子任务状态描述
        /// </summary>
        public string taskDetailsDescribe { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        public string createUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 修改用户
        /// </summary>
        public string modifiedUser { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? modifiedTime { get; set; }


        /// <summary>
        /// 子任务类型
        /// </summary>
        public int? taskType { get; set; }

    }
}