using HSZ.Dependency;
using System;

namespace HSZ.Extend.Entitys.Dto.TableExample
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：新建项目
    /// </summary>
    [SuppressSniffer]
    public class TableExampleRowUpInput
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string projectName { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        public string projectCode { get; set; }
        /// <summary>
        /// 项目类型
        /// </summary>
        public string projectType { get; set; }
        /// <summary>
        /// 项目阶段
        /// </summary>
        public string projectPhase { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string customerName { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string principal { get; set; }
        /// <summary>
        /// 立项人
        /// </summary>
        public string jackStands { get; set; }
        /// <summary>
        /// 交付日期
        /// </summary>
        public DateTime? interactionDate { get; set; }
        /// <summary>
        /// 费用金额
        /// </summary>
        public decimal? costAmount { get; set; }
        /// <summary>
        /// 已用金额
        /// </summary>
        public decimal? tunesAmount { get; set; }
        /// <summary>
        /// 预计收入
        /// </summary>
        public decimal? projectedIncome { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string oper { get; set; }
    }
}
