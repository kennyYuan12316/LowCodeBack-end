using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;
using System;

namespace HSZ.Message.Entitys
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：消息接收
    /// </summary>
    [SugarTable("ZJN_BASE_MESSAGE_RECEIVE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class MessageReceiveEntity : EntityBase<string>
    {
        /// <summary>
        /// 消息主键
        /// </summary>
        [SugarColumn(ColumnName = "F_MESSAGEID")]
        public string MessageId { get; set; }

        /// <summary>
        /// 用户主键
        /// </summary>
        [SugarColumn(ColumnName = "F_USERID")]
        public string UserId { get; set; }

        /// <summary>
        /// 是否阅读
        /// </summary>
        [SugarColumn(ColumnName = "F_ISREAD")]
        public int? IsRead { get; set; }

        /// <summary>
        /// 阅读时间
        /// </summary>
        [SugarColumn(ColumnName = "F_READTIME")]
        public DateTime? ReadTime { get; set; }

        /// <summary>
        /// 阅读次数
        /// </summary>
        [SugarColumn(ColumnName = "F_READCOUNT")]
        public int? ReadCount { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        [SugarColumn(ColumnName = "F_BODYTEXT")]
        public string BodyText { get; set; }
    }
}
