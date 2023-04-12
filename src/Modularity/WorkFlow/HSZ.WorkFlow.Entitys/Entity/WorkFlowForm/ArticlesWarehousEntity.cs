using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;
using System;

namespace HSZ.WorkFlow.Entitys
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：用品入库申请表
    /// </summary>
    [SugarTable("ZJN_WFORM_ARTICLE_WAREHOUS")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ArticlesWarehousEntity : EntityBase<string>
    {
        /// <summary>
        /// 流程主键
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWID")]
        public string FlowId { get; set; }
        /// <summary>
        /// 流程标题
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWTITLE")]
        public string FlowTitle { get; set; }
        /// <summary>
        /// 紧急程度
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWURGENT")]
        public int? FlowUrgent { get; set; }
        /// <summary>
        /// 流程单据
        /// </summary>
        [SugarColumn(ColumnName = "F_BILLNO")]
        public string BillNo { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYUSER")]
        public string ApplyUser { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        [SugarColumn(ColumnName = "F_DEPARTMENT")]
        public string Department { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYDATE")]
        public DateTime? ApplyDate { get; set; }
        /// <summary>
        /// 用品库存
        /// </summary>
        [SugarColumn(ColumnName = "F_ARTICLES")]
        public string Articles { get; set; }
        /// <summary>
        /// 用品分类
        /// </summary>
        [SugarColumn(ColumnName = "F_CLASSIFICATION")]
        public string Classification { get; set; }
        /// <summary>
        /// 用品编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ARTICLESID")]
        public string ArticlesId { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [SugarColumn(ColumnName = "F_COMPANY")]
        public string Company { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [SugarColumn(ColumnName = "F_ESTIMATEPEOPLE")]
        public string EstimatePeople { get; set; }
        /// <summary>
        /// 申请原因
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYREASONS")]
        public string ApplyReasons { get; set; }
    }
}
