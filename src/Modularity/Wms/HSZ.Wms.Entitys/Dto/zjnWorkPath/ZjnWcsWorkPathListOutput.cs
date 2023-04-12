using System;

namespace HSZ.wms.Entitys.Dto.ZjnWcsWorkPath
{
    /// <summary>
    /// 路径信息管理输入参数
    /// </summary>
    public class ZjnWcsWorkPathListOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 路径编号 
        /// </summary>
        public string PathID { get; set; }
        
        /// <summary>
        /// 路径名称
        /// </summary>
        public string StationFrom { get; set; }
        
        /// <summary>
        /// 起点站点
        /// </summary>
        public string DeviceFrom { get; set; }

        /// <summary>
        /// 起点站点名称
        /// </summary>
        public string DeviceFromName { get; set; }

        /// <summary>
        /// 起点设备
        /// </summary>
        public string StationTo { get; set; }

        /// <summary>
        /// 起点设备名称
        /// </summary>
        public string StationToName { get; set; }

        /// <summary>
        /// 终点站点
        /// </summary>
        public string DeviceTo { get; set; }

        /// <summary>
        /// 终点站点名称
        /// </summary>
        public string DeviceToName { get; set; }

        /// <summary>
        /// 路径类型
        /// </summary>
        public int? PathType { get; set; }
        
        /// <summary>
        /// 有效
        /// </summary>
        public string IsActive { get; set; }
        
        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string Descrip { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 最后修改者
        /// </summary>
        public string F_ModifiedUser { get; set; }
        
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? F_ModifiedTime { get; set; }
        
    }
}