using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.System.Entitys.Enum
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：查询方法
    /// </summary>
    public enum SearchMethod
    {
        Contains,           //like
        Equal,              //等于
        NotEqual,           //不等于
        LessThan,           //小于
        LessThanOrEqual,    //小于等于
        GreaterThan,        //大于
        GreaterThanOrEqual,  //大于等于
        In                  //In
    }
}
