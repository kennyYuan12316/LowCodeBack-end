using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.ModuleColumn
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：功能列表信息输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleColumnInfoOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 功能ID
        /// </summary>
        public string moduleId { get; set; }

        /// <summary>
        /// 绑定表格
        /// </summary>
        public string bindTable { get; set; }

        /// <summary>
        /// 表格描述
        /// </summary>
        public string bindTableName { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 字段注解
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 字段状态
        /// </summary>
        public int enabledMark { get; set; }

        /// <summary>
        /// 字段说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long? sortCode { get; set; }
    }
}
