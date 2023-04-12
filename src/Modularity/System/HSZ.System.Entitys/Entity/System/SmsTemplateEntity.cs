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
    /// 描 述：ZJN_BASE_SMS_TEMPLATE
    /// </summary>
    [SugarTable("ZJN_BASE_SMS_TEMPLATE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class SmsTemplateEntity : CLDEntityBase
    {
        
        /// <summary>
        /// 短信提供商
        /// </summary>
        [SugarColumn(ColumnName = "F_COMPANY")]        
        public int? Company { get; set; }
        
        /// <summary>
        /// 应用编号
        /// </summary>
        [SugarColumn(ColumnName = "F_APPID")]        
        public string AppId { get; set; }
        
        /// <summary>
        /// 签名内容
        /// </summary>
        [SugarColumn(ColumnName = "F_SIGNCONTENT")]        
        public string SignContent { get; set; }
        
        /// <summary>
        /// 模板编号
        /// </summary>
        [SugarColumn(ColumnName = "F_TEMPLATEID")]        
        public string TemplateId { get; set; }
        
        /// <summary>
        /// 模板参数JSON
        /// </summary>
        [SugarColumn(ColumnName = "F_TEMPLATEJSON")]        
        public string TemplateJson { get; set; }
        
        /// <summary>
        /// 模板名称
        /// </summary>
        [SugarColumn(ColumnName = "F_TEMPLATENAME")]        
        public string TemplateName { get; set; }
        
    }
}