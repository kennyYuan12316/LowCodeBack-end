using System;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Dto.System.SysConfig
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：赋予超级管理员 输入
    /// </summary>
    public class SetAdminListInput
    {
        /// <summary>
        /// 赋予超级管理员 Id 集合
        /// </summary>
        public List<string> adminIds { get; set; }

    }
}
