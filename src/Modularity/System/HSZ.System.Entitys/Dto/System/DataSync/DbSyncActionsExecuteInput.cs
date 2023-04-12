using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.DataSync
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class DbSyncActionsExecuteInput
    {
        /// <summary>
        /// 源数据库id
        /// </summary>
        public string dbConnectionFrom { get; set; }

        /// <summary>
        /// 目前数据库id
        /// </summary>
        public string dbConnectionTo { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string dbTable { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public int type { get; set; }
    }
}
