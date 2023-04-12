using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneLocation
{
    /// <summary>
    /// 货位信息修改输入参数
    /// </summary>
    public class ZjnPlaneLocationCrInput
    {
        /// <summary>
        /// 货位编号
        /// </summary>
        public string locationNo { get; set; }
        
        /// <summary>
        /// 巷道编号
        /// </summary>
        public List<string> aisleNo { get; set; }
        /// <summary>
        /// 巷道编号
        /// </summary>
        public string AisleNos { get; set; }

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
        public int? lastStatus { get; set; }
        
        /// <summary>
        /// 货位状态：详见数据字典，我懒得写了
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

    }
}