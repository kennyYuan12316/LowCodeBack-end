using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneTray
{
    /// <summary>
    /// 平面库托盘信息维护输出参数
    /// </summary>
    public class ZjnPlaneTrayInfoOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string trayNo { get; set; }
        
        /// <summary>
        /// 托盘类型（根据数据字典来）
        /// </summary>
        public string type { get; set; }
        
        /// <summary>
        /// 托盘状态（根据数据字典来）
        /// </summary>
        public string trayState { get; set; }
        
        /// <summary>
        /// 是否删除 0未删除 1删除
        /// </summary>
        public int? isDelete { get; set; }
        
        /// <summary>
        /// 禁用原因
        /// </summary>
        public string disableMark { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>
        public string createUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 修改人
        /// </summary>
        public string lastModifyUserId { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? lastModifyTime { get; set; }
        
    }
}