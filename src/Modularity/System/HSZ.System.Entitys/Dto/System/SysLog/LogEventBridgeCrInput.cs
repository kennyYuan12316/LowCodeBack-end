using HSZ.System.Entitys.System;

namespace HSZ.System.Entitys.Dto.System.SysLog
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：日记事件创建输入
    /// </summary>
    public class LogEventBridgeCrInput
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        public string tenantId { get; set; }

        /// <summary>
        /// 租户数据库名称
        /// </summary>
        public string tenantDbName { get; set; }

        /// <summary>
        /// 日记实体
        /// </summary>
        public SysLogEntity entity { get; set; }
    }
}
