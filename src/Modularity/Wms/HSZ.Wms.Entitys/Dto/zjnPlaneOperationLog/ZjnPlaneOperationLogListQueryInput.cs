using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneOperationLog
{
    /// <summary>
    /// 平面库操作日志信息列表查询输入
    /// </summary>
    public class ZjnPlaneOperationLogListQueryInput : PageInputBase
    {
        /// <summary>
        /// 选择导出数据key
        /// </summary>
        public string selectKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? dataType { get; set; }

        /// <summary>
        /// 操作类型 1.修改 2.删除
        /// </summary>
        public int? F_Type { get; set; }
        
        /// <summary>
        /// 操作描述（描述在哪个业务执行）
        /// </summary>
        public string F_Describe { get; set; }
        
        /// <summary>
        /// 操作路径 1.APP 2.WEB
        /// </summary>
        public int? F_WorkPath { get; set; }
        
    }
}