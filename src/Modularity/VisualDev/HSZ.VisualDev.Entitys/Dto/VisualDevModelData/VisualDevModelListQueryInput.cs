using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.VisualDev.Entitys.Dto.VisualDevModelData
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：在线开发功能模块列表查询输入
    /// </summary>
    public class VisualDevModelListQueryInput : PageInputBase
    {
        /// <summary>
        /// 动态搜索对像
        /// </summary>
        public string json { get; set; }

        /// <summary>
        /// 菜单ID
        /// </summary>
        public string menuId { get; set; }

        /// <summary>
        /// 选择导出数据key
        /// </summary>
        public List<string> selectKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string dataType { get; set; } = "0";
    }
}
