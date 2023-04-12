using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWcsWorkSite
{
    /// <summary>
    /// 站点信息管理列表查询输入
    /// </summary>
    public class ZjnWcsWorkSiteListQueryInput : PageInputBase
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
        /// 站点ID
        /// </summary>
        public string StationID { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        public string Capion { get; set; }
        
    }
}