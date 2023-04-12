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
    /// 描 述：租户信息
    /// </summary>
    [SugarTable("BASE_TENANT")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class TenantEntity:CLDEntityBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        [SugarColumn(ColumnName = "F_COMPANYNAME")]
        public string CompanyName { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        [SugarColumn(ColumnName = "F_EXPIRESTIME")]
        public DateTime? ExpiresTime { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        [SugarColumn(ColumnName = "F_DBNAME")]
        public string DbName { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        [SugarColumn(ColumnName = "F_IPADDRESS")]
        public string IPAddress { get; set; }
        /// <summary>
        /// ip归属地
        /// </summary>
        [SugarColumn(ColumnName = "F_IPADDRESSNAME")]
        public string IPAddressName { get; set; }
        /// <summary>
        /// 来源网站
        /// </summary>
        [SugarColumn(ColumnName = "F_SOURCEWEBSITE")]
        public string SourceWebsite { get; set; }
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
    }
}
