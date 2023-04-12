using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;
using System;
using Yitter.IdGenerator;

namespace HSZ.System.Entitys.Permission
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：用户关系映射
    /// </summary>
    [SugarTable("ZJN_BASE_ORGANIZE_RELATION")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class OrganizeRelationEntity
    {
        /// <summary>
        /// 获取或设置 自然主键
        /// </summary>
        [SugarColumn(ColumnName = "F_ID", ColumnDescription = "自然主键")]
        public string Id { get; set; }

        /// <summary>
        /// 获取或设置 组织Id
        /// </summary>
        [SugarColumn(ColumnName = "F_ORGANIZE_ID", ColumnDescription = "组织Id")]
        public string OrganizeId { get; set; }

        /// <summary>
        /// 对象类型（角色：Role、岗位：Position）
        /// </summary>
        [SugarColumn(ColumnName = "F_OBJECT_TYPE", ColumnDescription = "对象类型（角色：Role、岗位：Position）")]
        public string ObjectType { get; set; }

        /// <summary>
        /// 获取或设置 对象主键
        /// </summary>
        [SugarColumn(ColumnName = "F_OBJECT_ID", ColumnDescription = "对象主键")]
        public string ObjectId { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnName = "F_SORT_CODE")]
        public long? SortCode { get; set; }

        /// <summary>
        /// 获取或设置 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CREATOR_TIME", ColumnDescription = "创建时间")]
        public DateTime? CreatorTime { get; set; }

        /// <summary>
        /// 获取或设置 创建用户
        /// </summary>
        [SugarColumn(ColumnName = "F_CREATOR_USER_ID", ColumnDescription = "创建用户")]
        public string CreatorUserId { get; set; }

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
