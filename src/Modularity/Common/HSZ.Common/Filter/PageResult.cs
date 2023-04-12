using HSZ.Dependency;
using Mapster;
using SqlSugar;
using System.Collections.Generic;

namespace HSZ.Common.Filter
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：分页结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SuppressSniffer]
    public class PageResult<T>
    {
        public PageResult pagination { get; set; }

        public List<T> list { get; set; }

        // <summary>
        /// 替换sqlsugar分页
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static dynamic SqlSugarPageResult(SqlSugarPagedList<T> page)
        {
            return new
            {
                pagination = page.pagination.Adapt<PageResult>(),
                list = page.list
            };
        }
    }

    /// <summary>
    /// 分页结果
    /// </summary>
    [SuppressSniffer]
    public class PageResult : PageResult<object>
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int pageIndex { get; set; }

        /// <summary>
        /// 页容量
        /// </summary>
        public int pageSize { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
        public int total { get; set; }
    }
}
