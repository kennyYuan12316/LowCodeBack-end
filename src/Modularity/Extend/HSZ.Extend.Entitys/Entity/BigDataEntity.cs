using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSZ.Extend.Entitys
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：大数据测试
    /// </summary>
    [SugarTable("ZJN_EXT_BIG_DATA")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class BigDataEntity : EntityBase<string>
    {
        /// <summary>
        /// 编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CREATORTIME")]
        public DateTime? CreatorTime { get; set; }
    }
}
