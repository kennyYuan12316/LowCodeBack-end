using System;

namespace HSZ.wms.Entitys.Dto.ZjnWmsLineSettinglog
{
    /// <summary>
    /// 线体物料绑定履历表输入参数
    /// </summary>
    public class ZjnWmsLineSettinglogListOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_LineNo { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_LineName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_TrayNo { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_GoodsType { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_GoodsCode { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_LineStart { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_LineEnd { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_LineLayer { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int? F_LineMaxWork { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int? F_LineNowWork { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_Description { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string F_Expand { get; set; }
        
        /// <summary>
        /// 1-在线，2-已出
        /// </summary>
        public int? F_Status { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime? F_OutTime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
    }
}