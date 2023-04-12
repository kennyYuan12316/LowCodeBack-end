using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Dto.Permission.Authorize
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：权限数据修改输入
    /// </summary>
    [SuppressSniffer]
    public class AuthorizeDataUpInput
    {
        /// <summary>
        /// 类型Position/Role/User
        /// </summary>
        public string objectType { get; set; }

        /// <summary>
        /// 按钮
        /// </summary>
        public List<string> button { get; set; }

        /// <summary>
        /// 列表
        /// </summary>
        public List<string> column { get; set; }

        /// <summary>
        /// 模块
        /// </summary>
        public List<string> module { get; set; }

        /// <summary>
        /// 表单
        /// </summary>
        public List<string> form { get; set; }

        /// <summary>
        /// 资源
        /// </summary>
        public List<string> resource { get; set; }
    }
}
