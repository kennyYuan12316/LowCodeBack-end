using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.System.Interfaces.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：数据接口日志
    /// </summary>
    public interface IDataInterfaceLogService
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="id">接口id</param>
        /// <param name="sw">请求时间</param>
        /// <returns></returns>
        Task CreateLog(string id, Stopwatch sw);
    }
}
