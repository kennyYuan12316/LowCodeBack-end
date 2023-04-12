using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneOperationLog
{
    /// <summary>
    /// 平面库操作日志信息修改输入参数
    /// </summary>
    public class ZjnPlaneOperationLogCrInput
    {
        /// <summary>
        /// 操作类型 1.修改 2.删除
        /// </summary>
        public string type { get; set; }
        
        /// <summary>
        /// 操作描述（描述在哪个业务执行）
        /// </summary>
        public string describe { get; set; }
        
        /// <summary>
        /// 操作路径 1.APP 2.WEB
        /// </summary>
        public int? workPath { get; set; }
        
        /// <summary>
        /// 操作前数据
        /// </summary>
        public string beforeDate { get; set; }
        
        /// <summary>
        /// 操作后数据
        /// </summary>
        public string afterDate { get; set; }
        
        /// <summary>
        /// 操作人
        /// </summary>
        public string createUser { get; set; }
        
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
    }
}