using HSZ.Common.Filter;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWcsPlcObject
{
    /// <summary>
    /// PLC对象表列表查询输入
    /// </summary>
    public class ZjnWcsPlcObjectListQueryInput : PageInputBase
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
        /// 区域
        /// </summary>
        public string Region { get; set; }
        
        /// <summary>
        /// DB
        /// </summary>
        public string Db { get; set; }


        /// <summary>
        /// 设备Id
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// PlcID
        /// </summary>
        public string PlcID { get; set; }

        /// <summary>
        /// 设备分组
        /// </summary>
        public string StackerGroup { get; set; }
    }
}