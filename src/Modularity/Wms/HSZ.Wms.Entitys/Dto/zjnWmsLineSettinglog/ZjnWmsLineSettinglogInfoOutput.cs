using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsLineSettinglog
{
    /// <summary>
    /// 线体物料绑定履历表输出参数
    /// </summary>
    public class ZjnWmsLineSettinglogInfoOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string lineNo { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string lineName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string trayNo { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string goodsType { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string goodsCode { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string lineStart { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string lineEnd { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string lineLayer { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int? lineMaxWork { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int? lineNowWork { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string expand { get; set; }
        
        /// <summary>
        /// 1-在线，2-已出
        /// </summary>
        public int? status { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime? outTime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime? createTime { get; set; }
        
    }
}