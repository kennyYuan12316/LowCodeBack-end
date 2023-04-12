using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsLocation
{
    /// <summary>
    /// 货位信息输出参数
    /// </summary>
    public class ZjnWmsLocationInfoOutput
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
        /// 巷道编号
        /// </summary>
        //[JsonIgnore]
        //public string aisleNoList { get; set; }

        /// <summary>
        /// 巷道编号
        /// </summary>
        //public List<string> aisleNo { get; set; }

        /// <summary>
        /// 巷道编号
        /// </summary>
        public string aisleNo { get; set; }

        /// <summary>
        /// 巷道编号
        /// </summary>
        //public string aisleNo { get; set; }

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
        /// 
        /// </summary>
        public string lastStatus { get; set; }
        
        /// <summary>
        /// 货位状态：0空 1满 2未满 3故障 4火警 5静置中 6静置完成 7预约 8禁用
        /// </summary>
        public string locationStatus { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        
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
        /// 所属库位
        /// </summary>
        public string ByWarehouse { get; set; }

        /// <summary>
        /// 是否双托盘坐标
        /// </summary>
        public int IsDoubuleLocation { get; set; }

    }
}