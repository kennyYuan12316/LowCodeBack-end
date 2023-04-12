using HSZ.Common.Filter;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneTray
{
    /// <summary>
    /// 平面库托盘信息维护列表查询输入
    /// </summary>
    public class ZjnPlaneTrayListQueryInput : PageInputBase
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
        /// 托盘编号
        /// </summary>
        public string F_TrayNo { get; set; }

        /// <summary>
        /// 托盘类型（根据数据字典来）
        /// </summary>
        public string F_Type { get; set; }

        /// <summary>
        /// 托盘状态（根据数据字典来）
        /// </summary>
        public string F_TrayState { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime F_CreateTime {get;set;}  
        /// <summary>
        /// 创建人
        /// </summary>
        public string F_CreateUser { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime F_LastModifyTime { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string F_LastModifyUserId { get; set; }
    }
}