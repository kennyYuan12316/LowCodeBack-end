using HSZ.Dependency;
using System.ComponentModel.DataAnnotations;

namespace HSZ.System.Entitys.Dto.System.ModuleDataAuthorize
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：功能数据权限创建输入
    /// </summary>
    [SuppressSniffer]
    public class ModuleDataAuthorizeCrInput
    {
        /// <summary>
        /// 功能主键
        /// </summary>
        public string moduleId { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 字段注解
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 条件符号
        /// </summary>
        public string conditionSymbol { get; set; }

        /// <summary>
        /// 条件内容	
        /// </summary>
        public string conditionText { get; set; }

        /// <summary>
        /// 字段说明
        /// </summary>
        public string description { get; set; }
    }
}
