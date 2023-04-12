using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWcsWorkDevice
{
    /// <summary>
    /// 设备信息管理修改输入参数
    /// </summary>
    public class ZjnWcsWorkDeviceCrInput
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string deviceId { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        public string caption { get; set; }
        
        /// <summary>
        /// 有效
        /// </summary>
        public int isActive { get; set; }
        
        /// <summary>
        /// 区域
        /// </summary>
        public string region { get; set; }
        
        /// <summary>
        /// 是否控制器
        /// </summary>
        public string isController { get; set; }
        
        /// <summary>
        /// 控制器类型
        /// </summary>
        public string controllerType { get; set; }
        
        /// <summary>
        /// 是否为独立线程
        /// </summary>
        public int isAlone { get; set; }

        /// <summary>
        /// PLC id
        /// </summary>
        public string PlcID { get; set; }
        
        /// <summary>
        /// 同线程(一样的只开一线程）
        /// </summary>
        public string threadGroup { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string descrip { get; set; }
        
        /// <summary>
        /// 设备类型
        /// </summary>
        public string deviceType { get; set; }
        ///// <summary>
        ///// PlcID
        ///// </summary>
        //public string PlcID { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string createUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }

        /// <summary>
        /// 最后修改者
        /// </summary>
        public string modifiedUser { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? modifiedTime { get; set; }

        public int? IsDelete { get; set; }
    }
}