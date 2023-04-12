using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Dto.Permission.Authorize
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：列表字段权限输入
    /// </summary>
    [SuppressSniffer]
    public class ColumnsPurviewDataUpInput
    {
        /// <summary>
        /// 模块ID
        /// </summary>
        public string moduleId { get; set; }

        /// <summary>
        /// 列表字段数组
        /// </summary>
        public string fieldList { get; set; }

    }

    public class FieldList
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string prop { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool visible { get; set; }
    }
}
