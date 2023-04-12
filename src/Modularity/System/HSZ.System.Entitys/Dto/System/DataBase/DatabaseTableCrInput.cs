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
    public class DatabaseTableCrInput
    {
        /// <summary>
        /// 表信息
        /// </summary>
        public TableInfo tableInfo { get; set; }

        /// <summary>
        /// 表字段信息
        /// </summary>
        public List<TableFieldListItem> tableFieldList { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TableInfo
    {
        /// <summary>
        /// 旧表名称
        /// </summary>
        public string table { get; set; }

        /// <summary>
        /// 新表名称
        /// </summary>
        public string newTable { get; set; }

        /// <summary>
        /// 表说明
        /// </summary>
        public string tableName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TableFieldListItem
    {
        /// <summary>
        /// 是否允许为空
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
        /// 字段注释
        /// </summary>
        public string fieldName { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int index { get; set; }

        /// <summary>
        /// 是否主键（0：是，1：否）
        /// </summary>
        public int primaryKey { get; set; }
    }
}
