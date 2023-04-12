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
    /// 描 述：聊天会话
    /// </summary>
    [SugarTable("ZJN_BASE_IM_REPLY")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ImReplyEntity : EntityBase<string>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_UserId")]
        public string UserId { get; set; }

        /// <summary>
        /// 接收用户
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_ReceiveUserId")]
        public string ReceiveUserId { get; set; }

        /// <summary>
        /// 接收用户时间
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_ReceiveTime")]
        public DateTime? ReceiveTime { get; set; }
    }
}
