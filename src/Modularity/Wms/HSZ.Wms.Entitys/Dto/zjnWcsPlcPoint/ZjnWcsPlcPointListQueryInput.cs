using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWcsPlcPoint
{
    /// <summary>
    /// PLC点位表列表查询输入
    /// </summary>
    public class ZjnWcsPlcPointListQueryInput : PageInputBase
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
        /// 名称
        /// </summary>
        public string Caption { get; set; }
        
        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }
        
        /// <summary>
        /// DB
        /// </summary>
        public string Db { get; set; }

        /// <summary>
        /// PlcId
        /// </summary>
        public string PlcID { get; set; }

        /// <summary>
        /// 起点
        /// </summary>
        public int? Start { get; set; }

        /// <summary>
        /// 对象名称，长
        /// </summary>
        public string ObjType { get; set; }

    }
}