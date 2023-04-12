using HSZ.Common.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.VisualDev.Entitys.Dto.VisualDev
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：表单和弹窗 分页查询 输入和选中回写 输入
    /// </summary>
    public class VisualDevDataFieldDataListInput : PageInputBase
    {
        /// <summary>
        /// 查询 字段名
        /// </summary>
        public string relationField { get; set; }


        /// <summary>
        /// 弹窗选中 值
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 弹窗选中 字段名
        /// </summary>
        public string propsValue { get; set; }

        /// <summary>
        /// 设定显示的所有列  以 , 号隔开
        /// </summary>
        public string columnOptions { get; set; }
    }
}
