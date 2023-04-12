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
    /// 描 述：在线聊天
    /// </summary>
    [SugarTable("ZJN_BASE_IM_CONTENT")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class IMContentEntity : EntityBase<string>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_SENDUSERID")]
        public string SendUserId { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_SENDTIME")]
        public DateTime? SendTime { get; set; }

        /// <summary>
         /// 接收者
         /// </summary>
         /// <returns></returns>
        [SugarColumn(ColumnName = "F_RECEIVEUSERID")]
        public string ReceiveUserId { get; set; }
        /// <summary>
        /// 接收时间
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_RECEIVETIME")]
        public DateTime? ReceiveTime { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_CONTENT")]
        public string Content { get; set; }

        /// <summary>
        /// 内容类型：text、img、file
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTENTTYPE")]
        public string ContentType { get; set; }

        /// <summary>
        /// 状态（0:未读、1：已读）
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_STATE")]
        public int? State { get; set; }
    }
}