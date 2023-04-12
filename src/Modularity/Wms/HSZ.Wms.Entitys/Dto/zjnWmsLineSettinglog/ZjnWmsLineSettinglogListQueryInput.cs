using HSZ.Common.Filter;
using SqlSugar;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsLineSettinglog
{
    /// <summary>
    /// 线体物料绑定履历表列表查询输入
    /// </summary>
    public class ZjnWmsLineSettinglogListQueryInput : PageInputBase
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
        /// 
        /// </summary>
        public string F_LineNo { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_LineName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_TrayNo { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_GoodsType { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_GoodsCode { get; set; }
        /// <summary>
        /// 1-在线，2-已出
        /// </summary>
        public int? Status { get; set; }

    }
}