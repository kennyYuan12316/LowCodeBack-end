using HSZ.Dependency;
using System.Collections.Generic;

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
    public class DatabaseTableInfoOutput
    {
        /// <summary>
        /// 表信息
        /// </summary>
        public TableInfo tableInfo { get; set; }

        /// <summary>
        /// 表字段
        /// </summary>
        public List<TableFieldList> tableFieldList { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TableFieldList
    {
        /// <summary>
        /// 允许空(0-不允许，1-允许)
        /// </summary>
        public int allowNull { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public string dataLength { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string dataType { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string fieldName { get; set; }

        /// <summary>
        /// 是否是主键（1-是，0-否）
        /// </summary>
        public int primaryKey { get; set; }
    }
}
