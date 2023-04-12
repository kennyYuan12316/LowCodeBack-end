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
    /// 描 述：单据规则
    /// </summary>
    [SugarTable("ZJN_BASE_BILLRULE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class BillRuleEntity : CLDEntityBase
    {
        /// <summary>
        /// 单据名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }

        /// <summary>
        /// 单据编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }

        /// <summary>
        /// 单据前缀
        /// </summary>
        [SugarColumn(ColumnName = "F_PREFIX")]
        public string Prefix { get; set; }

        /// <summary>
        /// 日期格式
        /// </summary>
        [SugarColumn(ColumnName = "F_DATEFORMAT")]
        public string DateFormat { get; set; }

        /// <summary>
        /// 流水位数
        /// </summary>
        [SugarColumn(ColumnName = "F_DIGIT")]
        public int? Digit { get; set; }

        /// <summary>
        /// 流水起始
        /// </summary>
        [SugarColumn(ColumnName = "F_STARTNUMBER")]
        public string StartNumber { get; set; }

        /// <summary>
        /// 流水范例
        /// </summary>
        [SugarColumn(ColumnName = "F_EXAMPLE")]
        public string Example { get; set; }

        /// <summary>
        /// 当前流水号
        /// </summary>
        [SugarColumn(ColumnName = "F_THISNUMBER")]
        public int? ThisNumber { get; set; }

        /// <summary>
        /// 输出流水号
        /// </summary>
        [SugarColumn(ColumnName = "F_OUTPUTNUMBER")]
        public string OutputNumber { get; set; }

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
    }
}
