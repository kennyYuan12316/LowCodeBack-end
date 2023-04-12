using System;

namespace HSZ.wms.Entitys.Dto.ZjnWcsWorkDevice
{
    /// <summary>
    /// 设备信息管理输入参数
    /// </summary>
    public class ZjnWcsWorkDeviceListOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceID { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Caption { get; set; }
        
        /// <summary>
        /// 有效
        /// </summary>
        public string IsActive { get; set; }
        
        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }
        
        /// <summary>
        /// 是否控制器
        /// </summary>
        public string IsController { get; set; }
        
        /// <summary>
        /// 控制器类型
        /// </summary>
        public string ControllerType { get; set; }
        
        /// <summary>
        /// 是否为独立线程
        /// </summary>
        public string IsAlone { get; set; }

        /// <summary>
        /// PLC IP地址
        /// </summary>
        public string PlcIP { get; set; }
        
        /// <summary>
        /// 同线程(一样的只开一线程）
        /// </summary>
        public string ThreadGroup { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string Descrip { get; set; }
        
        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// PlcID
        /// </summary>
        public string F_PlcID { get; set; }

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

        public string StackerGroup { get; set; }
    }
}