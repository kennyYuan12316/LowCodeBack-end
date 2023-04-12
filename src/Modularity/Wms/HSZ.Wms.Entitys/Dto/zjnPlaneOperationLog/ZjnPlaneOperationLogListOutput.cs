using System;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneOperationLog
{
    /// <summary>
    /// 平面库操作日志信息输入参数
    /// </summary>
    public class ZjnPlaneOperationLogListOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 操作类型 1.修改 2.删除
        /// </summary>
        public string F_Type { get; set; }
        
        /// <summary>
        /// 操作描述（描述在哪个业务执行）
        /// </summary>
        public string F_Describe { get; set; }
        
        /// <summary>
        /// 操作路径 1.APP 2.WEB
        /// </summary>
        public int? F_WorkPath { get; set; }
        
        /// <summary>
        /// 操作前数据
        /// </summary>
        public string F_BeforeDate { get; set; }
        
        /// <summary>
        /// 操作后数据
        /// </summary>
        public string F_AfterDate { get; set; }
        
        /// <summary>
        /// 操作人
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
    }
}