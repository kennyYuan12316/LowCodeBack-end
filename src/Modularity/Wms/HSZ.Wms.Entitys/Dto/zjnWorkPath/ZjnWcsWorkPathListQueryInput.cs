using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWcsWorkPath
{
    /// <summary>
    /// 路径信息管理列表查询输入
    /// </summary>
    public class ZjnWcsWorkPathListQueryInput : PageInputBase
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
        /// 路径编号 
        /// </summary>
        public string PathID { get; set; }
        
        /// <summary>
        /// 路径名称
        /// </summary>
        public string StationFrom { get; set; }
        
        /// <summary>
        /// 路径类型
        /// </summary>
        public string PathType { get; set; }
        
        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }
        
    }
}