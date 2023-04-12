using HSZ.Dependency;

namespace HSZ.System.Entitys.Model.System.DataBase
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：表列表实体
    /// </summary>
    [SuppressSniffer]
    public class DynamicDbTableModel
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string F_TABLE { get; set; }

        /// <summary>
        /// 表说明
        /// </summary>
        public string F_TABLENAME { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        public string F_SIZE { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public string F_SUM { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public string F_PRIMARYKEY { get; set; }
    }
}
