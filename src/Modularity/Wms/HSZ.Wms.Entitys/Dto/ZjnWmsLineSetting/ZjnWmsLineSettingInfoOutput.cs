using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsLineSetting
{
    /// <summary>
    /// 线体信息配置输出参数
    /// </summary>
    public class ZjnWmsLineSettingInfoOutput
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 线体编号
        /// </summary>
        public string lineNo { get; set; }
        
        /// <summary>
        /// 线体名称
        /// </summary>
        public string lineName { get; set; }
        /// <summary>
        /// 电芯类型
        /// </summary>
        public string goodsType { get; set; }
        /// <summary>
        /// 线体缓存起点
        /// </summary>
        public string lineStart { get; set; }
        /// <summary>
        /// 线体缓存终点
        /// </summary>
        public string lineEnd { get; set; }
        /// <summary>
        /// 线体层
        /// </summary>
        public string lineLayer { get; set; }

        /// <summary>
        /// 线体最大任务（缓存）数
        /// </summary>
        public int? lineMaxWork { get; set; }
        
        /// <summary>
        /// 当前任务（缓存）数量
        /// </summary>
        public int? lineNowWork { get; set; }
        
        /// <summary>
        /// 线体描述
        /// </summary>
        public string description { get; set; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public int? enabledMark { get; set; }
        
    }
}