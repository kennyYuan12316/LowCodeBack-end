using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;

namespace HSZ.System.Entitys.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：消息模板
    /// </summary>
    [SugarTable("ZJN_BASE_MESSAGE_TEMPLATE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class MessageTemplateEntity : CLDEntityBase
    {
        /// <summary>
        /// 分类（数据字典）
        /// </summary>
        [SugarColumn(ColumnName = "F_CATEGORY")]        
        public string Category { get; set; }
        
        /// <summary>
        /// 模板名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]        
        public string FullName { get; set; }
        
        /// <summary>
        /// 标题
        /// </summary>
        [SugarColumn(ColumnName = "F_TITLE")]        
        public string Title { get; set; } = "";

        /// <summary>
        /// 是否站内信
        /// </summary>
        [SugarColumn(ColumnName = "F_ISSTATIONLETTER")]        
        public int? IsStationLetter { get; set; }
        
        /// <summary>
        /// 是否邮箱
        /// </summary>
        [SugarColumn(ColumnName = "F_ISEMAIL")]        
        public int? IsEmail { get; set; }
        
        /// <summary>
        /// 是否企业微信
        /// </summary>
        [SugarColumn(ColumnName = "F_ISWECOM")]        
        public int? IsWeCom { get; set; }
        
        /// <summary>
        /// 是否钉钉
        /// </summary>
        [SugarColumn(ColumnName = "F_ISDINGTALK")]        
        public int? IsDingTalk { get; set; }
        
        /// <summary>
        /// 是否短信
        /// </summary>
        [SugarColumn(ColumnName = "F_ISSMS")]        
        public int? IsSms { get; set; }
        
        /// <summary>
        /// 短信模板ID
        /// </summary>
        [SugarColumn(ColumnName = "F_SMSID")]        
        public string SmsId { get; set; }
        
        /// <summary>
        /// 模板参数JSON
        /// </summary>
        [SugarColumn(ColumnName = "F_TEMPLATEJSON")]        
        public string TemplateJson { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTENT")]
        public string Content { get; set; } = "";
    }
}