using HSZ.Common.Const;
using HSZ.Dependency;
using SqlSugar;
using System;
using Yitter.IdGenerator;

namespace HSZ.Common.Entity
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：创实体基类
    /// </summary>
    [SuppressSniffer]
    public abstract class CEntityBase : EntityBase<string>, ICreatorTime
    {
        /// <summary>
        /// 获取或设置 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CREATORTIME", ColumnDescription = "创建时间")]
        public virtual DateTime? CreatorTime { get; set; }

        /// <summary>
        /// 获取或设置 创建用户
        /// </summary>
        [SugarColumn(ColumnName = "F_CREATORUSERID", ColumnDescription = "创建用户")]
        public virtual string CreatorUserId { get; set; }

        /// <summary>
        /// 创建
        /// </summary>
        public virtual void Creator()
        {
            var userId = App.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value;
            this.CreatorTime = DateTime.Now;
            this.Id = YitIdHelper.NextId().ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                this.CreatorUserId = userId;
            }
        }
    }
}
