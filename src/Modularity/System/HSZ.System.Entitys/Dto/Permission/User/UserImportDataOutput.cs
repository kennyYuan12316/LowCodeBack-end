using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.System.Entitys.Dto.Permission.User
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：用户数据 导出 结果 输出
    /// </summary>
    public class UserImportResultOutput
    {
        /// <summary>
        /// 导入成功条数
        /// </summary>
        public int snum { get; set; }

        /// <summary>
        /// 导入失败条数
        /// </summary>
        public int fnum { get; set; }

        /// <summary>
        /// 导入结果状态(0：成功，1：失败)
        /// </summary>
        public int resultType { get; set; }

        /// <summary>
        /// 失败结果集合
        /// </summary>
        public List<UserListImportDataInput> failResult { get; set; }
    }
}
