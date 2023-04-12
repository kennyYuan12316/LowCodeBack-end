using HSZ.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.System.Entitys.Dto.System.ModuleDataAuthorizeScheme
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：功能权限数据计划信息输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleDataAuthorizeSchemeInfoOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 菜单id
        /// </summary>
        public string moduleId { get; set; }

        /// <summary>
        /// 方案名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 方案对象
        /// </summary>
        public string conditionJson { get; set; }

        /// <summary>
        /// 过滤条件
        /// </summary>
        public string conditionText { get; set; }
    }
}
