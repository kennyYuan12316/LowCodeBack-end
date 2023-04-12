using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;

namespace HSZ.System.Entitys.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：常用字段
    /// </summary>
    [SugarTable("ZJN_BASE_COMFIELDS")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ComFieldsEntity : CLDEntityBase
    {
        /// <summary>
        /// 字段注释
        /// </summary>
        [SugarColumn(ColumnName = "F_FIELDNAME")]
        public string FieldName { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        [SugarColumn(ColumnName = "F_FIELD")]
        public string Field { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(ColumnName = "F_DATATYPE")]
        public string DataType { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        [SugarColumn(ColumnName = "F_DATALENGTH")]
        public string DataLength { get; set; }

        /// <summary>
        /// 允许空
        /// </summary>
        [SugarColumn(ColumnName = "F_ALLOWNULL")]
        public int? AllowNull { get; set; }

        /// <summary>
        /// 排序码(默认0)
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }

        /// <summary>
        /// 描述说明
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
