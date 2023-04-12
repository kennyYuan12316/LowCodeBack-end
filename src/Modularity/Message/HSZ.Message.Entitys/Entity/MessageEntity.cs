using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;

namespace HSZ.Message.Entitys
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：消息实例
    /// </summary>
    [SugarTable("ZJN_BASE_MESSAGE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class MessageEntity : CLDEntityBase
    {
        /// <summary>
        /// 类别：1-通知公告，2-系统消息、3-私信消息
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE")]
        public int? Type { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [SugarColumn(ColumnName = "F_TITLE")]
        public string Title { get; set; } = "";

        /// <summary>
        /// 正文
        /// </summary>
        [SugarColumn(ColumnName = "F_BODYTEXT")]
        public string BodyText { get; set; }

        /// <summary>
        /// 优先
        /// </summary>
        [SugarColumn(ColumnName = "F_PRIORITYLEVEL")]
        public int? PriorityLevel { get; set; }

        /// <summary>
        /// 收件用户
        /// </summary>
        [SugarColumn(ColumnName = "F_TOUSERIDS")]
        public string ToUserIds { get; set; }

        /// <summary>
        /// 是否阅读
        /// </summary>
        [SugarColumn(ColumnName = "F_ISREAD")]
        public int? IsRead { get; set; }

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
        /// 附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILES")]
        public string Files{ get; set; }
    }
}
