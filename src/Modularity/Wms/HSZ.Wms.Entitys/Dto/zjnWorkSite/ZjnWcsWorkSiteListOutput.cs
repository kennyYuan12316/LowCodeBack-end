using SqlSugar;
using System;

namespace HSZ.wms.Entitys.Dto.ZjnWcsWorkSite
{
    /// <summary>
    /// 站点信息管理输入参数
    /// </summary>
    public class ZjnWcsWorkSiteListOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 站点ID
        /// </summary>
        public string StationID { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        public string Capion { get; set; }
        
        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }
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
        /// 站类别
        /// </summary>
        public string StationType { get; set; }

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

        /// <summary>
        /// 行
        /// </summary>
        
        public int? Row { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        
        public int? Cell { get; set; }

        /// <summary>
        /// 层
        /// </summary>
        
        public int? Layer { get; set; }

        /// <summary>
        /// 行2
        /// </summary>
        public int? Row2 { get; set; }

        /// <summary>
        /// 列2
        /// </summary>
        public int? Cell2 { get; set; }

        /// <summary>
        /// 层2
        /// </summary>
        public int? Layer2 { get; set; }
    }
}