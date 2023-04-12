using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Dto.Permission.Authorize
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：权限数据输出
    /// </summary>
    [SuppressSniffer]
    public class AuthorizeDataOutput
    {
        /// <summary>
        /// 树形结构
        /// </summary>
        public List<AuthorizeDataModelOutput> list { get; set; }

        /// <summary>
        /// 已选中ID
        /// </summary>
        public List<string> ids { get; set; }

        /// <summary>
        /// 所有id
        /// </summary>
        public List<string> all { get; set; }
    }
}
