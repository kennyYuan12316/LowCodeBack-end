using System.Collections.Generic;

namespace HSZ.VisualDev.Entitys.Model.CodeGen
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：代码生成表关系模型
    /// </summary>
    public class CodeGenTableRelationsModel
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 表名(首字母小写)
        /// </summary>
        public string LowerTableName => string.IsNullOrWhiteSpace(TableName)
                                      ? null
                                      : TableName.Substring(0, 1).ToLower() + TableName[1..];

        /// <summary>
        /// 主键
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        /// 表描述
        /// </summary>
        public string TableComment { get; set; }

        /// <summary>
        /// 外键字段
        /// </summary>
        public string TableField { get; set; }

        /// <summary>
        /// 关联主键
        /// </summary>
        public string RelationField { get; set; }

        /// <summary>
        /// 关联主键
        /// </summary>
        public string LowerRelationField => string.IsNullOrWhiteSpace(RelationField)
                                      ? null
                                      : RelationField.Substring(0, 1).ToLower() + RelationField[1..];

        /// <summary>
        /// 子表控件配置
        /// </summary>
        public List<TableColumnConfigModel> ChilderColumnConfigList { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int TableNo { get; set; }
    }
}