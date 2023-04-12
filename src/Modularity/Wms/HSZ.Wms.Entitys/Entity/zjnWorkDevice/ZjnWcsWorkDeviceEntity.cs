using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 设备信息管理
    /// </summary>
    [SugarTable("zjn_wcs_work_device")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWcsWorkDeviceEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_Id")]
        public string Id { get; set; }
        
        /// <summary>
        /// 设备编号
        /// </summary>
        [SugarColumn(ColumnName = "DeviceID", IsPrimaryKey = true)]        
        public string DeviceId { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        [SugarColumn(ColumnName = "Caption")]        
        public string Caption { get; set; }
        
        /// <summary>
        /// 有效
        /// </summary>
        [SugarColumn(ColumnName = "IsActive")]        
        public int IsActive { get; set; }
        
        /// <summary>
        /// 区域
        /// </summary>
        [SugarColumn(ColumnName = "Region")]        
        public string Region { get; set; }
        
        /// <summary>
        /// 是否控制器
        /// </summary>
        [SugarColumn(ColumnName = "IsController")]        
        public string IsController { get; set; }
        
        /// <summary>
        /// 控制器类型
        /// </summary>
        [SugarColumn(ColumnName = "DeviceType")]        
        public string DeviceType { get; set; }
        
        /// <summary>
        /// 是否为独立线程
        /// </summary>
        [SugarColumn(ColumnName = "IsAlone")]        
        public int IsAlone { get; set; }
        
        /// <summary>
        /// 同线程(一样的只开一线程）
        /// </summary>
        [SugarColumn(ColumnName = "JobGroup")]        
        public string JobGroup { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "Descrip")]        
        public string Descrip { get; set; }

        /// <summary>
        /// 配置对象，json
        /// </summary>
        [SugarColumn(ColumnName = "ObjCfg")]        
        public string ObjCfg { get; set; }

        /// <summary>
        /// plc Id
        /// </summary>
        [SugarColumn(ColumnName = "F_PlcID")]
        public string PlcID { get; set; }

        /// <summary>
        /// PlcIP
        /// </summary>
        [SugarColumn(ColumnName = "PlcIP")]
        public string PlcIP { get; set; }

        /// <summary>
        /// 控制器类型
        /// </summary>
        [SugarColumn(ColumnName = "ControllerType")]
        public string ControllerType { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "ThreadGroup")]
        public string ThreadGroup { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateUser")]
        public string CreateUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 最后修改者
        /// </summary>
        [SugarColumn(ColumnName = "F_ModifiedUser")]
        public string ModifiedUser { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [SugarColumn(ColumnName = "F_ModifiedTime")]
        public DateTime? ModifiedTime { get; set; }
        ///// <summary>
        ///// 有效标志
        ///// </summary>
        //[SugarColumn(ColumnName = "F_EnabledMark")]
        //public int? EnabledMark { get; set; }
        /// <summary>
        /// 是否删除 0未删除 1删除
        /// </summary>
        [SugarColumn(ColumnName = "F_IsDelete")]
        public int? IsDelete { get; set; }
    }
}