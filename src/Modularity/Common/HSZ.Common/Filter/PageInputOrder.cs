using HSZ.Dependency;

namespace HSZ.Common.Filter
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：通用输入帮助类
    /// </summary>
    [SuppressSniffer]
    public class PageInputOrder
    {
        /// <summary>
        /// 排序方式(默认降序)
        /// </summary>
        /// <param name="pageInput"></param>
        /// <param name="descSort">是否降序</param>
        /// <returns></returns>
        public static string OrderBuilder(PageInputBase pageInput, bool descSort = true)
        {
            // 约定默认每张表都有Id排序
            var orderStr = descSort ? "Id Desc" : "Id Asc";

            // 排序是否可用-排序字段和排序顺序都为非空才启用排序            
            if (!string.IsNullOrEmpty(pageInput.sidx) && !string.IsNullOrEmpty(pageInput.sort))
            {
                orderStr = $"{pageInput.sidx} {(pageInput.sort)}";
            }
            return orderStr;
        }
    }
}
