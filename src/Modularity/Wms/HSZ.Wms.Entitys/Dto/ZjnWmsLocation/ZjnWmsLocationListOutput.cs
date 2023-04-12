using System;

namespace HSZ.wms.Entitys.Dto.ZjnWmsLocation
{
    /// <summary>
    /// 货位信息输入参数
    /// </summary>
    public class ZjnWmsLocationListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 货位编号
        /// </summary>
        public string F_LocationNo { get; set; }
        
        /// <summary>
        /// 巷道编号
        /// </summary>
        public string F_AisleNo { get; set; }
        
        /// <summary>
        /// 行
        /// </summary>
        public int? F_Row { get; set; }
        
        /// <summary>
        /// 列
        /// </summary>
        public int? F_Cell { get; set; }
        
        /// <summary>
        /// 层
        /// </summary>
        public int? F_Layer { get; set; }
        
        /// <summary>
        /// 深
        /// </summary>
        public int? F_Depth { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string F_TrayNo { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int? F_LastStatus { get; set; }
        
        /// <summary>
        /// 货位状态：
        /// </summary>
        public string F_LocationStatus { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string F_Description { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public string F_EnabledMark { get; set; }

        /// <summary>
        /// 所属库位
        /// </summary>
        public string ByWarehouse { get; set; }

        /// <summary>
        /// 是否双托盘坐标
        /// </summary>
        public int F_IsDoubuleLocation { get; set; }

    }
}