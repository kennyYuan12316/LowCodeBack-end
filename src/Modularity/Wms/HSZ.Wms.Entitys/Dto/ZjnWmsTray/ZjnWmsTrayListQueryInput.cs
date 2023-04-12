using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsTray
{
    /// <summary>
    /// 托盘信息列表查询输入
    /// </summary>
    public class ZjnWmsTrayListQueryInput : PageInputBase
    {
        /// <summary>
        /// 选择导出数据key
        /// </summary>
        public string selectKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int dataType { get; set; }

        /// <summary>
        /// 托盘编号
        /// </summary>
        public string F_TrayNo { get; set; }

        /// <summary>
        /// 托盘名称
        /// </summary>
        public string F_TrayName { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string F_Type { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string F_TrayStates { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public string F_EnabledMark { get; set; }


        /// <summary>
        /// 托盘属性
        /// </summary>
        public string F_TrayAttr { get; set; }
    }
}