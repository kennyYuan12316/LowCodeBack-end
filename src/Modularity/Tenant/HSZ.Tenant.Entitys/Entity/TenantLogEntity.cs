using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;
using System;

namespace HSZ.Tenant.Entitys
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：租户日志
    /// </summary>
    [SugarTable("BASE_TENANTLOG")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class TenantLogEntity: EntityBase<string>
    {
        /// <summary>
        /// 租户主键
        /// </summary>
        [SugarColumn(ColumnName = "F_TENANTID")]
        public string TenantId { get; set; }
        /// <summary>
        /// 登陆账户
        /// </summary>
        [SugarColumn(ColumnName = "F_LOGINACCOUNT")]
        public string LoginAccount { get; set; }
        /// <summary>
        /// 登陆IP地址
        /// </summary>
        [SugarColumn(ColumnName = "F_LOGINIPADDRESS")]
        public string LoginIPAddress { get; set; }
        /// <summary>
        /// 登陆IP归属地
        /// </summary>
        [SugarColumn(ColumnName = "F_LOGINIPADDRESSNAME")]
        public string LoginIPAddressName { get; set; }
        /// <summary>
        /// 来源网站
        /// </summary>
        [SugarColumn(ColumnName = "F_LOGINSOURCEWEBSITE")]
        public string LoginSourceWebsite { get; set; }
        /// <summary>
        /// 登陆时间
        /// </summary>
        [SugarColumn(ColumnName = "F_LOGINTIME")]
        public DateTime? LoginTime { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
