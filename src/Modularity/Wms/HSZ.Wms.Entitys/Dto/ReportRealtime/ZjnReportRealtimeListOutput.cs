using System;

namespace HSZ.wms.Entitys.Dto.ZjnReportRealtime
{
    /// <summary>
    /// 实时库存输入参数
    /// </summary>
    public class ZjnReportRealtimeListOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 序号
        /// </summary>
        public int? F_Num { get; set; }
        
        /// <summary>
        /// 工厂
        /// </summary>
        public string F_Factory { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        public string F_ProductsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string F_ProductsName { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        public string F_Batch { get; set; }
        
        /// <summary>
        /// 质量状态
        /// </summary>
        public string F_Quality { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string F_ProductsUnit { get; set; }
        
        /// <summary>
        /// 总数库内
        /// </summary>
        public int? F_SumInside { get; set; }
        
        /// <summary>
        /// 托盘数库内
        /// </summary>
        public int? F_TrayCountInside { get; set; }
        
        /// <summary>
        /// 总数库外
        /// </summary>
        public int? F_SumOutside { get; set; }
        
        /// <summary>
        /// 托盘数库外
        /// </summary>
        public int? F_TrayCountOutside { get; set; }
        
        /// <summary>
        /// 总数移动中
        /// </summary>
        public int? F_SumMoving { get; set; }
        
        /// <summary>
        /// 托盘数移动中
        /// </summary>
        public int? F_TrayCountMoving { get; set; }
        
        /// <summary>
        /// 总数全部
        /// </summary>
        public int? F_SumTotal { get; set; }
        
        /// <summary>
        /// 托盘数全部
        /// </summary>
        public int? F_TrayCountTotal { get; set; }
        
    }
}