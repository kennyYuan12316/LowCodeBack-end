using HSZ.Common.Util;
using HSZ.Dependency;
using Newtonsoft.Json;

namespace HSZ.System.Entitys.Model.Permission.UsersCurrent
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：当前用户权限模型
    /// </summary>
    [SuppressSniffer]
    public class UsersCurrentAuthorizeMoldel : TreeModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }

        /// <summary>
        /// 功能主键
        /// </summary>
        [JsonIgnore]
        public string moduleId { get; set; }
    }
}
