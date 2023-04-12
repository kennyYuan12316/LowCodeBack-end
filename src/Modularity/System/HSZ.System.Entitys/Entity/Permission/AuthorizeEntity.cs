using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;

namespace HSZ.System.Entitys.Permission
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：实体类：操作权限
    /// </summary>
    [SugarTable("ZJN_BASE_AUTHORIZE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class AuthorizeEntity : CEntityBase
    {
        /// <summary>
        /// 项目类型：menu、module、button、column、resource
        /// </summary>
        [SugarColumn(ColumnName = "F_ITEMTYPE")]
        public string ItemType { get; set; }

        /// <summary>
        /// 项目主键
        /// </summary>
        [SugarColumn(ColumnName = "F_ITEMID")]
        public string ItemId { get; set; }

        /// <summary>
        /// 对象类型：Role、Position、User
        /// </summary>
        [SugarColumn(ColumnName = "F_OBJECTTYPE")]
        public string ObjectType { get; set; }

        /// <summary>
        /// 对象主键
        /// </summary>
        [SugarColumn(ColumnName = "F_OBJECTID")]
        public string ObjectId { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }

        /// <summary>
        /// A集合是否存在B集合
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is AuthorizeEntity)
            {
                AuthorizeEntity authorizeEntity = obj as AuthorizeEntity;
                return ItemType == authorizeEntity.ItemType && ItemId == authorizeEntity.ItemId && ObjectId == authorizeEntity.ObjectId && ObjectType == authorizeEntity.ObjectType;
            }
            return false;
        }

        /// <summary>
        /// 实体哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ItemType.GetHashCode() ^ ItemId.GetHashCode() ^ ObjectId.GetHashCode() ^ ObjectType.GetHashCode();
        }
    }
}
