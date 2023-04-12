using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.zjnWcsProcessConfig
{
    /// <summary>
    /// 业务路径配置表输出参数
    /// </summary>
    public class ZjnWcsProcessConfigInfoOutput
    {
        /// <summary>
        /// 任务路径配置主键
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 业务编号
        /// </summary>
        public string workNo { get; set; }
        
        /// <summary>
        /// 业务名称
        /// </summary>
        public string workName { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public string workType { get; set; }


        /// <summary>
        /// 物料类型
        /// </summary>
        public string GoodsType { get; set; }

        /// <summary>
        /// 所属库位
        /// </summary>
        public string workWarehouse { get; set; }
        
        /// <summary>
        /// 是否存在动态站点
        /// </summary>
        public bool workNodes { get; set; }
        
        /// <summary>
        /// 起点工位
        /// </summary>
        public string workStart { get; set; }
        
        /// <summary>
        /// 终点工位
        /// </summary>
        public string workEnd { get; set; }


        /// <summary>
        /// 站点数
        /// </summary>
        public int workPathcount { get; set; }

        /// <summary>
        /// 业务完整路径
        /// </summary>
        public string workPath { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string createUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public string createTime { get; set; }
        
        /// <summary>
        /// 修改用户
        /// </summary>
        public string lastModifyUserId { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        public string lastModifyTime { get; set; }
        
    }
}