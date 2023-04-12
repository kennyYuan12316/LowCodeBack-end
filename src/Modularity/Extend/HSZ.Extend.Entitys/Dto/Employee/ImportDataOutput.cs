using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.Extend.Entitys.Dto.Employee
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class ImportDataOutput
    {
        /// <summary>
        /// 导入失败信息
        /// </summary>
        public List<EmployeeListOutput> failResult { get; set; } = new List<EmployeeListOutput>();
        /// <summary>
        /// 失败条数
        /// </summary>
        public int fnum { get; set; }
        /// <summary>
        /// 导入是否成功（0：成功）
        /// </summary>
        public int resultType { get; set; }
        /// <summary>
        /// 成功条数
        /// </summary>
        public int snum { get; set; }
    }
}
