using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Dto.System.ModuleColumn
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：功能列表行为处理输入
    /// </summary>
    [SuppressSniffer]
    public class ModuleFormActionsBatchInput
    {
        /// <summary>
        /// 功能主键
        /// </summary>
        public string moduleId { get; set; }

        /// <summary>
        /// 表格描述	
        /// </summary>
        public string bindTableName { get; set; }

        /// <summary>
        /// 字段数据
        /// </summary>
        public List<Form> formJson { get; set; }

        /// <summary>
        /// 绑定表格
        /// </summary>
        public string bindTable { get; set; }
    }

    /// <summary>
    /// 列字段数据
    /// </summary>
    [SuppressSniffer]
    public class Form
    {
        /// <summary>
        /// 字段名称
        /// </summary
        public string enCode { get; set; }

        /// <summary>
        /// 字段注解
        /// </summary>
        public string fullName { get; set; }
    }
}
