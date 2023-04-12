using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.Permission.Department
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：部门信息输出
    /// </summary>
    [SuppressSniffer]
    public class DepartmentInfoOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        public string parentId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 部门编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 主管ID
        /// </summary>
        public string managerId { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public long? sortCode { get; set; }
    }
}
