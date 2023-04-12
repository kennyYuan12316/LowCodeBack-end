using System;

namespace HSZ.wms.Entitys.Dto.ZjnWmsLineSetting
{
    /// <summary>
    /// 线体信息配置输入参数
    /// </summary>
    public class ZjnWmsLineSettingListOutput
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 线体编号
        /// </summary>
        public string F_LineNo { get; set; }
        
        /// <summary>
        /// 线体名称
        /// </summary>
        public string F_LineName { get; set; }
        /// <summary>
        /// 电芯类型
        /// </summary>
        public string F_GoodsType { get; set; }
        /// <summary>
        /// 物料类型名称
        /// </summary>
        public string F_GoodsTypeName { get; set; }
        
        /// <summary>
        /// 线体缓存起点
        /// </summary>
        public string F_LineStart { get; set; }
        /// <summary>
        /// 线体缓存终点
        /// </summary>
        public string F_LineEnd { get; set; }
        /// <summary>
        /// 线体层
        /// </summary>
        public string F_LineLayer { get; set; }

        /// <summary>
        /// 线体最大任务（缓存）数
        /// </summary>
        public int? F_LineMaxWork { get; set; }
        
        /// <summary>
        /// 当前任务（缓存）数量
        /// </summary>
        public int? F_LineNowWork { get; set; }
        
        /// <summary>
        /// 线体描述
        /// </summary>
        public string F_Description { get; set; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public int? F_EnabledMark { get; set; }
    }
}