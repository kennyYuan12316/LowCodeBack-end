using HSZ.Common.Filter;
using SqlSugar;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.zjnWcsProcessConfig
{
    /// <summary>
    /// 业务路径配置表列表查询输入
    /// </summary>
    public class ZjnWcsProcessConfigListQueryInput : PageInputBase
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
        /// 业务编号
        /// </summary>
        public string F_work_no { get; set; }
        
        /// <summary>
        /// 业务名称
        /// </summary>
        public string F_work_name { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public string F_work_type { get; set; }


        /// <summary>
        /// 物料类型
        /// </summary>
        public string F_GoodsType { get; set; }

        /// <summary>
        /// 起点工位
        /// </summary>
        public string F_work_start { get; set; }
        
        /// <summary>
        /// 终点工位
        /// </summary>
        public string F_work_end { get; set; }
        
    }
}