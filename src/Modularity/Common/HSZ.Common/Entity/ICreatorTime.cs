using HSZ.Dependency;
using SqlSugar;
using System;

namespace HSZ.Common.Entity
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：定义创建时间
    /// </summary>
    public interface ICreatorTime
    {
        /// <summary>
        /// 获取或设置 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreatorTime", ColumnDescription = "创建时间")]
        DateTime? CreatorTime { get; set; }
    }
}
