using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.ModuleForm
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：表单权限列表输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleFormListOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 字段注解	
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 菜单id
        /// </summary>
        public string moduleId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long? sortCode { get; set; }
    }
}
