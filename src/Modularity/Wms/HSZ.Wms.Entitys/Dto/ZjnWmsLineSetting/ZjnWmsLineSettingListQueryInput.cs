using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsLineSetting
{
    /// <summary>
    /// 线体信息配置列表查询输入
    /// </summary>
    public class ZjnWmsLineSettingListQueryInput : PageInputBase
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
        /// 线体编号
        /// </summary>
        public string F_LineNo { get; set; }
        
        /// <summary>
        /// 线体名称
        /// </summary>
        public string F_LineName { get; set; }
        /// <summary>
        /// 电芯类型
        /// </summary>
        public string F_GoodsType { get; set; }
        /// <summary>
        /// 线体缓存起点
        /// </summary>
        public string F_LineStart { get; set; }
        /// <summary>
        /// 线体缓存终点
        /// </summary>
        public string F_LineEnd { get; set; }
        /// <summary>
        /// 线体层
        /// </summary>
        public string F_LineLayer { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public int? F_EnabledMark { get; set; }

    }
}