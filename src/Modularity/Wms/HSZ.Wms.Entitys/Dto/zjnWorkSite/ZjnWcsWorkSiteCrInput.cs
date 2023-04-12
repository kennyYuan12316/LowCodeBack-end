using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWcsWorkSite
{
    /// <summary>
    /// 站点信息管理修改输入参数
    /// </summary>
    public class ZjnWcsWorkSiteCrInput
    {
        /// <summary>
        /// 站点ID
        /// </summary>
        public string stationId { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        public string capion { get; set; }
        
        /// <summary>
        /// 设备ID
        /// </summary>
        public string deviceId { get; set; }
        
        /// <summary>
        /// 有效
        /// </summary>
        public bool isActive { get; set; }
        
        /// <summary>
        /// 区域
        /// </summary>
        public string region { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string descrip { get; set; }
        
        /// <summary>
        /// 站类别
        /// </summary>
        public string stationType { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string createUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }

        /// <summary>
        /// 最后修改者
        /// </summary>
        public string modifiedUser { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? modifiedTime { get; set; }

        public int? IsDelete { get; set; }

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
        /// 行2
        /// </summary>
        public int? row2 { get; set; }

        /// <summary>
        /// 列2
        /// </summary>
        public int? cell2 { get; set; }

        /// <summary>
        /// 层2
        /// </summary>
        public int? layer2 { get; set; }

    }
}