using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;

namespace HSZ.Extend.Entitys
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：工作日志
    /// </summary>
    [SugarTable("ZJN_EXT_WORK_LOG")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class WorkLogEntity : CLDEntityBase
    {
        /// <summary>
        /// 日志标题
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_TITLE")]
        public string Title { get; set; }
        /// <summary>
        /// 今天内容
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_TODAYCONTENT")]
        public string TodayContent { get; set; }
        /// <summary>
        /// 明天内容
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_TOMORROWCONTENT")]
        public string TomorrowContent { get; set; }
        /// <summary>
        /// 遇到问题
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_QUESTION")]
        public string Question { get; set; }
        /// <summary>
        /// 发送给谁
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_TOUSERID")]
        public string ToUserId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }
    }
}