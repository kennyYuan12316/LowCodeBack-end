using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;

namespace HSZ.WorkFlow.Entitys
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：流程评论
    /// </summary>
    [SugarTable("ZJN_FLOW_COMMENT")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class FlowCommentEntity : CLDEntityBase
    {
        /// <summary>
        /// 按钮上级
        /// </summary>
        [SugarColumn(ColumnName = "F_TASKID")]
        public string TaskId { get; set; }

        /// <summary>
        /// 文本
        /// </summary>
        [SugarColumn(ColumnName = "F_TEXT")]
        public string Text { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        [SugarColumn(ColumnName = "F_IMAGE")]
        public string Image { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILE")]
        public string File { get; set; }
    }
}
