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
    /// 描 述：流程委托
    /// </summary>
    [SugarTable("ZJN_FLOW_DELEGATE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class FlowDelegateEntity : CLDEntityBase
    {
        /// <summary>
        /// 被委托人
        /// </summary>
        [SugarColumn(ColumnName = "F_TOUSERID")]
        public string ToUserId { get; set; }
        /// <summary>
        /// 被委托人
        /// </summary>
        [SugarColumn(ColumnName = "F_TOUSERNAME")]
        public string ToUserName { get; set; }
        /// <summary>
        /// 委托流程
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWID")]
        public string FlowId { get; set; }
        /// <summary>
        /// 委托流程
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWNAME")]
        public string FlowName { get; set; }
        /// <summary>
        /// 流程分类
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWCATEGORY")]
        public string FlowCategory { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [SugarColumn(ColumnName = "F_STARTTIME")]
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [SugarColumn(ColumnName = "F_ENDTIME")]
        public DateTime? EndTime { get; set; }
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