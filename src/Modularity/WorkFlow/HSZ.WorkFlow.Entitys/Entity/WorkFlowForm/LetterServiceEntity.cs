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
    /// 描 述：发文单
    /// </summary>
    [SugarTable("ZJN_WFORM_LETTER_SERVICE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class LetterServiceEntity : EntityBase<string>
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
        /// 主办单位
        /// </summary>
        [SugarColumn(ColumnName = "F_HOSTUNIT")]
        public string HostUnit { get; set; }
        /// <summary>
        /// 发文标题
        /// </summary>
        [SugarColumn(ColumnName = "F_TITLE")]
        public string Title { get; set; }
        /// <summary>
        /// 发文字号
        /// </summary>
        [SugarColumn(ColumnName = "F_ISSUEDNUM")]
        public string IssuedNum { get; set; }
        /// <summary>
        /// 发文日期
        /// </summary>
        [SugarColumn(ColumnName = "F_WRITINGDATE")]
        public DateTime? WritingDate { get; set; }
        /// <summary>
        /// 份数
        /// </summary>
        [SugarColumn(ColumnName = "F_SHARENUM")]
        public string ShareNum { get; set; }
        /// <summary>
        /// 主送
        /// </summary>
        [SugarColumn(ColumnName = "F_MAINDELIVERY")]
        public string MainDelivery { get; set; }
        /// <summary>
        /// 抄送
        /// </summary>
        [SugarColumn(ColumnName = "F_COPY")]
        public string Copy { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
    }
}
