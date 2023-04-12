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
    /// 描 述：字典数据
    /// </summary>
    [SugarTable("ZJN_BASE_DICTIONARY_DATA")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class DictionaryDataEntity : CLDEntityBase
    {
        /// <summary>
        /// 上级
        /// </summary>
        [SugarColumn(ColumnName = "F_PARENTID")]
        public string ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }

        /// <summary>
        /// 拼音
        /// </summary>
        [SugarColumn(ColumnName = "F_SIMPLESPELLING")]
        public string SimpleSpelling { get; set; }

        /// <summary>
        /// 默认
        /// </summary>
        [SugarColumn(ColumnName = "F_ISDEFAULT")]
        public int? IsDefault { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }

        /// <summary>
        /// 类别主键
        /// </summary>
        [SugarColumn(ColumnName = "F_DICTIONARYTYPEID")]
        public string DictionaryTypeId { get; set; }
    }
}
