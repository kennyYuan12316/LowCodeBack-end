using HSZ.Common.Filter;
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
    /// 描 述：用户数据导出 输入
    /// </summary>
    public class UserExportDataInput: PageInputBase
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public string organizeId { get; set; }

        /// <summary>
        /// 导出类型 (0：分页数据，其他：全部数据)
        /// </summary>
        public string dataType { get; set; }

        /// <summary>
        /// 选择的导出 字段集合 按 , 号隔开
        /// </summary>
        public string selectKey { get; set; }
    }
}
