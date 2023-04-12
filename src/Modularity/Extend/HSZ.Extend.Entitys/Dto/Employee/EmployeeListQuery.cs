using HSZ.Common.Filter;
using HSZ.Dependency;

namespace HSZ.Extend.Entitys.Dto.Employee
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：获取职员列表(分页)
    /// </summary>
    [SuppressSniffer]
    public class EmployeeListQuery : PageInputBase
    {
        /// <summary>
        /// 查询内容
        /// </summary>
        public string condition { get; set; }
        /// <summary>
        /// 查询字段
        /// </summary>
        public string selectKey { get; set; }
        /// <summary>
        /// 是否分页（0：分页）
        /// </summary>
        public string dataType { get; set; }

    }
}
