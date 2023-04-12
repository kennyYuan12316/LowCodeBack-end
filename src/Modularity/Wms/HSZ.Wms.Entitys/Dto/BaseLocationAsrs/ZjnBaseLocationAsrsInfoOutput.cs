using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseLocationAsrs
{
    /// <summary>
    /// 立库货位信息输出参数
    /// </summary>
    public class ZjnBaseLocationAsrsInfoOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 货位编号
        /// </summary>
        public string locationNo { get; set; }
        
        /// <summary>
        /// 设备编号（堆垛机）
        /// </summary>
        public string deviceNo { get; set; }
        
        /// <summary>
        /// 行
        /// </summary>
        public int? row { get; set; }
        
        /// <summary>
        /// 列
        /// </summary>
        public int? cell { get; set; }
        
        /// <summary>
        /// 层
        /// </summary>
        public int? layer { get; set; }
        
        /// <summary>
        /// 深
        /// </summary>
        public int? depth { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string trayNo { get; set; }
        
        /// <summary>
        /// 上次货位状态
        /// </summary>
        public int? lastStatus { get; set; }
        
        /// <summary>
        /// 货位状态：0空 1满 2未满 3故障 4火警 5静置中 6静置完成 7预约 8禁用
        /// </summary>
        public string locationStatus { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        public string createUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? enabledMark { get; set; }
        
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string byWarehouse { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string Legion { get; set; }


    }
}