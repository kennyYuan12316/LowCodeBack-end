using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.Database
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class DatabaseTableListOutput
    {
        /// <summary>
        /// 说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 表记录数
        /// </summary>
        public int sum { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string table { get; set; }

        /// <summary>
        /// 表说明
        /// </summary>
        public string tableName { get; set; } 
    }
}
