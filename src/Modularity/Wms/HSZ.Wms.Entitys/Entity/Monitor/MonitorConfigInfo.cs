using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 监控配置
    /// </summary>
    public class MonitorConfigInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        /// <summary>
        /// 楼层名
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 界面元素转成HTML
        /// </summary>
        public string? Json { get; set; }
    }
}
