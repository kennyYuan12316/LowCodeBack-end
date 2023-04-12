using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Dto.Permission.UserRelation
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：用户关系创建输入
    /// </summary>
    [SuppressSniffer]
    public class UserRelationCrInput
    {
        /// <summary>
        /// 关系类型
        /// </summary>
        public string objectType { get; set; }

        /// <summary>
        /// 用户数组
        /// </summary>
        public List<string> userIds { get; set; }
    }
}
