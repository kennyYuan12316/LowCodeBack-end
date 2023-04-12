using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWcsWorkPath
{
    /// <summary>
    /// 路径信息管理修改输入参数
    /// </summary>
    public class ZjnWcsWorkPathCrInput
    {
        /// <summary>
        /// 路径编号 
        /// </summary>
        public string pathId { get; set; }
        
        /// <summary>
        /// 路径名称
        /// </summary>
        public string stationFrom { get; set; }
        
        /// <summary>
        /// 起点站点
        /// </summary>
        public string deviceFrom { get; set; }
        
        /// <summary>
        /// 起点设备
        /// </summary>
        public string stationTo { get; set; }
        
        /// <summary>
        /// 终点站点
        /// </summary>
        public string deviceTo { get; set; }
        
        /// <summary>
        /// 路径类型
        /// </summary>
        public string pathType { get; set; }
        
        /// <summary>
        /// 有效
        /// </summary>
        public int? isActive { get; set; }
        
        /// <summary>
        /// 区域
        /// </summary>
        public string region { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string descrip { get; set; }
        
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
        
    }
}